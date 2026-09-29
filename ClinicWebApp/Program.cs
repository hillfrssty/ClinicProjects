using ClinicWebApp.Components;
using ClinicWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<HospitalizationService>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<BedService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();