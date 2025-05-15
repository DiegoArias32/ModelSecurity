using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : GenericController<RolDto, Rol>
    {
        private readonly RolBusiness _rolBusiness;

        public RolController(RolBusiness rolBusiness, ILogger<RolController> logger)
            : base(
                rolBusiness, 
                logger, 
                "rol", 
                dto => dto.Id,
                new[] { "/Name", "/Description" } // Campos permitidos para PATCH
            )
        {
            _rolBusiness = rolBusiness;
        }

    }
}