using FinanceSplit.Domain.Entities;

namespace FinanceSplit.Domain.ValueObjects;

public record PersonSummary
{
    public required Person Person { get; init; }
    public decimal Income { get; init; }
    public decimal SharedExpenses { get; init; }
    public decimal PersonalExpenses { get; init; }
    public decimal TotalPaid { get; init; }
}
