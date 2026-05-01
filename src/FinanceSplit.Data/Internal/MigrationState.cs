using FinanceSplit.Data.Interfaces;

namespace FinanceSplit.Data.Internal;

internal class MigrationState : IMigrationState
{
    public bool IsDone { get; set; }
}
