using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261111153041_InitialMigration : Migration
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

        migrationBuilder.CreateTable(
            name: "Summaries",
            columns: table => new
            {
                Month = table.Column<DateOnly>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Summaries", x => x.Month);
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
                Month = table.Column<DateOnly>(type: "TEXT", nullable: false),
                SplitType = table.Column<int>(type: "INTEGER", nullable: false),
                PaidById = table.Column<Guid>(type: "TEXT", nullable: true),
                RecurrenceId = table.Column<Guid>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Transactions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Transactions_People_PaidById",
                    column: x => x.PaidById,
                    principalTable: "People",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Transactions_Recurrence_RecurrenceId",
                    column: x => x.RecurrenceId,
                    principalTable: "Recurrence",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ExpenseSettlement",
            columns: table => new
            {
                MonthSummaryMonth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                FromId = table.Column<Guid>(type: "TEXT", nullable: false),
                ToId = table.Column<Guid>(type: "TEXT", nullable: false),
                Amount = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExpenseSettlement", x => new { x.MonthSummaryMonth, x.FromId, x.ToId });
                table.ForeignKey(
                    name: "FK_ExpenseSettlement_People_FromId",
                    column: x => x.FromId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ExpenseSettlement_People_ToId",
                    column: x => x.ToId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ExpenseSettlement_Summaries_MonthSummaryMonth",
                    column: x => x.MonthSummaryMonth,
                    principalTable: "Summaries",
                    principalColumn: "Month",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PersonSummary",
            columns: table => new
            {
                MonthSummaryMonth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                Income = table.Column<decimal>(type: "TEXT", nullable: false),
                SharedExpenses = table.Column<decimal>(type: "TEXT", nullable: false),
                PersonalExpenses = table.Column<decimal>(type: "TEXT", nullable: false),
                TotalPaid = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersonSummary", x => new { x.MonthSummaryMonth, x.PersonId });
                table.ForeignKey(
                    name: "FK_PersonSummary_People_PersonId",
                    column: x => x.PersonId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PersonSummary_Summaries_MonthSummaryMonth",
                    column: x => x.MonthSummaryMonth,
                    principalTable: "Summaries",
                    principalColumn: "Month",
                    onDelete: ReferentialAction.Cascade);
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
            name: "IX_ExpenseSettlement_FromId",
            table: "ExpenseSettlement",
            column: "FromId");

        migrationBuilder.CreateIndex(
            name: "IX_ExpenseSettlement_ToId",
            table: "ExpenseSettlement",
            column: "ToId");

        migrationBuilder.CreateIndex(
            name: "IX_IncomeRecords_PersonId",
            table: "IncomeRecords",
            column: "PersonId");

        migrationBuilder.CreateIndex(
            name: "IX_PersonSummary_PersonId",
            table: "PersonSummary",
            column: "PersonId");

        migrationBuilder.CreateIndex(
            name: "IX_PersonTransaction_SharedWithId",
            table: "PersonTransaction",
            column: "SharedWithId");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_PaidById",
            table: "Transactions",
            column: "PaidById");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_RecurrenceId",
            table: "Transactions",
            column: "RecurrenceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExpenseSettlement");

        migrationBuilder.DropTable(
            name: "IncomeRecords");

        migrationBuilder.DropTable(
            name: "PersonSummary");

        migrationBuilder.DropTable(
            name: "PersonTransaction");

        migrationBuilder.DropTable(
            name: "Summaries");

        migrationBuilder.DropTable(
            name: "Transactions");

        migrationBuilder.DropTable(
            name: "People");

        migrationBuilder.DropTable(
            name: "Recurrence");
    }
}
