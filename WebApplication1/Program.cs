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

// 🔹 Agregar servicios de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Configurar CORS
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(OrigenesPermitidos)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 🔹 Agregar el contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Registrar servicios de negocio y datos
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<ClientData>();
builder.Services.AddScoped<ClientBusiness>();

builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();

builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermissionBusiness>();

builder.Services.AddScoped<WorkerData>();
builder.Services.AddScoped<WorkerBusiness>();

builder.Services.AddScoped<PqrData>();
builder.Services.AddScoped<PqrBusiness>();


// 🔹 Logging (opcional si lo vas a usar)
builder.Services.AddLogging();

// 🔹 Agregar controladores
builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Swagger solo en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 Activar CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
