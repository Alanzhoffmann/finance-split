using FinanceSplit.Api.Components;
using FinanceSplit.Api.Endpoints;
using FinanceSplit.Api.Middleware;
using FinanceSplit.Application;
using FinanceSplit.Data;
using FinanceSplit.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=financesplit.db";

builder.Services.AddDataServices(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext?.Request;
    var baseAddress = request is not null ? $"{request.Scheme}://{request.Host}" : "http://localhost";
    return new HttpClient { BaseAddress = new Uri(baseAddress) };
});
builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    // app.UseHttpsRedirection();
}

app.UseMiddleware<MigrationMiddleware>();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapPersonEndpoints();
app.MapTransactionEndpoints();
app.MapExpenseEndpoints();
app.MapImportEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FinanceSplit.Web.Components.Routes).Assembly);

app.Run();

public partial class Program;
