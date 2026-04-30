using FinanceSplit.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.AspNetCore;

namespace FinanceSplit.Api.Tests;

public class FinanceSplitTestFactory : TestWebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove all EF Core registrations
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
        });
    }
}
