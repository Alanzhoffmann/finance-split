using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

internal class RecurrenceConfiguration : IEntityTypeConfiguration<Recurrence>
{
    public void Configure(EntityTypeBuilder<Recurrence> builder)
    {
        builder.HasKey(r => r.Id);

        builder
            .HasMany(r => r.Transactions)
            .WithOne(t => t.Recurrence)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
