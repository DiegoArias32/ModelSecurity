using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolUserController : GenericController<RolUserDto, RolUser>
    {
        private readonly RolUserBusiness _rolUserBusiness;

        public RolUserController(RolUserBusiness rolUserBusiness, ILogger<RolUserController> logger)
            : base(
                rolUserBusiness,
                logger,
                "relación rol-usuario",
                dto => dto.Id,
                new[] { "/UserId", "/RolId" }
            )
        {
            _rolUserBusiness = rolUserBusiness ?? throw new ArgumentNullException(nameof(rolUserBusiness));
        }
        
        // No se requieren métodos adicionales ya que GenericController
        // proporciona los endpoints CRUD básicos
    }
}