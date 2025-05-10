// Data/MenuData.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class MenuData
    {
        private readonly IRepository<RolFormPermission> _rolFormPermissionRepository;
        private readonly IRepository<FormModule> _formModuleRepository;
        private readonly IRepository<Form> _formRepository;
        private readonly IRepository<Permission> _permissionRepository;
        private readonly ILogger<MenuData> _logger;

        public MenuData(
            IRepository<RolFormPermission> rolFormPermissionRepository,
            IRepository<FormModule> formModuleRepository,
            IRepository<Form> formRepository,
            IRepository<Permission> permissionRepository,
            ILogger<MenuData> logger)
        {
            _rolFormPermissionRepository = rolFormPermissionRepository;
            _formModuleRepository = formModuleRepository;
            _formRepository = formRepository;
            _permissionRepository = permissionRepository;
            _logger = logger;
        }

        public async Task<List<MenuItemDto>> GetMenuByRolIdAsync(int rolId)
        {
            try
            {
                // Obtener los permisos del rol específico
                var allRolFormPermissions = await _rolFormPermissionRepository.GetAllAsync();
                var rolPermissions = allRolFormPermissions.Where(rfp => rfp.RolId == rolId).ToList();
                
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
                    
                    // Más elementos de menú para admin...
                    
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
                    if (rolPermissions != null && rolPermissions.Count > 0)
                    {
                        int menuId = 3; // Comenzar desde el ID 3
                        
                        foreach (var permission in rolPermissions)
                        {
                            try
                            {
                                var allPermissions = await _permissionRepository.GetAllAsync();
                                var permissionEntity = allPermissions.FirstOrDefault(p => p.Id == permission.PermissionId);
                                
                                var allForms = await _formRepository.GetAllAsync();
                                var form = allForms.FirstOrDefault(f => f.Id == permission.FormId);
                                
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