using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261109195843_AddMonthSummary : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Date",
            table: "Transactions",
            newName: "Month");

        migrationBuilder.CreateTable(
            name: "Summaries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Month = table.Column<DateOnly>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Summaries", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ExpenseSettlement",
            columns: table => new
            {
                MonthSummaryId = table.Column<Guid>(type: "TEXT", nullable: false),
                FromId = table.Column<Guid>(type: "TEXT", nullable: false),
                ToId = table.Column<Guid>(type: "TEXT", nullable: false),
                Amount = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ExpenseSettlement", x => new { x.MonthSummaryId, x.FromId, x.ToId });
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
                    name: "FK_ExpenseSettlement_Summaries_MonthSummaryId",
                    column: x => x.MonthSummaryId,
                    principalTable: "Summaries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PersonSummary",
            columns: table => new
            {
                MonthSummaryId = table.Column<Guid>(type: "TEXT", nullable: false),
                PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                Income = table.Column<decimal>(type: "TEXT", nullable: false),
                SharedExpenses = table.Column<decimal>(type: "TEXT", nullable: false),
                PersonalExpenses = table.Column<decimal>(type: "TEXT", nullable: false),
                TotalPaid = table.Column<decimal>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PersonSummary", x => new { x.MonthSummaryId, x.PersonId });
                table.ForeignKey(
                    name: "FK_PersonSummary_People_PersonId",
                    column: x => x.PersonId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PersonSummary_Summaries_MonthSummaryId",
                    column: x => x.MonthSummaryId,
                    principalTable: "Summaries",
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
            name: "IX_PersonSummary_PersonId",
            table: "PersonSummary",
            column: "PersonId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ExpenseSettlement");

        migrationBuilder.DropTable(
            name: "PersonSummary");

        migrationBuilder.DropTable(
            name: "Summaries");

        migrationBuilder.RenameColumn(
            name: "Month",
            table: "Transactions",
            newName: "Date");
    }
}
