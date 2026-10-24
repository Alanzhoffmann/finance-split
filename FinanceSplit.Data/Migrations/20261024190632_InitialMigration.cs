using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261024190632_InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "People",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_People", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IncomeRecords",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Month = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                Type = table.Column<int>(type: "INTEGER", nullable: false),
                PersonId = table.Column<Guid>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IncomeRecords", x => x.Id);
                table.ForeignKey(
                    name: "FK_IncomeRecords_People_PersonId",
                    column: x => x.PersonId,
                    principalTable: "People",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Transactions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                Value = table.Column<decimal>(type: "TEXT", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                SplitType = table.Column<int>(type: "INTEGER", nullable: false),
                IsRecurring = table.Column<bool>(type: "INTEGER", nullable: false),
                PaidById = table.Column<Guid>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Transactions_People_PaidById",
                    column: x => x.PaidById,
                    principalTable: "People",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "PersonTransaction",
            columns: table => new
            {
                SharedTransactionsId = table.Column<Guid>(type: "TEXT", nullable: false),
                SharedWithId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersonTransaction", x => new { x.SharedTransactionsId, x.SharedWithId });
                table.ForeignKey(
                    name: "FK_PersonTransaction_People_SharedWithId",
                    column: x => x.SharedWithId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PersonTransaction_Transactions_SharedTransactionsId",
                    column: x => x.SharedTransactionsId,
                    principalTable: "Transactions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_IncomeRecords_PersonId",
            table: "IncomeRecords",
            column: "PersonId");

        migrationBuilder.CreateIndex(
            name: "IX_PersonTransaction_SharedWithId",
            table: "PersonTransaction",
            column: "SharedWithId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_PaidById",
            table: "Transactions",
            column: "PaidById");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "IncomeRecords");

        migrationBuilder.DropTable(
            name: "PersonTransaction");

        migrationBuilder.DropTable(
            name: "Transactions");

        migrationBuilder.DropTable(
            name: "People");
    }
}
