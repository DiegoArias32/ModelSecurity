using Business;
using Data;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
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

// Agregar CORS
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
});

// 🔹 Agregar el contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Registrar implementaciones específicas
// Implementaciones regulares existentes
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

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

builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolUserBusiness>();

builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();

builder.Services.AddScoped<LoginData>();
builder.Services.AddScoped<LoginBusiness>();

builder.Services.AddScoped<MenuData>();
builder.Services.AddScoped<MenuBusiness>();

builder.Services.AddScoped<WorkerLoginData>();
builder.Services.AddScoped<WorkerLoginBusiness>();

builder.Services.AddScoped<ActivityLogData>();
builder.Services.AddScoped<ActivityLogBusiness>();

// 🔹 Registrar implementaciones genéricas para cada entidad
// Form
builder.Services.AddScoped<IGenericData<Form>, FormData>();
builder.Services.AddScoped<IGenericBusiness<FormDto, Form>, FormBusiness>();


// Agregar el HttpContextAccessor para obtener la IP del cliente
builder.Services.AddHttpContextAccessor();

// 🔹 Logging (opcional si lo vas a usar)
builder.Services.AddLogging();

// 🔹 Agregar controladores
builder.Services.AddControllers().AddNewtonsoftJson();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5163); // Habilita escucha en todas las IPs en el puerto 5163
});

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