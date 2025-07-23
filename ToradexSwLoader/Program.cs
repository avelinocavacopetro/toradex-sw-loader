using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.EntityFrameworkCore;
using ToradexSwLoader.Components;
using ToradexSwLoader.Data;
using ToradexSwLoader.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<PackageService>();
builder.Services.AddScoped<FleetService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<WindowService>();
builder.Services.AddScoped<FilterService>();
builder.Services.AddSingleton<FinalProductStateService>();
builder.Services.AddHostedService<DeviceStatusUpdaterService>();
builder.Services.AddSweetAlert2();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

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
