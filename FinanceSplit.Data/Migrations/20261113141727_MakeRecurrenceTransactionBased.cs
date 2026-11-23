using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261113141727_MakeRecurrenceTransactionBased : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Transactions_Recurrence_RecurrenceId",
            table: "Transactions");

        migrationBuilder.DropTable(
            name: "Recurrence");

        migrationBuilder.AddColumn<string>(
            name: "Discriminator",
            table: "Transactions",
            type: "TEXT",
            maxLength: 13,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "Ended",
            table: "Transactions",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.AddColumn<DateOnly>(
            name: "LastMonth",
            table: "Transactions",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "NumberOfRecurrences",
            table: "Transactions",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Type",
            table: "Transactions",
            type: "INTEGER",
            nullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Transactions_Transactions_RecurrenceId",
            table: "Transactions",
            column: "RecurrenceId",
            principalTable: "Transactions",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Transactions_Transactions_RecurrenceId",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "Discriminator",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "Ended",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "LastMonth",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "NumberOfRecurrences",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "Type",
            table: "Transactions");

        migrationBuilder.CreateTable(
            name: "Recurrence",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                Ended = table.Column<bool>(type: "INTEGER", nullable: false),
                LastMonth = table.Column<DateOnly>(type: "TEXT", nullable: true),
                NumberOfRecurrences = table.Column<int>(type: "INTEGER", nullable: true),
                Type = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Recurrence", x => x.Id);
            });

        migrationBuilder.AddForeignKey(
            name: "FK_Transactions_Recurrence_RecurrenceId",
            table: "Transactions",
            column: "RecurrenceId",
            principalTable: "Recurrence",
            principalColumn: "Id");
    }
}
