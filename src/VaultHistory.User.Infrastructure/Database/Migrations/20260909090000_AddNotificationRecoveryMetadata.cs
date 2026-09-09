using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultHistory.User.Infrastructure.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260909090000_AddNotificationRecoveryMetadata")]
    public partial class AddNotificationRecoveryMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in new[] { "users", "outbox_messages" })
            {
                migrationBuilder.AddColumn<int>(name: "notificationAttemptCount", table: table, type: "integer", nullable: false, defaultValue: 0);
                migrationBuilder.AddColumn<DateTime>(name: "notificationProcessingStartedAt", table: table, type: "timestamp(3) with time zone", nullable: true);
                migrationBuilder.AddColumn<DateTime>(name: "notificationNextRetryAt", table: table, type: "timestamp(3) with time zone", nullable: true);
                migrationBuilder.AddColumn<string>(name: "notificationFailureStage", table: table, type: "text", nullable: true);
                migrationBuilder.AddColumn<string>(name: "notificationFailureReason", table: table, type: "text", nullable: true);
            }

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_notification_recovery",
                table: "outbox_messages",
                columns: new[] { "status", "notificationNextRetryAt", "notificationProcessingStartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_users_notification_recovery",
                table: "users",
                columns: new[] { "notificationStatus", "notificationNextRetryAt", "notificationProcessingStartedAt" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_outbox_messages_notification_recovery", table: "outbox_messages");
            migrationBuilder.DropIndex(name: "IX_users_notification_recovery", table: "users");

            foreach (var table in new[] { "users", "outbox_messages" })
            {
                migrationBuilder.DropColumn(name: "notificationAttemptCount", table: table);
                migrationBuilder.DropColumn(name: "notificationProcessingStartedAt", table: table);
                migrationBuilder.DropColumn(name: "notificationNextRetryAt", table: table);
                migrationBuilder.DropColumn(name: "notificationFailureStage", table: table);
                migrationBuilder.DropColumn(name: "notificationFailureReason", table: table);
            }
        }
    }
}
