using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

internal sealed class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(g => g.Id);

        builder
            .HasMany(g => g.Transactions)
            .WithOne(t => t.Goal)
            .OnDelete(DeleteBehavior.ClientSetNull);
        builder.HasMany(g => g.SharedWith).WithMany();

        builder.Navigation(g => g.Transactions).AutoInclude();
        builder.Navigation(g => g.SharedWith).AutoInclude();
    }
}
