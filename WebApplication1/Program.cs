using AutoMapper;
using Business;
using Business.Services;
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
using Utilities.Factories;
using Utilities.Interfaces;
using Utilities.Mapping;
using Utilities.Repositories;
using Utilities.Services;
using Utilities.Strategies;
using Module = Entity.Model.Module;

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

// 🔹 Registrar AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<Utilities.Mapping.AutoMapperProfile>();
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// 🔹 Registrar UnitOfWork y Repository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 🔹 Registrar ServiceFactory
builder.Services.AddScoped<IServiceFactory, ServiceFactory>();

// 🔹 Registrar la clase base para Business
builder.Services.AddScoped(typeof(BaseBusiness<,>));

// 🔹 Registrar la clase base para Data
builder.Services.AddScoped(typeof(BaseData<>));

// 🔹 Registrar servicios genéricos
builder.Services.AddScoped(typeof(IService<,>), typeof(Service<,>));

// 🔹 Registrar servicios especializados
builder.Services.AddScoped<IService<Rol, RolDto>, RolService>();
builder.Services.AddScoped<IService<Form, FormDto>, FormService>();
builder.Services.AddScoped<IService<Module, ModuleDto>, ModuleService>();
builder.Services.AddScoped<IService<User, UserDto>, UserService>();
builder.Services.AddScoped<IService<Permission, PermissionDto>, PermissionService>();
builder.Services.AddScoped<IService<Worker, WorkerDto>, WorkerService>();
builder.Services.AddScoped<IService<RolUser, RolUserDto>, RolUserService>();
builder.Services.AddScoped<IService<RolFormPermission, RolFormPermissionDto>, RolFormPermissionService>();
builder.Services.AddScoped<IService<Login, LoginDto>, LoginService>();
builder.Services.AddScoped<IService<FormModule, FormModuleDto>, FormModuleService>();
builder.Services.AddScoped<IService<WorkerLogin, WorkerLoginDto>, WorkerLoginService>();
builder.Services.AddScoped<IService<ActivityLog, ActivityLogDto>, ActivityLogService>();

// 🔹 Registrar clases Business
builder.Services.AddScoped<RolBusiness>();
builder.Services.AddScoped<FormBusiness>();
builder.Services.AddScoped<FormModuleBusiness>();
builder.Services.AddScoped<ModuleBusiness>();
builder.Services.AddScoped<UserBusiness>();
builder.Services.AddScoped<PermissionBusiness>();
builder.Services.AddScoped<WorkerBusiness>();
builder.Services.AddScoped<RolUserBusiness>();
builder.Services.AddScoped<RolFormPermissionBusiness>();
builder.Services.AddScoped<LoginBusiness>();
builder.Services.AddScoped<MenuBusiness>();
builder.Services.AddScoped<WorkerLoginBusiness>();
builder.Services.AddScoped<ActivityLogBusiness>();

// 🔹 Registrar clases Data
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<WorkerData>();
builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<LoginData>();
builder.Services.AddScoped<MenuData>();
builder.Services.AddScoped<WorkerLoginData>();
builder.Services.AddScoped<ActivityLogData>();

// Agregar el HttpContextAccessor para obtener la IP del cliente
builder.Services.AddHttpContextAccessor();

// 🔹 Logging
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