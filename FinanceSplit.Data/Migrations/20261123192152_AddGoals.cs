using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations;

/// <inheritdoc />
public partial class _20261123192152_AddGoals : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "GoalId",
            table: "Transactions",
            type: "TEXT",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Goals",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", nullable: false),
                InitialMonth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                FinalMonth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Value = table.Column<decimal>(type: "TEXT", nullable: false),
                SplitType = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Goals", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GoalPerson",
            columns: table => new
            {
                GoalId = table.Column<Guid>(type: "TEXT", nullable: false),
                SharedWithId = table.Column<Guid>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GoalPerson", x => new { x.GoalId, x.SharedWithId });
                table.ForeignKey(
                    name: "FK_GoalPerson_Goals_GoalId",
                    column: x => x.GoalId,
                    principalTable: "Goals",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GoalPerson_People_SharedWithId",
                    column: x => x.SharedWithId,
                    principalTable: "People",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_GoalId",
            table: "Transactions",
            column: "GoalId");

        migrationBuilder.CreateIndex(
            name: "IX_GoalPerson_SharedWithId",
            table: "GoalPerson",
            column: "SharedWithId");

        migrationBuilder.AddForeignKey(
            name: "FK_Transactions_Goals_GoalId",
            table: "Transactions",
            column: "GoalId",
            principalTable: "Goals",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Transactions_Goals_GoalId",
            table: "Transactions");

        migrationBuilder.DropTable(
            name: "GoalPerson");

        migrationBuilder.DropTable(
            name: "Goals");

        migrationBuilder.DropIndex(
            name: "IX_Transactions_GoalId",
            table: "Transactions");

        migrationBuilder.DropColumn(
            name: "GoalId",
            table: "Transactions");
    }
}
