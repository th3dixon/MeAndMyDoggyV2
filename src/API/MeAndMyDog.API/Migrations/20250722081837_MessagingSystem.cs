using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAndMyDog.API.Migrations
{
    /// <summary>
    /// Database migration for implementing the complete messaging system with conversations, messages, and real-time communication features
    /// </summary>
    /// <inheritdoc />
    public partial class MessagingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "VideoCallSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "IceCandidates",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecorded",
                table: "VideoCallSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "VideoCallSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NetworkQuality",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QualityRating",
                table: "VideoCallSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecordingUrl",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ScreenSharingUsed",
                table: "VideoCallSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SdpAnswer",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SdpOffer",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "VideoCallSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Messages",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Messages",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditHistory",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EditedAt",
                table: "Messages",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEncrypted",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Mentions",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentMessageId",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SentAt",
                table: "Messages",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceInfo",
                table: "MessageReadReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                table: "MessageAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "MessageAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "MessageAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "MessageAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "MessageAttachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "MessageAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConversationType",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessageId",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastMessagePreview",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "Conversations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Conversations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "ConversationParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "ConversationParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "ConversationParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastReadAt",
                table: "ConversationParticipants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastReadMessageId",
                table: "ConversationParticipants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "MutedUntil",
                table: "ConversationParticipants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ConversationParticipants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UnreadCount",
                table: "ConversationParticipants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CalendarAppointments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MeetingUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AppointmentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAllDay = table.Column<bool>(type: "bit", nullable: false),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecurrenceInterval = table.Column<int>(type: "int", nullable: true),
                    RecurrenceEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "int", nullable: true),
                    ExternalEventId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RemindersEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DefaultReminderMinutes = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AttachmentFiles = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarAppointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CalendarIntegrations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExternalCalendarId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CalendarName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SyncToExternal = table.Column<bool>(type: "bit", nullable: false),
                    SyncFromExternal = table.Column<bool>(type: "bit", nullable: false),
                    SyncDirection = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TokenExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastSyncAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NextSyncAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SyncFrequencyMinutes = table.Column<int>(type: "int", nullable: false),
                    AutoSyncEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastSyncStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastSyncError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SyncFailureCount = table.Column<int>(type: "int", nullable: false),
                    MaxSyncFailures = table.Column<int>(type: "int", nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConfigurationSettings = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastVerifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarIntegrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationEncryptionKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KeyId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EncryptedKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeySalt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KeyVersion = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationEncryptionKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationEncryptionKeys_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversationEncryptionKeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileUploadRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniqueFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    UploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessingStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PublicUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScanStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScanResultJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadCount = table.Column<int>(type: "int", nullable: false),
                    IsEncrypted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploadRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationBookmarks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlaceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "general"),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationBookmarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationShares",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: true),
                    Altitude = table.Column<double>(type: "float", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlaceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LocationType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsLive = table.Column<bool>(type: "bit", nullable: false),
                    LiveExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LiveUpdateIntervalSeconds = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Visibility = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "conversation"),
                    SharedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationShares_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocationShares_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationShares_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageEncryptions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Algorithm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KeyDerivationFunction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    InitializationVector = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KeyDerivationIterations = table.Column<int>(type: "int", nullable: false),
                    AuthenticationTag = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KeyId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EncryptionVersion = table.Column<int>(type: "int", nullable: false),
                    IsEndToEndEncrypted = table.Column<bool>(type: "bit", nullable: false),
                    ContentHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdditionalAuthenticatedData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageEncryptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageEncryptions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageSearches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Query = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SenderId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DateTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IncludeAttachments = table.Column<bool>(type: "bit", nullable: false),
                    IncludeVoiceMessages = table.Column<bool>(type: "bit", nullable: false),
                    IncludeEncryptedMessages = table.Column<bool>(type: "bit", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSearches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageSearches_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MessageSearches_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageSecurities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SecurityLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequiresAuthentication = table.Column<bool>(type: "bit", nullable: false),
                    RequiresVerification = table.Column<bool>(type: "bit", nullable: false),
                    VerificationMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasWatermark = table.Column<bool>(type: "bit", nullable: false),
                    WatermarkText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BlockScreenshots = table.Column<bool>(type: "bit", nullable: false),
                    BlockCopyPaste = table.Column<bool>(type: "bit", nullable: false),
                    BlockRightClick = table.Column<bool>(type: "bit", nullable: false),
                    BlockForwarding = table.Column<bool>(type: "bit", nullable: false),
                    AllowDownload = table.Column<bool>(type: "bit", nullable: false),
                    AllowPrint = table.Column<bool>(type: "bit", nullable: false),
                    AccessExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    GeographicRestrictions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TimeRestrictions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeviceRestrictions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IpWhitelist = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IpBlacklist = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RequiredClearanceLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataClassification = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EnableAuditLogging = table.Column<bool>(type: "bit", nullable: false),
                    EnableAccessAnalytics = table.Column<bool>(type: "bit", nullable: false),
                    CustomPolicies = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ConfiguredByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageSecurities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageSecurities_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsShared = table.Column<bool>(type: "bit", nullable: false),
                    IsSystemTemplate = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTemplates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageTranslations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SourceLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TargetLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SourceText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslatedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: true),
                    TranslationProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TranslationMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsCached = table.Column<bool>(type: "bit", nullable: false),
                    CharacterCount = table.Column<int>(type: "int", nullable: false),
                    TranslationCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QualityRating = table.Column<int>(type: "int", nullable: true),
                    QualityFeedback = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastAccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccessCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTranslations_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageTranslations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDevices",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AppVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OsVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NotificationsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LastSeenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PushEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmailEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SmsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    InAppEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CustomSound = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuietHoursStart = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    QuietHoursEnd = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuietHoursDays = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MinInterval = table.Column<int>(type: "int", nullable: true),
                    MaxPerDay = table.Column<int>(type: "int", nullable: true),
                    MinPriority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CustomConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SearchHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SearchQuery = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SearchFilters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultCount = table.Column<int>(type: "int", nullable: false),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false),
                    HasInteraction = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SearchHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecurityIncidents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IncidentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    OccurredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DetectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DetectionMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedInvestigator = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    InvestigationNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    RemediationActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResolutionSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ClientInformation = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    AuthoritiesNotified = table.Column<bool>(type: "bit", nullable: false),
                    UsersNotified = table.Column<bool>(type: "bit", nullable: false),
                    IncidentData = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityIncidents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfDestructMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SetByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    DestructMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimerSeconds = table.Column<int>(type: "int", nullable: false),
                    TriggerEvent = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TimerStartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DestructAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDestroyed = table.Column<bool>(type: "bit", nullable: false),
                    DestroyedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DestructionMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NotifyOnDestruction = table.Column<bool>(type: "bit", nullable: false),
                    ShowCountdown = table.Column<bool>(type: "bit", nullable: false),
                    MaxViews = table.Column<int>(type: "int", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    BlockScreenshots = table.Column<bool>(type: "bit", nullable: false),
                    SecurityOptions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfDestructMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelfDestructMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationCache",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TextHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SourceLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TargetLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SourceText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TranslatedText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfidenceScore = table.Column<double>(type: "float", nullable: true),
                    TranslationProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CharacterCount = table.Column<int>(type: "int", nullable: false),
                    UsageCount = table.Column<int>(type: "int", nullable: false),
                    AverageQualityRating = table.Column<double>(type: "float", nullable: true),
                    QualityRatingCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserEncryptionKeys",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KeyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KeyUsage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EncryptedPrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KeySizeBits = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevokedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RevocationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEncryptionKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEncryptionKeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLanguagePreferences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    PrimaryLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SecondaryLanguages = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AutoTranslateIncoming = table.Column<bool>(type: "bit", nullable: false),
                    AutoDetectOutgoing = table.Column<bool>(type: "bit", nullable: false),
                    PreferredProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinConfidenceThreshold = table.Column<double>(type: "float", nullable: false),
                    ShowConfidenceScores = table.Column<bool>(type: "bit", nullable: false),
                    EnableTranslationCache = table.Column<bool>(type: "bit", nullable: false),
                    ExcludeLanguages = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnableSuggestions = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLanguagePreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLanguagePreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoCallParticipants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VideoCallSessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LeftAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VideoEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AudioEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ScreenSharing = table.Column<bool>(type: "bit", nullable: false),
                    ConnectionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetworkQuality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallAccepted = table.Column<bool>(type: "bit", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeerConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudioLevel = table.Column<int>(type: "int", nullable: false),
                    IsSpeaking = table.Column<bool>(type: "bit", nullable: false),
                    DeviceInfo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoCallParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoCallParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoCallParticipants_VideoCallSessions_VideoCallSessionId",
                        column: x => x.VideoCallSessionId,
                        principalTable: "VideoCallSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AudioFormat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DurationSeconds = table.Column<double>(type: "float", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    SampleRate = table.Column<int>(type: "int", nullable: false),
                    BitRate = table.Column<int>(type: "int", nullable: false),
                    Channels = table.Column<int>(type: "int", nullable: false),
                    IsTranscribed = table.Column<bool>(type: "bit", nullable: false),
                    TranscriptionText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranscriptionConfidence = table.Column<double>(type: "float", nullable: true),
                    TranscriptionLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsProcessing = table.Column<bool>(type: "bit", nullable: false),
                    ProcessingStatus = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WaveformData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPlayed = table.Column<bool>(type: "bit", nullable: false),
                    PlayCount = table.Column<int>(type: "int", nullable: false),
                    FirstPlayedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastPlayedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AutoDeleteAfterPlay = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentInstances",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentAppointmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    InstanceNumber = table.Column<int>(type: "int", nullable: false),
                    OriginalStartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    OriginalEndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualStartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ActualEndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CustomTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CustomDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CustomLocation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalEventId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentInstances_CalendarAppointments_ParentAppointmentId",
                        column: x => x.ParentAppointmentId,
                        principalTable: "CalendarAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentParticipants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppointmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResponseStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsOrganizer = table.Column<bool>(type: "bit", nullable: false),
                    InvitedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RespondedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalParticipantId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentParticipants_CalendarAppointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "CalendarAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentReminders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppointmentId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ReminderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinutesBefore = table.Column<int>(type: "int", nullable: false),
                    ReminderTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeliveryError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    NextRetryAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CustomMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentReminders_CalendarAppointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "CalendarAppointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantKeyShares",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConversationEncryptionKeyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EncryptedKeyShare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicKeyFingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsAcknowledged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AcknowledgedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantKeyShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantKeyShares_ConversationEncryptionKeys_ConversationEncryptionKeyId",
                        column: x => x.ConversationEncryptionKeyId,
                        principalTable: "ConversationEncryptionKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantKeyShares_Users_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "LocationUpdates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LocationShareId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: true),
                    Altitude = table.Column<double>(type: "float", nullable: true),
                    Speed = table.Column<double>(type: "float", nullable: true),
                    Bearing = table.Column<double>(type: "float", nullable: true),
                    BatteryLevel = table.Column<int>(type: "int", nullable: true),
                    LocationSource = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CapturedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReceivedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationUpdates_LocationShares_LocationShareId",
                        column: x => x.LocationShareId,
                        principalTable: "LocationShares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageAccessLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MessageSecurityId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    AccessType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    ClientUserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GeographicLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AccessGranted = table.Column<bool>(type: "bit", nullable: false),
                    DenialReason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    VerificationMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RiskScore = table.Column<double>(type: "float", nullable: false),
                    TriggeredAlert = table.Column<bool>(type: "bit", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AccessMetadata = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAccessLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageAccessLogs_MessageSecurities_MessageSecurityId",
                        column: x => x.MessageSecurityId,
                        principalTable: "MessageSecurities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TemplateId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TemplateContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateVariables = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    NextOccurrence = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RecurrenceEndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "int", nullable: true),
                    OccurrenceCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SentMessageId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduledMessages_MessageTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "MessageTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduledMessages_Messages_SentMessageId",
                        column: x => x.SentMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ScheduledMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "PushNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TargetDeviceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Sound = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Badge = table.Column<int>(type: "int", nullable: true),
                    CustomData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TimeToLive = table.Column<int>(type: "int", nullable: true),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushNotifications_NotificationDevices_TargetDeviceId",
                        column: x => x.TargetDeviceId,
                        principalTable: "NotificationDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PushNotifications_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "MessageViewTrackings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SelfDestructMessageId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ViewedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ViewedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ViewDurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    ClientUserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeviceFingerprint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TriggeredTimer = table.Column<bool>(type: "bit", nullable: false),
                    ViewMetadata = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageViewTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageViewTrackings_SelfDestructMessages_SelfDestructMessageId",
                        column: x => x.SelfDestructMessageId,
                        principalTable: "SelfDestructMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeliveries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotificationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderMessageId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttemptedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeliveredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    OpenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ErrorCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ProviderResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_NotificationDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "NotificationDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationDeliveries_PushNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "PushNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentInstances_ParentAppointmentId",
                table: "AppointmentInstances",
                column: "ParentAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentParticipants_AppointmentId",
                table: "AppointmentParticipants",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReminders_AppointmentId",
                table: "AppointmentReminders",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationEncryptionKeys_ConversationId",
                table: "ConversationEncryptionKeys",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationEncryptionKeys_UserId",
                table: "ConversationEncryptionKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookmarks_Category_Privacy",
                table: "LocationBookmarks",
                columns: new[] { "Category", "IsPrivate" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookmarks_Coordinates",
                table: "LocationBookmarks",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookmarks_Usage",
                table: "LocationBookmarks",
                column: "UsageCount");

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookmarks_User_Active",
                table: "LocationBookmarks",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationBookmarks_User_Name",
                table: "LocationBookmarks",
                columns: new[] { "UserId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationShares_Conversation_Date",
                table: "LocationShares",
                columns: new[] { "ConversationId", "SharedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationShares_Coordinates",
                table: "LocationShares",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationShares_LiveExpiry",
                table: "LocationShares",
                column: "LiveExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationShares_MessageId",
                table: "LocationShares",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationShares_User_Live",
                table: "LocationShares",
                columns: new[] { "UserId", "IsLive", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_LocationUpdates_Captured",
                table: "LocationUpdates",
                column: "CapturedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LocationUpdates_Share_Date",
                table: "LocationUpdates",
                columns: new[] { "LocationShareId", "ReceivedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageAccessLogs_MessageSecurityId",
                table: "MessageAccessLogs",
                column: "MessageSecurityId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageEncryptions_MessageId",
                table: "MessageEncryptions",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSearches_ConversationId",
                table: "MessageSearches",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSearches_Usage",
                table: "MessageSearches",
                column: "UsageCount");

            migrationBuilder.CreateIndex(
                name: "IX_MessageSearches_User_Active",
                table: "MessageSearches",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageSearches_User_Pinned",
                table: "MessageSearches",
                columns: new[] { "UserId", "IsPinned" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageSecurities_MessageId",
                table: "MessageSecurities",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_IsSystemTemplate",
                table: "MessageTemplates",
                column: "IsSystemTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplates_UserId_Category",
                table: "MessageTemplates",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTranslations_MessageId",
                table: "MessageTranslations",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTranslations_UserId",
                table: "MessageTranslations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageViewTrackings_SelfDestructMessageId",
                table: "MessageViewTrackings",
                column: "SelfDestructMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_DeviceId",
                table: "NotificationDeliveries",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDeliveries_NotificationId",
                table: "NotificationDeliveries",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDevices_UserId_DeviceToken",
                table: "NotificationDevices",
                columns: new[] { "UserId", "DeviceToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId_NotificationType",
                table: "NotificationPreferences",
                columns: new[] { "UserId", "NotificationType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantKeyShares_ConversationEncryptionKeyId",
                table: "ParticipantKeyShares",
                column: "ConversationEncryptionKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantKeyShares_ParticipantId",
                table: "ParticipantKeyShares",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotifications_TargetDeviceId",
                table: "PushNotifications",
                column: "TargetDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PushNotifications_TargetUserId",
                table: "PushNotifications",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_ConversationId",
                table: "ScheduledMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_NextOccurrence",
                table: "ScheduledMessages",
                column: "NextOccurrence");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_ScheduledAt",
                table: "ScheduledMessages",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_SenderId_Status",
                table: "ScheduledMessages",
                columns: new[] { "SenderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_SentMessageId",
                table: "ScheduledMessages",
                column: "SentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledMessages_TemplateId",
                table: "ScheduledMessages",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_Date",
                table: "SearchHistory",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_Query",
                table: "SearchHistory",
                column: "SearchQuery");

            migrationBuilder.CreateIndex(
                name: "IX_SearchHistory_User_Date",
                table: "SearchHistory",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SelfDestructMessages_MessageId",
                table: "SelfDestructMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEncryptionKeys_UserId",
                table: "UserEncryptionKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLanguagePreferences_UserId",
                table: "UserLanguagePreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallParticipants_UserId",
                table: "VideoCallParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallParticipants_VideoCallSessionId",
                table: "VideoCallParticipants",
                column: "VideoCallSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceMessages_MessageId",
                table: "VoiceMessages",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentInstances");

            migrationBuilder.DropTable(
                name: "AppointmentParticipants");

            migrationBuilder.DropTable(
                name: "AppointmentReminders");

            migrationBuilder.DropTable(
                name: "CalendarIntegrations");

            migrationBuilder.DropTable(
                name: "FileUploadRecord");

            migrationBuilder.DropTable(
                name: "LocationBookmarks");

            migrationBuilder.DropTable(
                name: "LocationUpdates");

            migrationBuilder.DropTable(
                name: "MessageAccessLogs");

            migrationBuilder.DropTable(
                name: "MessageEncryptions");

            migrationBuilder.DropTable(
                name: "MessageSearches");

            migrationBuilder.DropTable(
                name: "MessageTranslations");

            migrationBuilder.DropTable(
                name: "MessageViewTrackings");

            migrationBuilder.DropTable(
                name: "NotificationDeliveries");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "ParticipantKeyShares");

            migrationBuilder.DropTable(
                name: "ScheduledMessages");

            migrationBuilder.DropTable(
                name: "SearchHistory");

            migrationBuilder.DropTable(
                name: "SecurityIncidents");

            migrationBuilder.DropTable(
                name: "TranslationCache");

            migrationBuilder.DropTable(
                name: "UserEncryptionKeys");

            migrationBuilder.DropTable(
                name: "UserLanguagePreferences");

            migrationBuilder.DropTable(
                name: "VideoCallParticipants");

            migrationBuilder.DropTable(
                name: "VoiceMessages");

            migrationBuilder.DropTable(
                name: "CalendarAppointments");

            migrationBuilder.DropTable(
                name: "LocationShares");

            migrationBuilder.DropTable(
                name: "MessageSecurities");

            migrationBuilder.DropTable(
                name: "SelfDestructMessages");

            migrationBuilder.DropTable(
                name: "PushNotifications");

            migrationBuilder.DropTable(
                name: "ConversationEncryptionKeys");

            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "NotificationDevices");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "IceCandidates",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "IsRecorded",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "NetworkQuality",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "QualityRating",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "RecordingUrl",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "ScreenSharingUsed",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "SdpAnswer",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "SdpOffer",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EditHistory",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsEncrypted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Mentions",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ParentMessageId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeviceInfo",
                table: "MessageReadReceipts");

            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "MessageAttachments");

            migrationBuilder.DropColumn(
                name: "ConversationType",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "LastMessagePreview",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "LastReadAt",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "LastReadMessageId",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "MutedUntil",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "ConversationParticipants");

            migrationBuilder.DropColumn(
                name: "UnreadCount",
                table: "ConversationParticipants");
        }
    }
}
