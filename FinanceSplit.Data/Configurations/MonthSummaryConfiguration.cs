using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

internal class MonthSummaryConfiguration : IEntityTypeConfiguration<MonthSummary>
{
    private const string _monthSummaryMonth = $"{nameof(MonthSummary)}{nameof(MonthSummary.Month)}";

    public void Configure(EntityTypeBuilder<MonthSummary> builder)
    {
        builder.HasKey(m => m.Month);

        builder.OwnsMany(
            m => m.People,
            b =>
            {
                const string personId =
                    $"{nameof(PersonSummary.Person)}{nameof(PersonSummary.Person.Id)}";

                b.Property<Guid>(personId);

                b.HasOne(p => p.Person).WithMany().HasForeignKey(personId);

                b.HasKey(_monthSummaryMonth, personId);
            }
        );

        builder.OwnsMany(
            m => m.Settlements,
            b =>
            {
                const string fromId =
                    $"{nameof(ExpenseSettlement.From)}{nameof(ExpenseSettlement.From.Id)}";
                const string toId =
                    $"{nameof(ExpenseSettlement.To)}{nameof(ExpenseSettlement.To.Id)}";

                b.Property<Guid>(fromId);
                b.Property<Guid>(toId);

                b.Property(s => s.Amount);

                b.HasOne(s => s.From).WithMany().HasForeignKey(fromId);
                b.HasOne(s => s.To).WithMany().HasForeignKey(toId);

                b.HasKey(_monthSummaryMonth, fromId, toId);
            }
        );
    }
}
