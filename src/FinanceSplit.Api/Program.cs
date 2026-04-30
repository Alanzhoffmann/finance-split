using FinanceSplit.Api.Endpoints;
using FinanceSplit.Api.Middleware;
using FinanceSplit.Application;
using FinanceSplit.Data;
using FinanceSplit.Web.Components;

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

app.UseMiddleware<MigrationMiddleware>();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapPersonEndpoints();
app.MapTransactionEndpoints();
app.MapExpenseEndpoints();
app.MapImportEndpoints();

app.MapRazorComponents<App>();

app.Run();

public partial class Program;
