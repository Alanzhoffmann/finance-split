using FinanceSplit.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using TUnit.AspNetCore;
using TUnit.Core;
using TUnit.Playwright;

namespace FinanceSplit.Web.Tests;

public abstract class WebPageTest : PageTest, IAsyncDisposable
{
    protected WebTestFactory Factory { get; private set; } = null!;
    protected string BaseUrl { get; private set; } = null!;

    [Before(HookType.Test)]
    public async Task SetupWebApp()
    {
        var dbName = $"WebTest_{Guid.NewGuid()}";
        Factory = new WebTestFactory();

        Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase(dbName));
            });
        });

        var client = Factory.CreateClient();
        BaseUrl = client.BaseAddress!.ToString().TrimEnd('/');

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceSplitDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    protected async Task NavigateToAsync(string path)
    {
        await Page.GotoAsync($"{BaseUrl}{path}");
    }

    public async ValueTask DisposeAsync()
    {
        await Factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
