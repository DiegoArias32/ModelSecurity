// WebApplication1/Controllers/MenuController.cs
using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly MenuBusiness _menuBusiness;
        private readonly ILogger<MenuController> _logger;

        public MenuController(MenuBusiness menuBusiness, ILogger<MenuController> logger)
        {
            _menuBusiness = menuBusiness ?? throw new ArgumentNullException(nameof(menuBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("byrol/{rolId}")]
        public async Task<IActionResult> GetMenuByRolId(int rolId)
        {
            try
            {
                var menu = await _menuBusiness.GetMenuByRolIdAsync(rolId);
                return Ok(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener menú para rol ID: {RolId}", rolId);
                return StatusCode(500, new { error = "Error al obtener el menú" });
            }
        }
    }
}