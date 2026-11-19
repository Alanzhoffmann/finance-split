using FinanceSplit.Contracts.Transactions.Commands;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Dialogs;

public partial class TransactionRecurrenceDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private IReadOnlyList<(UpdateRecurrence Value, string Name)> Options { get; } =
        Enum.GetValues<UpdateRecurrence>()
            .Select(u => (u, u.ToStringFast(useMetadataAttributes: true)))
            .ToList();

    private UpdateRecurrence Value { get; set; } = UpdateRecurrence.Transaction;

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        MudDialog.Close(DialogResult.Ok(Value));
    }
}
