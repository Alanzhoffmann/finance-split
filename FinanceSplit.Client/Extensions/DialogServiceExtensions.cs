using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FinanceSplit.Client.Extensions;

public static class DialogServiceExtensions
{
    extension(IDialogService dialogService)
    {
        public async Task<T?> ShowDialogWithResultAsync<T, TDialog>(
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

        public async Task<bool> ShowGenericDeleteWarning()
        {
            bool? result = await dialogService.ShowMessageBoxAsync(
                "Warning",
                "Deleting can not be undone!",
                yesText: "Delete!",
                cancelText: "Cancel"
            );

            return result == true;
        }
    }
}
