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

builder.Services.AddScoped<IGenericData<FormModule>>(sp => sp.GetRequiredService<FormModuleData>());
builder.Services.AddScoped<IGenericBusiness<FormModuleDto, FormModule>>(sp => sp.GetRequiredService<FormModuleBusiness>());

builder.Services.AddScoped<IGenericData<Login>>(sp => sp.GetRequiredService<LoginData>());
builder.Services.AddScoped<IGenericBusiness<LoginDto, Login>>(sp => sp.GetRequiredService<LoginBusiness>());

builder.Services.AddScoped<IGenericData<Module>>(sp => sp.GetRequiredService<ModuleData>());
builder.Services.AddScoped<IGenericBusiness<ModuleDto, Module>>(sp => sp.GetRequiredService<ModuleBusiness>());

builder.Services.AddScoped<IGenericData<Permission>>(sp => sp.GetRequiredService<PermissionData>());
builder.Services.AddScoped<IGenericBusiness<PermissionDto, Permission>>(sp => sp.GetRequiredService<PermissionBusiness>());

builder.Services.AddScoped<IGenericData<RolFormPermission>>(sp => sp.GetRequiredService<RolFormPermissionData>());
builder.Services.AddScoped<IGenericBusiness<RolFormPermissionDto, RolFormPermission>>(sp => sp.GetRequiredService<RolFormPermissionBusiness>());

builder.Services.AddScoped<IGenericData<RolUser>>(sp => sp.GetRequiredService<RolUserData>());
builder.Services.AddScoped<IGenericBusiness<RolUserDto, RolUser>>(sp => sp.GetRequiredService<RolUserBusiness>());

builder.Services.AddScoped<IGenericData<User>>(sp => sp.GetRequiredService<UserData>());
builder.Services.AddScoped<IGenericBusiness<UserDto, User>>(sp => sp.GetRequiredService<UserBusiness>());

builder.Services.AddScoped<IGenericData<Worker>>(sp => sp.GetRequiredService<WorkerData>());
builder.Services.AddScoped<IGenericBusiness<WorkerDto, Worker>>(sp => sp.GetRequiredService<WorkerBusiness>());

builder.Services.AddScoped<IGenericData<WorkerLogin>>(sp => sp.GetRequiredService<WorkerLoginData>());
builder.Services.AddScoped<IGenericBusiness<WorkerLoginDto, WorkerLogin>>(sp => sp.GetRequiredService<WorkerLoginBusiness>());

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