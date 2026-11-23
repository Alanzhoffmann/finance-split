using FinanceSplit.Client.Clients;
using FinanceSplit.Client.Extensions;
using FinanceSplit.Contracts.Dashboard.Services;
using FinanceSplit.Contracts.FileImport.Services;
using FinanceSplit.Contracts.People.Services;
using FinanceSplit.Contracts.Transactions.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder
    .Services.AddHttpClient(Options.DefaultName)
    .ConfigureHttpClient(client =>
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    );

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IFileImportService, FileImportService>();

builder.Services.AddClientDependencies();

await builder.Build().RunAsync();
