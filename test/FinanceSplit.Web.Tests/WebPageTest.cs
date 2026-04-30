using FinanceSplit.Application;
using FinanceSplit.Data;
using FinanceSplit.Data.Interfaces;
using FinanceSplit.Data.Repositories;
using FinanceSplit.Domain.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using TUnit.Core;
using TUnit.Playwright;

namespace FinanceSplit.Web.Tests;

[Category("E2E")]
public abstract class WebPageTest : PageTest, IAsyncDisposable
{
    protected string BaseUrl { get; private set; } = null!;
    private WebApplication _app = null!;

    [Before(HookType.Test)]
    public async Task SetupWebApp()
    {
        var dbName = $"WebTest_{Guid.NewGuid()}";

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions { ApplicationName = typeof(Program).Assembly.GetName().Name!, EnvironmentName = "Development" }
        );

        builder.Services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase(dbName));
        builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddApplicationServices();
        builder.Services.AddSingleton<IMigrationState>(new TestMigrationState());
        builder.Services.AddRazorComponents();

        _app = builder.Build();
        _app.Urls.Add("http://127.0.0.1:0");

        _app.UseAntiforgery();
        _app.MapStaticAssets();
        _app.MapRazorComponents<FinanceSplit.Web.Components.App>();

        await _app.StartAsync();
        BaseUrl = _app.Urls.First();

        using var scope = _app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceSplitDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    protected async Task NavigateToAsync(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private class TestMigrationState : IMigrationState
    {
        public bool IsDone { get; set; } = true;
    }
}
