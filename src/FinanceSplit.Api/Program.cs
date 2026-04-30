using FinanceSplit.Api.Components;
using FinanceSplit.Api.Endpoints;
using FinanceSplit.Api.Middleware;
using FinanceSplit.Application;
using FinanceSplit.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=financesplit.db";

builder.Services.AddDataServices(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddRazorComponents().AddInteractiveWebAssemblyComponents();

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

app.MapRazorComponents<App>().AddInteractiveWebAssemblyRenderMode().AddAdditionalAssemblies(typeof(FinanceSplit.Web.Components.Routes).Assembly);

app.Run();

public partial class Program;
