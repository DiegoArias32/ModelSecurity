// Data/MenuData.cs
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public class MenuData
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<MenuData> _logger;

        public MenuData(
            RolFormPermissionData rolFormPermissionData,
            FormModuleData formModuleData,
            ILogger<MenuData> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _formModuleData = formModuleData;
            _logger = logger;
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
                        Name = "Administración",
                        Icon = "fas fa-cogs",
                        Url = "#",
                        IsActive = true,
                        Children = new List<MenuItemDto>
                        {
                            new MenuItemDto 
                            { 
                                Id = 11, 
                                Name = "Usuarios", 
                                Icon = "fas fa-users", 
                                Url = "/admin/users",
                                IsActive = true
                            },
                            new MenuItemDto 
                            { 
                                Id = 12, 
                                Name = "Roles", 
                                Icon = "fas fa-user-tag", 
                                Url = "/admin/roles",
                                IsActive = true
                            },
                            new MenuItemDto 
                            { 
                                Id = 13, 
                                Name = "Módulos", 
                                Icon = "fas fa-cubes", 
                                Url = "/admin/modules",
                                IsActive = true
                            },
                            new MenuItemDto 
                            { 
                                Id = 14, 
                                Name = "Formularios", 
                                Icon = "fas fa-file-alt", 
                                Url = "/admin/forms",
                                IsActive = true
                            }
                        }
                    };
                    
                    menu.Add(adminMenu);
                }
                else // Usuario normal
                {
                    // Solo menú de perfil
                    var profileMenu = new MenuItemDto
                    {
                        Id = 2,
                        Name = "Mi Perfil",
                        Icon = "fas fa-user",
                        Url = "/profile",
                        IsActive = true
                    };
                    
                    menu.Add(profileMenu);
                }
                
                return menu;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú por rol ID: {RolId}", rolId);
                throw;
            }
        }
    }
}