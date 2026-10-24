using FinanceSplit.Contracts.DTOs;
using FinanceSplit.Contracts.Enums;
using FinanceSplit.Contracts.Services;

namespace FinanceSplit.Api;

internal class MockTransactionService : ITransactionService
{
    public async Task<IReadOnlyList<TransactionResponse>?> GetTransactionsAsync(
        DateTime? month = null,
        CancellationToken cancellationToken = default
    ) =>
        [
            new TransactionResponse(
                Guid.NewGuid(),
                "Internet",
                42,
                new DateTime(2026, 10, 24),
                SplitType.Ratio,
                new(Guid.NewGuid(), "name 1"),
                true,
                [new(Guid.NewGuid(), "name 1"), new(Guid.NewGuid(), "name 2")]
            ),
            new TransactionResponse(
                Guid.NewGuid(),
                "Furniture",
                500,
                new DateTime(2026, 10, 15),
                SplitType.Even,
                new(Guid.NewGuid(), "name 1"),
                false,
                [new(Guid.NewGuid(), "name 1"), new(Guid.NewGuid(), "name 2")]
            ),
            new TransactionResponse(
                Guid.NewGuid(),
                "Books",
                20,
                new DateTime(2026, 10, 8),
                SplitType.None,
                new(Guid.NewGuid(), "name 1"),
                false,
                []
            ),
        ];
}
