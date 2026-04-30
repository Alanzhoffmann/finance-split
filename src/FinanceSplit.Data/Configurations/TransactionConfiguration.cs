using FinanceSplit.Contracts.Enums;
using FinanceSplit.Domain.Entities;
using FinanceSplit.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceSplit.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).HasMaxLength(500).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Date).IsRequired();

        builder.HasOne(t => t.PaidBy).WithMany().HasForeignKey("PaidById").IsRequired();

        // Participants: many-to-many via join table
        builder
            .HasMany(t => t.Participants)
            .WithMany()
            .UsingEntity(
                "TransactionParticipants",
                j => j.HasOne(typeof(Person)).WithMany().HasForeignKey("PersonId"),
                j => j.HasOne(typeof(Transaction)).WithMany().HasForeignKey("TransactionId")
            );
        builder.Navigation(t => t.Participants).UsePropertyAccessMode(PropertyAccessMode.Field);

        // SplitPay: store SplitType as a column, ignore People (loaded via Participants)
        builder.OwnsOne(
            t => t.SplitPay,
            splitPay =>
            {
                splitPay.Property(s => s.SplitType).HasConversion<string>().HasMaxLength(20).HasColumnName("SplitType").IsRequired();
                splitPay.Ignore(s => s.People);
            }
        );

        builder.OwnsOne(
            t => t.Recurrence,
            recurrence =>
            {
                recurrence.Property(r => r.StartMonth).HasColumnName("RecurrenceStartMonth");
                recurrence.Ignore(r => r.Termination);
                recurrence.Ignore(r => r.IsForever);
                recurrence.Ignore(r => r.EndMonth);
                recurrence.Ignore(r => r.OccurrenceCount);

                // Store termination as flat columns
                recurrence.Property<string>("TerminationType").HasMaxLength(20).HasColumnName("RecurrenceTerminationType");
                recurrence.Property<int?>("TerminationCount").HasColumnName("RecurrenceTerminationCount");
                recurrence.Property<DateOnly?>("TerminationEndDate").HasColumnName("RecurrenceTerminationEndDate");
            }
        );
    }
}
