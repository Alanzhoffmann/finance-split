using FinanceSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

internal sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name);

        builder.HasMany(p => p.IncomeRecords).WithOne();

        builder.HasMany(p => p.Transactions).WithOne(t => t.PaidBy);

        builder.HasMany(p => p.SharedTransactions).WithMany(t => t.SharedWith);

        builder.Navigation(p => p.IncomeRecords).AutoInclude();
    }
}
