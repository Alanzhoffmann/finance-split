using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Ignore(p => p.Salaries);

        builder.OwnsMany<Salary>(
            "_salaries",
            salary =>
            {
                salary.ToTable("Salaries");
                salary.Property<int>("Id").ValueGeneratedOnAdd();
                salary.Property(s => s.Date).IsRequired();
                salary.Property(s => s.Amount).HasPrecision(18, 2).IsRequired();
                salary.WithOwner().HasForeignKey("PersonId");
            }
        );

        builder.Navigation("_salaries").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
