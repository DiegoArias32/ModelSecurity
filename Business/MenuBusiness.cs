// Business/MenuBusiness.cs
using Entity.DTOs;
using Microsoft.Extensions.Logging;
using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business
{
    public class MenuBusiness
    {
        private readonly MenuData _menuData;
        private readonly ILogger<MenuBusiness> _logger;

        public MenuBusiness(MenuData menuData, ILogger<MenuBusiness> logger)
        {
            _menuData = menuData ?? throw new ArgumentNullException(nameof(menuData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<MenuItemDto>> GetMenuByRolIdAsync(int rolId)
        {
            try
            {
                return await _menuData.GetMenuByRolIdAsync(rolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MenuBusiness.GetMenuByRolIdAsync para rolId: {RolId}", rolId);
                throw;
            }
        }
    }
}