using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261101192611_AddRecurrence : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsRecurring",
            table: "Transactions");

        migrationBuilder.AddColumn<Guid>(
            name: "RecurrenceId",
            table: "Transactions",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Recurrence",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Type = table.Column<int>(type: "INTEGER", nullable: false),
                Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                NumberOfRecurrences = table.Column<int>(type: "INTEGER", nullable: true),
                LastMonth = table.Column<DateOnly>(type: "TEXT", nullable: true),
                Ended = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Recurrence", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_RecurrenceId",
            table: "Transactions",
            column: "RecurrenceId");

        migrationBuilder.AddForeignKey(
            name: "FK_Transactions_Recurrence_RecurrenceId",
            table: "Transactions",
            column: "RecurrenceId",
            principalTable: "Recurrence",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Transactions_Recurrence_RecurrenceId",
            table: "Transactions");

        migrationBuilder.DropTable(
            name: "Recurrence");

        migrationBuilder.DropIndex(
            name: "IX_Transactions_RecurrenceId",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "RecurrenceId",
            table: "Transactions");

        migrationBuilder.AddColumn<bool>(
            name: "IsRecurring",
            table: "Transactions",
            type: "INTEGER",
            nullable: false,
            defaultValue: false);
    }
}
