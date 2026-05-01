using FinanceSplit.Data;
using FinanceSplit.Data.BackgroundServices;
using FinanceSplit.Data.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using TUnit.AspNetCore;

namespace FinanceSplit.Web.Tests;

public class WebTestFactory : TestWebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var dbDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<FinanceSplitDbContext>)
                    || d.ServiceType == typeof(FinanceSplitDbContext)
                    || d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true
                )
                .ToList();
            foreach (var d in dbDescriptors)
            {
                services.Remove(d);
            }

            // Add a default InMemory DB so service validation passes
            services.AddDbContext<FinanceSplitDbContext>(options => options.UseInMemoryDatabase("WebTestDefault"));

            // Remove migration background service
            var migrationDescriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(MigrationBackgroundService));
            if (migrationDescriptor is not null)
            {
                services.Remove(migrationDescriptor);
            }

            // Mark migrations as done
            var stateDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMigrationState));
            if (stateDescriptor is not null)
            {
                services.Remove(stateDescriptor);
            }
            services.AddSingleton<IMigrationState>(new TestMigrationState());

            // Ensure MudBlazor services are registered (required for MudPopoverProvider)
            services.AddMudServices();
            services.AddMudPopoverService();
        });
    }

    private class TestMigrationState : IMigrationState
    {
        public bool IsDone { get; set; } = true;
    }
}
