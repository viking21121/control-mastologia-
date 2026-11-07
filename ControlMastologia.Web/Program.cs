using ControlMastologiaV2;
using Microsoft.EntityFrameworkCore;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// 1. YA NO NECESITAS ESTO AQUÍ porque lo manejas dentro de AppDbContext.cs
// builder.Services.AddDbContext<AppDbContext>(...); 

// 2. Configuración de Supabase (Mantenla igual)
var supabaseUrl = "https://mqfbqgrrvhmjctiueydy.supabase.co";
var supabaseKey = "sb_publishable_2PCNeKA-ugOtexSTGO1g1Q_B0OkPDTV";

builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions
    {
        AutoRefreshToken = true,
        AutoConnectRealtime = true
    }));

// Servicios de API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Pipeline de configuración
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();