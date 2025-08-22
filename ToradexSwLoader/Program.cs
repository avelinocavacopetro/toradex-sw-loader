using CurrieTechnologies.Razor.SweetAlert2;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor;
using System.Reflection;
using ToradexSwLoader.Components;
using ToradexSwLoader.Data;
using ToradexSwLoader.Services;

var builder = WebApplication.CreateBuilder(args);

var logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
Directory.CreateDirectory(logDirectory);

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("Log4NetConfig.xml"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySQL(connectionString)
);

// Configurar serviços a serem utilizados
builder.Services.AddHttpClient<TorizonService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<FleetService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<FilterService>();
builder.Services.AddSingleton<FinalProductStateService>();
builder.Services.AddHostedService<DeviceStatusUpdaterService>();
builder.Services.AddSweetAlert2();
builder.Services.AddBlazorBootstrap();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddLocalization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

string[] supportedCultures = ["pt-PT", "en-US", "es-ES"];

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("pt-PT")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
