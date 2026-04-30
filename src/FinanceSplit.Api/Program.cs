using FinanceSplit.Api.Endpoints;
using FinanceSplit.Application;
using FinanceSplit.Data;
using FinanceSplit.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=financesplit.db";

builder.Services.AddDataServices(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddRazorComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseAntiforgery();
app.MapStaticAssets();

app.MapPersonEndpoints();
app.MapTransactionEndpoints();
app.MapExpenseEndpoints();
app.MapImportEndpoints();

app.MapRazorComponents<App>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanceSplitDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();

public partial class Program;
