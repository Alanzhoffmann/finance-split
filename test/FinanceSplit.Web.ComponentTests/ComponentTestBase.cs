using Bunit;
using FinanceSplit.Contracts.Responses;
using FinanceSplit.Web.Services;
using Microsoft.Extensions.DependencyInjection;
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
        MockApi = new MockApiClient();
        Ctx.Services.AddSingleton<ApiClient>(MockApi);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Ctx?.Dispose();
        GC.SuppressFinalize(this);
    }
}
