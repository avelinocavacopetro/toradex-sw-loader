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
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)
));

// Configurar serviços a serem utilizados
builder.Services.AddHttpClient<TorizonService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<PackageService>();
builder.Services.AddSweetAlert2();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
