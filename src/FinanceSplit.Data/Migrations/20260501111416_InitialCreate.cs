using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceSplit.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20260501111416_InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => new { x.PersonId, x.Id });
                    table.ForeignKey(
                        name: "FK_Salaries_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PaidById = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    RecurrenceStartMonth = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    RecurrenceTerminationCount = table.Column<int>(type: "INTEGER", nullable: true),
                    RecurrenceTerminationEndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    RecurrenceTerminationType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    SplitType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_People_PaidById",
                        column: x => x.PaidById,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "TransactionParticipants",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<Guid>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionParticipants", x => new { x.PersonId, x.TransactionId });
                    table.ForeignKey(
                        name: "FK_TransactionParticipants_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_TransactionParticipants_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(name: "IX_TransactionParticipants_TransactionId", table: "TransactionParticipants", column: "TransactionId");

            migrationBuilder.CreateIndex(name: "IX_Transactions_PaidById", table: "Transactions", column: "PaidById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Salaries");

            migrationBuilder.DropTable(name: "TransactionParticipants");

            migrationBuilder.DropTable(name: "Transactions");

            migrationBuilder.DropTable(name: "People");
        }
    }
}
