// Data/MenuData.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Agregado para usar métodos LINQ
using System.Threading.Tasks;

namespace Data
{
    public class MenuData
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly FormModuleData _formModuleData;
        private readonly FormData _formData;
        private readonly PermissionData _permissionData; // Añadido para obtener permisos
        private readonly ILogger<MenuData> _logger;

        public MenuData(
            RolFormPermissionData rolFormPermissionData,
            FormModuleData formModuleData,
            FormData formData,
            PermissionData permissionData, // Añadido
            ILogger<MenuData> logger)
        {
            _rolFormPermissionData = rolFormPermissionData ?? throw new ArgumentNullException(nameof(rolFormPermissionData));
            _formModuleData = formModuleData ?? throw new ArgumentNullException(nameof(formModuleData));
            _formData = formData ?? throw new ArgumentNullException(nameof(formData));
            _permissionData = permissionData ?? throw new ArgumentNullException(nameof(permissionData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<MenuItemDto>> GetMenuByRolIdAsync(int rolId)
        {
            try
            {
                // Obtener los permisos del rol específico
                var rolPermissions = await _rolFormPermissionData.GetByRolIdAsync(rolId);
                
                // Crear el menú según el rol
                var menu = new List<MenuItemDto>();
                
                // Para rol admin (supongamos que rolId = 1 es admin)
                if (rolId == 1) // Admin
                {
                    // Menú de administración
                    var adminMenu = new MenuItemDto
                    {
                        Id = 1,
                        Name = "Dashboard",
                        Icon = "fas fa-tachometer-alt",
                        Url = "/dashboard",
                        IsActive = true
                    };
                    
                    menu.Add(adminMenu);
                    
                    // Agregar todos los módulos de administración
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 11, 
                        Name = "Usuarios", 
                        Icon = "fas fa-users", 
                        Url = "/usuarios",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 12, 
                        Name = "Roles", 
                        Icon = "fas fa-user-tag", 
                        Url = "/roles",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 13, 
                        Name = "Módulos", 
                        Icon = "fas fa-cubes", 
                        Url = "/modulos",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 14, 
                        Name = "Formularios", 
                        Icon = "fas fa-file-alt", 
                        Url = "/formularios",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 15, 
                        Name = "Permisos", 
                        Icon = "fas fa-key", 
                        Url = "/permisos",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 16, 
                        Name = "Asignación Rol-Form-Permisos", 
                        Icon = "fas fa-user-lock", 
                        Url = "/rolformpermisos",
                        IsActive = true
                    });
                    
                    menu.Add(new MenuItemDto 
                    { 
                        Id = 17, 
                        Name = "Asignación Form-Módulos", 
                        Icon = "fas fa-th-list", 
                        Url = "/formmodulos",
                        IsActive = true
                    });
                    
                    // Menú de perfil para admin
                    menu.Add(new MenuItemDto
                    {
                        Id = 18,
                        Name = "Mi Perfil",
                        Icon = "fas fa-user",
                        Url = "/perfil",
                        IsActive = true
                    });
                }
                else // Usuario normal
                {
                    // Siempre agregar el menú de perfil
                    var profileMenu = new MenuItemDto
                    {
                        Id = 2,
                        Name = "Mi Perfil",
                        Icon = "fas fa-user",
                        Url = "/perfil",
                        IsActive = true
                    };
                    
                    menu.Add(profileMenu);
                    
                    // Añadir elementos de menú según los permisos
                    // Corregido: Verificar si rolPermissions es nulo y luego usar Count()
                    if (rolPermissions != null && rolPermissions.Count() > 0)
                    {
                        int menuId = 3; // Comenzar desde el ID 3
                        
                        foreach (var permission in rolPermissions)
                        {
                            try
                            {
                                // Corregido: Obtener el permiso usando PermissionData
                                var permissionEntity = await _permissionData.GetByIdAsync(permission.PermissionId);
                                var form = await _formData.GetByIdAsync(permission.FormId);
                                
                                // Solo agregar si tiene permiso de lectura y el formulario existe
                                if (permissionEntity != null && permissionEntity.Can_Read && form != null)
                                {
                                    // Evitar duplicar el perfil que ya añadimos
                                    if (!form.Code.Equals("PERFIL", StringComparison.OrdinalIgnoreCase))
                                    {
                                        var moduleMenuItem = new MenuItemDto
                                        {
                                            Id = menuId++,
                                            Name = form.Name,
                                            Icon = GetIconForFormCode(form.Code),
                                            Url = $"/{form.Code.ToLower()}",
                                            IsActive = form.Active
                                        };
                                        
                                        menu.Add(moduleMenuItem);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error al procesar permiso ID: {PermissionId} para menú", permission.PermissionId);
                                // Continuar con el siguiente permiso
                            }
                        }
                    }
                }
                
                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú por rol ID: {RolId}", rolId);
                throw;
            }
        }

        // Método auxiliar para asignar iconos según el código del formulario
        private string GetIconForFormCode(string code)
        {
            switch (code.ToUpper())
            {
                case "DASHBOARD": return "fas fa-tachometer-alt";
                case "USUARIOS": return "fas fa-users";
                case "ROLES": return "fas fa-user-tag";
                case "MODULOS": return "fas fa-cubes";
                case "FORMULARIOS": return "fas fa-file-alt";
                case "PERMISOS": return "fas fa-key";
                case "ROLFORMPERMISOS": return "fas fa-user-lock";
                case "FORMMODULOS": return "fas fa-th-list";
                case "PERFIL": return "fas fa-user";
                default: return "fas fa-file";
            }
        }
    }
}