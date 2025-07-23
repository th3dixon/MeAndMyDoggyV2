using Microsoft.EntityFrameworkCore;
using MeAndMyDog.API.Data;
using MeAndMyDog.API.Models.DTOs.Provider;
using MeAndMyDog.API.Models.Entities;

namespace MeAndMyDog.API.Services;

/// <summary>
/// Service implementation for provider business operations and analytics
/// </summary>
public class ProviderBusinessService : IProviderBusinessService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProviderBusinessService> _logger;

    public ProviderBusinessService(ApplicationDbContext context, ILogger<ProviderBusinessService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProviderDashboardDto> GetDashboardDataAsync(string providerId)
    {
        try
        {
            var provider = await _context.ServiceProviders
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
            {
                throw new ArgumentException("Service provider not found", nameof(providerId));
            }

            var dashboard = new ProviderDashboardDto
            {
                Provider = new ProviderInfoDto
                {
                    Id = provider.Id,
                    BusinessName = provider.BusinessName,
                    OwnerName = provider.User?.UserName ?? "Unknown",
                    Email = provider.BusinessEmail ?? provider.User?.Email ?? "",
                    IsPremium = provider.IsPremium
                },
                Metrics = await GetBusinessMetricsAsync(providerId),
                TodaysBookings = await GetTodaysBookingsAsync(providerId),
                RecentInvoices = await GetRecentInvoicesAsync(providerId, 5),
                QuickStats = await GetQuickStatsAsync(providerId),
                RecentMessages = await GetRecentMessagesAsync(providerId, 3),
                RevenueTrend = await GetRevenueTrendAsync(providerId, 6)
            };

            return dashboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data for provider {ProviderId}", providerId);
            throw;
        }
    }

    public async Task<BusinessMetricsDto> GetBusinessMetricsAsync(string providerId)
    {
        try
        {
            var currentMonth = DateTime.UtcNow.ToString("yyyy-MM");
            var previousMonth = DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM");

            // Get or calculate current month metrics
            var currentMetrics = await GetOrCalculateMetricsAsync(providerId, currentMonth);
            var previousMetrics = await GetOrCalculateMetricsAsync(providerId, previousMonth);

            var revenueGrowth = previousMetrics?.TotalRevenue > 0 
                ? ((currentMetrics.TotalRevenue - previousMetrics.TotalRevenue) / previousMetrics.TotalRevenue) * 100
                : 0;

            return new BusinessMetricsDto
            {
                MonthlyRevenue = currentMetrics.TotalRevenue,
                RevenueGrowth = revenueGrowth,
                ActiveBookings = currentMetrics.ActiveBookings,
                PendingBookings = currentMetrics.PendingBookings,
                TotalClients = currentMetrics.TotalClients,
                NewClients = currentMetrics.NewClients,
                AverageRating = currentMetrics.AverageRating,
                TotalReviews = currentMetrics.TotalReviews
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting business metrics for provider {ProviderId}", providerId);
            throw;
        }
    }

    public async Task<List<TodaysBookingDto>> GetTodaysBookingsAsync(string providerId)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var todayStart = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var todayEnd = today.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            var bookings = await _context.Appointments
                .Include(a => a.PetOwner)
                .Include(a => a.Dog)
                .Where(a => a.ServiceProviderId == providerId &&
                           a.StartTime >= todayStart &&
                           a.StartTime <= todayEnd)
                .OrderBy(a => a.StartTime)
                .Select(a => new TodaysBookingDto
                {
                    Id = a.Id,
                    Time = a.StartTime.ToString("HH:mm"),
                    Service = a.ServiceType,
                    ClientName = a.PetOwner.UserName ?? "Unknown",
                    PetName = a.Dog != null ? a.Dog.Name : "N/A",
                    Duration = $"{(a.EndTime - a.StartTime).TotalMinutes} min",
                    Price = a.Price ?? 0,
                    Status = a.Status,
                    Location = a.Location,
                    Notes = a.Notes
                })
                .ToListAsync();

            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting today's bookings for provider {ProviderId}", providerId);
            throw;
        }
    }

    public async Task<List<RecentInvoiceDto>> GetRecentInvoicesAsync(string providerId, int limit = 10)
    {
        try
        {
            var invoices = await _context.Set<Invoice>()
                .Include(i => i.Client)
                .Where(i => i.ServiceProviderId == providerId)
                .OrderByDescending(i => i.CreatedAt)
                .Take(limit)
                .Select(i => new RecentInvoiceDto
                {
                    Id = i.Id,
                    Number = i.InvoiceNumber,
                    ClientName = i.Client.UserName ?? "Unknown",
                    Amount = i.TotalAmount,
                    Status = i.Status,
                    Date = i.IssueDate.ToString("yyyy-MM-dd"),
                    DueDate = i.DueDate.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return invoices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent invoices for provider {ProviderId}", providerId);
            return new List<RecentInvoiceDto>();
        }
    }

    public async Task<QuickStatsDto> GetQuickStatsAsync(string providerId)
    {
        try
        {
            var weekStart = DateTime.UtcNow.StartOfWeek(DayOfWeek.Monday);
            var weekEnd = weekStart.AddDays(7);

            var weeklyRevenue = await _context.Set<Invoice>()
                .Where(i => i.ServiceProviderId == providerId &&
                           i.Status == "Paid" &&
                           i.PaidDate >= weekStart &&
                           i.PaidDate < weekEnd)
                .SumAsync(i => i.TotalAmount);

            var pendingPayments = await _context.Set<Invoice>()
                .Where(i => i.ServiceProviderId == providerId &&
                           i.Status == "Pending")
                .SumAsync(i => i.TotalAmount);

            var completedServices = await _context.Appointments
                .CountAsync(a => a.ServiceProviderId == providerId &&
                               a.Status == "Completed" &&
                               a.StartTime >= weekStart &&
                               a.StartTime < weekEnd);

            // Calculate response rate (simplified - would need message tracking)
            var responseRate = 98m; // Placeholder

            return new QuickStatsDto
            {
                WeekRevenue = weeklyRevenue,
                PendingPayments = pendingPayments,
                CompletedServices = completedServices,
                ResponseRate = responseRate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quick stats for provider {ProviderId}", providerId);
            throw;
        }
    }

    public async Task<RevenueTrendDto> GetRevenueTrendAsync(string providerId, int months = 6)
    {
        try
        {
            var trends = new RevenueTrendDto();
            var currentDate = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var month = currentDate.AddMonths(-i);
                var monthKey = month.ToString("yyyy-MM");
                var monthStart = new DateTime(month.Year, month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                var monthEnd = monthStart.AddMonths(1);

                var monthlyRevenue = await _context.Set<Invoice>()
                    .Where(i => i.ServiceProviderId == providerId &&
                               i.Status == "Paid" &&
                               i.PaidDate >= monthStart &&
                               i.PaidDate < monthEnd)
                    .SumAsync(i => i.TotalAmount);

                trends.Labels.Add(month.ToString("MMM"));
                trends.Data.Add(monthlyRevenue);
                trends.TotalRevenue += monthlyRevenue;
            }

            if (trends.Data.Count > 1)
            {
                var growthRates = new List<decimal>();
                for (int i = 1; i < trends.Data.Count; i++)
                {
                    if (trends.Data[i - 1] > 0)
                    {
                        var growth = ((trends.Data[i] - trends.Data[i - 1]) / trends.Data[i - 1]) * 100;
                        growthRates.Add(growth);
                    }
                }
                trends.AverageMonthlyGrowth = growthRates.Count > 0 ? growthRates.Average() : 0;
            }

            return trends;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting revenue trend for provider {ProviderId}", providerId);
            throw;
        }
    }

    public async Task<List<RecentMessageDto>> GetRecentMessagesAsync(string providerId, int limit = 5)
    {
        try
        {
            // Placeholder implementation - would need actual messaging system
            var messages = new List<RecentMessageDto>
            {
                new()
                {
                    Id = "1",
                    ClientName = "Sarah Johnson",
                    ClientAvatar = "https://images.unsplash.com/photo-1494790108755-2616b612b5bb?w=150",
                    Preview = "Hi! Could you walk Buddy an extra 15 minutes today?",
                    Time = "10 min ago",
                    Unread = true
                },
                new()
                {
                    Id = "2",
                    ClientName = "Michael Brown",
                    ClientAvatar = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150",
                    Preview = "Thank you for the excellent grooming service!",
                    Time = "2 hours ago",
                    Unread = false
                }
            };

            return messages.Take(limit).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent messages for provider {ProviderId}", providerId);
            return new List<RecentMessageDto>();
        }
    }

    public async Task<CreateInvoiceResponseDto> CreateInvoiceAsync(string providerId, CreateInvoiceDto createInvoiceDto)
    {
        try
        {
            var vatAmount = createInvoiceDto.Amount * (createInvoiceDto.VatRate / 100);
            var totalAmount = createInvoiceDto.Amount + vatAmount;

            var invoice = new Invoice
            {
                ServiceProviderId = providerId,
                ClientId = createInvoiceDto.ClientId,
                AppointmentId = createInvoiceDto.AppointmentId,
                InvoiceNumber = await GenerateInvoiceNumberAsync(providerId),
                Description = createInvoiceDto.Description,
                Amount = createInvoiceDto.Amount,
                VatRate = createInvoiceDto.VatRate,
                VatAmount = vatAmount,
                TotalAmount = totalAmount,
                IssueDate = DateTimeOffset.UtcNow,
                DueDate = createInvoiceDto.DueDate,
                Notes = createInvoiceDto.Notes,
                Terms = createInvoiceDto.Terms,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _context.Set<Invoice>().Add(invoice);
            await _context.SaveChangesAsync();

            return new CreateInvoiceResponseDto
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.TotalAmount,
                Success = true,
                Message = "Invoice created successfully"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for provider {ProviderId}", providerId);
            return new CreateInvoiceResponseDto
            {
                Success = false,
                Message = $"Error creating invoice: {ex.Message}"
            };
        }
    }

    public async Task<InvoiceDetailsDto?> GetInvoiceDetailsAsync(string providerId, string invoiceId)
    {
        try
        {
            var invoice = await _context.Set<Invoice>()
                .Include(i => i.Client)
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.ServiceProviderId == providerId);

            if (invoice == null) return null;

            return new InvoiceDetailsDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                ClientName = invoice.Client.UserName ?? "Unknown",
                ClientEmail = invoice.Client.Email ?? "",
                Description = invoice.Description,
                Amount = invoice.Amount,
                VatRate = invoice.VatRate,
                VatAmount = invoice.VatAmount,
                TotalAmount = invoice.TotalAmount,
                Currency = invoice.Currency,
                Status = invoice.Status,
                IssueDate = invoice.IssueDate.ToString("yyyy-MM-dd"),
                DueDate = invoice.DueDate.ToString("yyyy-MM-dd"),
                PaidDate = invoice.PaidDate?.ToString("yyyy-MM-dd"),
                PaymentMethod = invoice.PaymentMethod,
                PaymentReference = invoice.PaymentReference,
                Notes = invoice.Notes,
                Terms = invoice.Terms,
                IsSent = invoice.IsSent,
                SentDate = invoice.SentDate?.ToString("yyyy-MM-dd")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice details {InvoiceId} for provider {ProviderId}", invoiceId, providerId);
            return null;
        }
    }

    public async Task<bool> UpdateInvoiceStatusAsync(string providerId, string invoiceId, string status, string? paymentMethod = null, string? paymentReference = null)
    {
        try
        {
            var invoice = await _context.Set<Invoice>()
                .FirstOrDefaultAsync(i => i.Id == invoiceId && i.ServiceProviderId == providerId);

            if (invoice == null) return false;

            invoice.Status = status;
            invoice.UpdatedAt = DateTimeOffset.UtcNow;

            if (status == "Paid")
            {
                invoice.PaidDate = DateTimeOffset.UtcNow;
                invoice.PaymentMethod = paymentMethod;
                invoice.PaymentReference = paymentReference;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice status {InvoiceId} for provider {ProviderId}", invoiceId, providerId);
            return false;
        }
    }

    public async Task<bool> CalculateBusinessMetricsAsync(string providerId, string period)
    {
        try
        {
            var metrics = await GetOrCalculateMetricsAsync(providerId, period);
            return metrics != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating business metrics for provider {ProviderId} period {Period}", providerId, period);
            return false;
        }
    }

    public async Task<List<ClientDto>> GetClientsAsync(string providerId)
    {
        try
        {
            var clients = await _context.Appointments
                .Include(a => a.PetOwner)
                .Where(a => a.ServiceProviderId == providerId)
                .Select(a => a.PetOwner)
                .Distinct()
                .Select(u => new ClientDto
                {
                    Id = u.Id,
                    Name = u.UserName ?? "Unknown",
                    Email = u.Email ?? "",
                    Phone = u.PhoneNumber
                })
                .ToListAsync();

            return clients;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting clients for provider {ProviderId}", providerId);
            return new List<ClientDto>();
        }
    }

    private async Task<ProviderBusinessMetrics> GetOrCalculateMetricsAsync(string providerId, string period)
    {
        var existingMetrics = await _context.Set<ProviderBusinessMetrics>()
            .FirstOrDefaultAsync(m => m.ServiceProviderId == providerId && m.MetricsPeriod == period);

        if (existingMetrics != null && existingMetrics.CalculatedAt > DateTimeOffset.UtcNow.AddDays(-1))
        {
            return existingMetrics;
        }

        // Calculate metrics for the period
        var periodDate = DateTime.ParseExact(period, "yyyy-MM", null);
        var periodStart = new DateTime(periodDate.Year, periodDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = periodStart.AddMonths(1);

        var metrics = existingMetrics ?? new ProviderBusinessMetrics
        {
            ServiceProviderId = providerId,
            MetricsPeriod = period,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Calculate revenue
        metrics.TotalRevenue = await _context.Set<Invoice>()
            .Where(i => i.ServiceProviderId == providerId &&
                       i.Status == "Paid" &&
                       i.PaidDate >= periodStart &&
                       i.PaidDate < periodEnd)
            .SumAsync(i => i.TotalAmount);

        // Calculate bookings
        var bookings = await _context.Appointments
            .Where(a => a.ServiceProviderId == providerId &&
                       a.StartTime >= periodStart &&
                       a.StartTime < periodEnd)
            .ToListAsync();

        metrics.ActiveBookings = bookings.Count(b => b.Status == "Confirmed" || b.Status == "InProgress");
        metrics.PendingBookings = bookings.Count(b => b.Status == "Scheduled");
        metrics.CompletedBookings = bookings.Count(b => b.Status == "Completed");
        metrics.CancelledBookings = bookings.Count(b => b.Status == "Cancelled");

        // Calculate clients
        var clientIds = bookings.Select(b => b.PetOwnerId).Distinct().ToList();
        metrics.TotalClients = clientIds.Count;

        // Calculate average booking value
        var completedBookingsWithPrice = bookings.Where(b => b.Status == "Completed" && b.Price.HasValue).ToList();
        metrics.AverageBookingValue = completedBookingsWithPrice.Any()
            ? (decimal)completedBookingsWithPrice.Average(b => (double)b.Price!.Value)
            : 0;

        // Calculate ratings
        var reviews = await _context.ServiceProviderReviews
            .Where(r => r.ServiceProviderId == providerId &&
                       r.CreatedAt >= periodStart &&
                       r.CreatedAt < periodEnd)
            .ToListAsync();

        if (reviews.Any())
        {
            metrics.AverageRating = (decimal)reviews.Average(r => r.Rating);
            metrics.TotalReviews = reviews.Count;
        }

        metrics.CalculatedAt = DateTimeOffset.UtcNow;
        metrics.UpdatedAt = DateTimeOffset.UtcNow;

        if (existingMetrics == null)
        {
            _context.Set<ProviderBusinessMetrics>().Add(metrics);
        }

        await _context.SaveChangesAsync();
        return metrics;
    }

    private async Task<string> GenerateInvoiceNumberAsync(string providerId)
    {
        var year = DateTime.UtcNow.Year;
        var lastInvoice = await _context.Set<Invoice>()
            .Where(i => i.ServiceProviderId == providerId && i.InvoiceNumber.StartsWith($"INV-{year}-"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastInvoice != null)
        {
            var lastNumberPart = lastInvoice.InvoiceNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastNumberPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"INV-{year}-{nextNumber:D3}";
    }
}

/// <summary>
/// Extension method for DateTime to get start of week
/// </summary>
public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}