using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.Entities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeAndMyDog.API.MigrationTests
{
    [TestFixture]
    public class MigrationIntegrationTests
    {
        private ApplicationDbContext _context;
        private string _connectionString;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Testing.json")
                .Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [Test]
        public async Task Migration_ShouldMaintainUserData()
        {
            // Arrange
            var userCountBefore = await _context.Users.CountAsync();

            // Act - This assumes migration has been applied
            await _context.Database.EnsureCreatedAsync();

            // Assert
            var userCountAfter = await _context.Users.CountAsync();
            Assert.That(userCountAfter, Is.GreaterThanOrEqualTo(userCountBefore));
        }

        [Test]
        public async Task Migration_ShouldMaintainServiceProviderRelationships()
        {
            // Arrange & Act
            var serviceProviders = await _context.ServiceProviders
                .AsNoTracking() // Read-only query optimization for tests
                .Include(sp => sp.User)
                .Include(sp => sp.Services)
                .Include(sp => sp.Reviews)
                .Take(10)
                .ToListAsync();

            // Assert
            Assert.That(serviceProviders, Is.Not.Empty);
            foreach (var sp in serviceProviders)
            {
                Assert.That(sp.User, Is.Not.Null);
                Assert.That(sp.UserId, Is.Not.EqualTo(Guid.Empty));
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainDogProfileIntegrity()
        {
            // Arrange & Act
            var dogProfiles = await _context.DogProfiles
                .AsNoTracking() // Read-only query optimization for tests
                .Include(dp => dp.Owner)
                .Include(dp => dp.MedicalRecords)
                .Take(10)
                .ToListAsync();

            // Assert
            foreach (var dog in dogProfiles)
            {
                Assert.That(dog.Owner, Is.Not.Null);
                Assert.That(dog.OwnerId, Is.Not.Empty);
                Assert.That(dog.Name, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainBookingRelationships()
        {
            // Arrange & Act
            var bookings = await _context.Bookings
                .AsNoTracking() // Read-only query optimization for tests
                .Include(b => b.ServiceProvider)
                .Include(b => b.Customer)
                .Include(b => b.Dog)
                .Where(b => b.Status == "Scheduled")
                .Take(10)
                .ToListAsync();

            // Assert
            foreach (var booking in bookings)
            {
                Assert.That(booking.ServiceProvider, Is.Not.Null);
                Assert.That(booking.Customer, Is.Not.Null);
                Assert.That(booking.BookingReference, Is.Not.Null.And.Not.Empty);
            }
        }

        [Test]
        public async Task Migration_ShouldMaintainMessagingIntegrity()
        {
            // Arrange & Act
            var conversations = await _context.Conversations
                .AsNoTracking() // Read-only query optimization for tests
                .Include(c => c.Messages)
                .Include(c => c.Participants)
                .Where(c => c.IsActive)
                .Take(5)
                .ToListAsync();

            // Assert
            foreach (var conversation in conversations)
            {
                Assert.That(conversation.Participants, Is.Not.Empty);
                Assert.That(conversation.CreatedBy, Is.Not.Empty);
            }
        }

        [Test]
        [TestCase("AspNetUsers")]
        [TestCase("ServiceProviders")]
        [TestCase("DogProfiles")]
        [TestCase("Bookings")]
        [TestCase("Messages")]
        public async Task Migration_ShouldMaintainTableIndexes(string tableName)
        {
            // Arrange
            var query = @"
                SELECT i.name AS IndexName
                FROM sys.indexes i
                INNER JOIN sys.tables t ON i.object_id = t.object_id
                WHERE t.name = @p0 AND i.type > 0";

            // Act
            var indexes = await _context.Database
                .SqlQueryRaw<string>(query, tableName)
                .ToListAsync();

            // Assert
            Assert.That(indexes, Is.Not.Empty, $"Table {tableName} should have indexes");
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }
    }
}