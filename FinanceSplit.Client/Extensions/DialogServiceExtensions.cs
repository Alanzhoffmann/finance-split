using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Extensions;

public static class DialogServiceExtensions
{
    extension(IDialogService dialogService)
    {
        public async Task<T?> ShowDialogAsync<T, TDialog>(
            DialogParameters parameters,
            DialogOptions options
        )
            where TDialog : IComponent
        {
            var dialog = await dialogService.ShowAsync<TDialog>(parameters, options);
            var result = await dialog.Result;

            return result is not null && !result.Canceled && result.Data is T editedData
                ? editedData
                : default;
        }
    }
}
