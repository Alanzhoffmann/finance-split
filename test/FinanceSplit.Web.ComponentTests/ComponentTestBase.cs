using Bunit;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using TUnit.Core;
using BunitTestContext = Bunit.TestContext;

namespace FinanceSplit.Web.ComponentTests;

public abstract class ComponentTestBase : IDisposable
{
    protected BunitTestContext Ctx { get; private set; } = null!;
    protected MockApiClient MockApi { get; private set; } = null!;

    [Before(HookType.Test)]
    public Task Setup()
    {
        Ctx = new BunitTestContext();
        Ctx.Services.AddMudServices();
        MockApi = new MockApiClient();
        Ctx.Services.AddSingleton<ApiClient>(MockApi);
        Ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return Task.CompletedTask;
    }

    protected IRenderedComponent<TComponent> RenderWithProviders<TComponent>()
        where TComponent : IComponent
    {
        var cut = Ctx.Render<MudPopoverProvider>();
        return Ctx.Render<TComponent>();
    }

    public void Dispose()
    {
        Ctx?.Dispose();
        GC.SuppressFinalize(this);
    }
}
