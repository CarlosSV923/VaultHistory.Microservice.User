using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultHistory.User.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlignSharedJobsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "outbox_messages");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updatedAt");

            migrationBuilder.RenameColumn(
                name: "PasswordSalt",
                table: "users",
                newName: "passwordSalt");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "passwordHash");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "users",
                newName: "birthDate");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "users",
                newName: "fullname");

            migrationBuilder.Sql("UPDATE users SET fullname = \"FirstName\" || ' ' || fullname;");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "users");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "outbox_messages",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "outbox_messages",
                newName: "payload");

            migrationBuilder.RenameColumn(
                name: "OccurredOn",
                table: "outbox_messages",
                newName: "occurredOn");

            migrationBuilder.RenameColumn(
                name: "Error",
                table: "outbox_messages",
                newName: "error");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "outbox_messages",
                newName: "id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedAt",
                table: "users",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "isActive",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createdAt",
                table: "users",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "birthDate",
                table: "users",
                type: "timestamp(3) with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "character",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notification",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "notificationDate",
                table: "users",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notificationStatus",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "theme",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "outbox_messages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "occurredOn",
                table: "outbox_messages",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "outbox_messages",
                type: "text",
                nullable: true,
                defaultValue: "PENDING");

            migrationBuilder.AddColumn<DateTime>(
                name: "updateAt",
                table: "outbox_messages",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE outbox_messages
                SET status = CASE WHEN "Processed" THEN 'PROCESSED' ELSE 'PENDING' END,
                    "updateAt" = "ProcessedOn";
                """);

            migrationBuilder.DropColumn(
                name: "Processed",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "ProcessedOn",
                table: "outbox_messages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_messages",
                table: "outbox_messages",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_messages",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "character",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notificationDate",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notificationStatus",
                table: "users");

            migrationBuilder.DropColumn(
                name: "theme",
                table: "users");

            migrationBuilder.DropColumn(
                name: "status",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "updateAt",
                table: "outbox_messages");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "outbox_messages",
                newName: "OutboxMessages");

            migrationBuilder.RenameColumn(
                name: "updatedAt",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "passwordSalt",
                table: "Users",
                newName: "PasswordSalt");

            migrationBuilder.RenameColumn(
                name: "passwordHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "birthDate",
                table: "Users",
                newName: "BirthDate");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "fullname",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "OutboxMessages",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "payload",
                table: "OutboxMessages",
                newName: "Payload");

            migrationBuilder.RenameColumn(
                name: "occurredOn",
                table: "OutboxMessages",
                newName: "OccurredOn");

            migrationBuilder.RenameColumn(
                name: "error",
                table: "OutboxMessages",
                newName: "Error");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OutboxMessages",
                newName: "Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldPrecision: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldPrecision: 3);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                table: "Users",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OutboxMessages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "OccurredOn",
                table: "OutboxMessages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldPrecision: 3);

            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "OutboxMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedOn",
                table: "OutboxMessages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "Id");
        }
    }
}
