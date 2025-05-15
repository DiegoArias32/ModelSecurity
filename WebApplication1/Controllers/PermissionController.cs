using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : GenericController<PermissionDto, Permission>
    {
        public PermissionController(IGenericBusiness<PermissionDto, Permission> business, ILogger<PermissionController> logger)
            : base(
                business,
                logger,
                "permiso",
                dto => dto.Id,
                new[] { "/CanRead", "/CanCreate", "/CanUpdate", "/CanDelete" } // Campos permitidos para PATCH
            )
        {
        }
        
        // Si necesitas endpoints específicos para Permission, puedes añadirlos aquí
    }
}