using Business;
using Data;
using Entity.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Agregar el contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar los servicios de negocio y de datos
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<ClientData>();
builder.Services.AddScoped<ClientBusiness>();

// 🔹 Registrar FormData y FormBusiness para evitar el error
builder.Services.AddScoped<FormData>();  
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

builder.Services.AddScoped<ModuleData>(); // Repositorio de datos
builder.Services.AddScoped<ModuleBusiness>(); // Lógica de negocio

builder.Services.AddScoped<UserData>(); // Repositorio de datos
builder.Services.AddScoped<UserBusiness>(); // Lógica de negocio

// Registrar el servicio de logging
builder.Services.AddLogging();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
