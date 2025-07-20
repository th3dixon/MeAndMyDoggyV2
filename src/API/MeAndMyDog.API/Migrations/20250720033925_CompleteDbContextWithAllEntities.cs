using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAndMyDog.API.Migrations
{
    /// <inheritdoc />
    public partial class CompleteDbContextWithAllEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_KYCVerifications_UserId",
                table: "KYCVerifications");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_UserId",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_User");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VideoCallSessions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "VideoCallSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "InitiatorId",
                table: "VideoCallSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ConversationId",
                table: "VideoCallSessions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "VideoCallSessions",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "SubscriptionPlans",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SubscriptionPlans",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionPlans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Features",
                table: "SubscriptionPlans",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SubscriptionPlans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SubscriptionPlans",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "BillingCycle",
                table: "SubscriptionPlans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Monthly",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MessageReadReceipts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReadAt",
                table: "MessageReadReceipts",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "MessageId",
                table: "MessageReadReceipts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MessageReactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Reaction",
                table: "MessageReactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MessageId",
                table: "MessageReactions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "MessageReactions",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "VerifiedBy",
                table: "KYCVerifications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VerificationResult",
                table: "KYCVerifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SubmittedAt",
                table: "KYCVerifications",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "KYCVerifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "KYCVerifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "KYCVerifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "KYCVerifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentImageUrl",
                table: "KYCVerifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UploadedAt",
                table: "KYCDocuments",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "KYCVerificationId",
                table: "KYCDocuments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "KYCDocuments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentUrl",
                table: "KYCDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "KYCDocuments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "JoinedAt",
                table: "ConversationParticipants",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceProviderId",
                table: "AvailabilitySlots",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                table: "AvailabilitySlots",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AvailabilitySlots",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "AvailabilitySlots",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AIUsageTrackings",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "AIUsageTrackings",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "AIUsageTrackings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "AIUsageTrackings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FeatureType",
                table: "AIUsageTrackings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "AIUsageTrackings",
                type: "decimal(10,6)",
                precision: 10,
                scale: 6,
                nullable: false,
                defaultValue: 0.0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ToxicityScore",
                table: "AIContentModerations",
                type: "decimal(5,4)",
                precision: 5,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "AIContentModerations",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "ModerationResult",
                table: "AIContentModerations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "AIContentModerations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "gemini-1.5-flash",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Flags",
                table: "AIContentModerations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "AIContentModerations",
                type: "decimal(10,6)",
                precision: 10,
                scale: 6,
                nullable: false,
                defaultValue: 0.0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "AIContentModerations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentId",
                table: "AIContentModerations",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AIContentModerations",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallSessions_Conversation_StartTime",
                table: "VideoCallSessions",
                columns: new[] { "ConversationId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallSessions_Initiator",
                table: "VideoCallSessions",
                column: "InitiatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCallSessions_Status",
                table: "VideoCallSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_IsActive",
                table: "SubscriptionPlans",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_Name",
                table: "SubscriptionPlans",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_Price",
                table: "SubscriptionPlans",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_Message_User",
                table: "MessageReadReceipts",
                columns: new[] { "MessageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReadReceipts_User",
                table: "MessageReadReceipts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_Message_User_Reaction",
                table: "MessageReactions",
                columns: new[] { "MessageId", "UserId", "Reaction" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_User",
                table: "MessageReactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KYCVerifications_Status",
                table: "KYCVerifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KYCVerifications_SubmittedAt",
                table: "KYCVerifications",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_KYCVerifications_User_Status",
                table: "KYCVerifications",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_KYCDocuments_DocumentType",
                table: "KYCDocuments",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_KYCDocuments_KYCVerification",
                table: "KYCDocuments",
                column: "KYCVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_Conversation_User",
                table: "ConversationParticipants",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_Provider_TimeRange",
                table: "AvailabilitySlots",
                columns: new[] { "ServiceProviderId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_TimeRange_Available",
                table: "AvailabilitySlots",
                columns: new[] { "StartTime", "EndTime", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageTracking_FeatureType",
                table: "AIUsageTrackings",
                column: "FeatureType");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageTracking_Timestamp",
                table: "AIUsageTrackings",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AIUsageTracking_User_Timestamp",
                table: "AIUsageTrackings",
                columns: new[] { "UserId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AIContentModerations_Content",
                table: "AIContentModerations",
                columns: new[] { "ContentType", "ContentId" });

            migrationBuilder.CreateIndex(
                name: "IX_AIContentModerations_ProcessedAt",
                table: "AIContentModerations",
                column: "ProcessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AIContentModerations_Result",
                table: "AIContentModerations",
                column: "ModerationResult");

            migrationBuilder.CreateIndex(
                name: "IX_AIContentModerations_User",
                table: "AIContentModerations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AIContentModerations_Users_UserId",
                table: "AIContentModerations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AIUsageTrackings_Users_UserId",
                table: "AIUsageTrackings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySlots_ServiceProviders_ServiceProviderId",
                table: "AvailabilitySlots",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KYCDocuments_KYCVerifications_KYCVerificationId",
                table: "KYCDocuments",
                column: "KYCVerificationId",
                principalTable: "KYCVerifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_Messages_MessageId",
                table: "MessageReactions",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_Users_UserId",
                table: "MessageReactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadReceipts_Messages_MessageId",
                table: "MessageReadReceipts",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReadReceipts_Users_UserId",
                table: "MessageReadReceipts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoCallSessions_Conversations_ConversationId",
                table: "VideoCallSessions",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoCallSessions_Users_InitiatorId",
                table: "VideoCallSessions",
                column: "InitiatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIContentModerations_Users_UserId",
                table: "AIContentModerations");

            migrationBuilder.DropForeignKey(
                name: "FK_AIUsageTrackings_Users_UserId",
                table: "AIUsageTrackings");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySlots_ServiceProviders_ServiceProviderId",
                table: "AvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_KYCDocuments_KYCVerifications_KYCVerificationId",
                table: "KYCDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_Messages_MessageId",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_Users_UserId",
                table: "MessageReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadReceipts_Messages_MessageId",
                table: "MessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageReadReceipts_Users_UserId",
                table: "MessageReadReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoCallSessions_Conversations_ConversationId",
                table: "VideoCallSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoCallSessions_Users_InitiatorId",
                table: "VideoCallSessions");

            migrationBuilder.DropIndex(
                name: "IX_VideoCallSessions_Conversation_StartTime",
                table: "VideoCallSessions");

            migrationBuilder.DropIndex(
                name: "IX_VideoCallSessions_Initiator",
                table: "VideoCallSessions");

            migrationBuilder.DropIndex(
                name: "IX_VideoCallSessions_Status",
                table: "VideoCallSessions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_IsActive",
                table: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_Name",
                table: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_Price",
                table: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_MessageReadReceipts_Message_User",
                table: "MessageReadReceipts");

            migrationBuilder.DropIndex(
                name: "IX_MessageReadReceipts_User",
                table: "MessageReadReceipts");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_Message_User_Reaction",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_MessageReactions_User",
                table: "MessageReactions");

            migrationBuilder.DropIndex(
                name: "IX_KYCVerifications_Status",
                table: "KYCVerifications");

            migrationBuilder.DropIndex(
                name: "IX_KYCVerifications_SubmittedAt",
                table: "KYCVerifications");

            migrationBuilder.DropIndex(
                name: "IX_KYCVerifications_User_Status",
                table: "KYCVerifications");

            migrationBuilder.DropIndex(
                name: "IX_KYCDocuments_DocumentType",
                table: "KYCDocuments");

            migrationBuilder.DropIndex(
                name: "IX_KYCDocuments_KYCVerification",
                table: "KYCDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ConversationParticipants_Conversation_User",
                table: "ConversationParticipants");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_Provider_TimeRange",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_TimeRange_Available",
                table: "AvailabilitySlots");

            migrationBuilder.DropIndex(
                name: "IX_AIUsageTracking_FeatureType",
                table: "AIUsageTrackings");

            migrationBuilder.DropIndex(
                name: "IX_AIUsageTracking_Timestamp",
                table: "AIUsageTrackings");

            migrationBuilder.DropIndex(
                name: "IX_AIUsageTracking_User_Timestamp",
                table: "AIUsageTrackings");

            migrationBuilder.DropIndex(
                name: "IX_AIContentModerations_Content",
                table: "AIContentModerations");

            migrationBuilder.DropIndex(
                name: "IX_AIContentModerations_ProcessedAt",
                table: "AIContentModerations");

            migrationBuilder.DropIndex(
                name: "IX_AIContentModerations_Result",
                table: "AIContentModerations");

            migrationBuilder.DropIndex(
                name: "IX_AIContentModerations_User",
                table: "AIContentModerations");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "VideoCallSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AIContentModerations");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationParticipants_User",
                table: "ConversationParticipants",
                newName: "IX_ConversationParticipants_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "StartTime",
                table: "VideoCallSessions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "InitiatorId",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ConversationId",
                table: "VideoCallSessions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "SubscriptionPlans",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "SubscriptionPlans",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Features",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SubscriptionPlans",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "BillingCycle",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Monthly");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MessageReadReceipts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReadAt",
                table: "MessageReadReceipts",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "MessageId",
                table: "MessageReadReceipts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "MessageReactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Reaction",
                table: "MessageReactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MessageId",
                table: "MessageReactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "MessageReactions",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "VerifiedBy",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VerificationResult",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SubmittedAt",
                table: "KYCVerifications",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentImageUrl",
                table: "KYCVerifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UploadedAt",
                table: "KYCDocuments",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "KYCVerificationId",
                table: "KYCDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "KYCDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentUrl",
                table: "KYCDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "DocumentType",
                table: "KYCDocuments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "JoinedAt",
                table: "ConversationParticipants",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceProviderId",
                table: "AvailabilitySlots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "RecurrenceRule",
                table: "AvailabilitySlots",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "AvailabilitySlots",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AIUsageTrackings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "AIUsageTrackings",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "AIUsageTrackings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "AIUsageTrackings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FeatureType",
                table: "AIUsageTrackings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "AIUsageTrackings",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldDefaultValue: 0.0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "ToxicityScore",
                table: "AIContentModerations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,4)",
                oldPrecision: 5,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ProcessedAt",
                table: "AIContentModerations",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "ModerationResult",
                table: "AIContentModerations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ModelUsed",
                table: "AIContentModerations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "gemini-1.5-flash");

            migrationBuilder.AlterColumn<string>(
                name: "Flags",
                table: "AIContentModerations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "AIContentModerations",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,6)",
                oldPrecision: 10,
                oldScale: 6,
                oldDefaultValue: 0.0m);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "AIContentModerations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ContentId",
                table: "AIContentModerations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.CreateIndex(
                name: "IX_KYCVerifications_UserId",
                table: "KYCVerifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationParticipants_ConversationId",
                table: "ConversationParticipants",
                column: "ConversationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationParticipants_Users_UserId",
                table: "ConversationParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "UserSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
