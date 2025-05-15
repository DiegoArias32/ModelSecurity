using Business;
using Entity.DTOs;
using Entity.Model;
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
    public class ModuleController : GenericController<ModuleDto, Module>
    {
        private readonly ModuleBusiness _moduleBusiness;

        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
            : base(
                moduleBusiness, 
                logger, 
                "módulo", 
                dto => dto.Id,
                new[] { "/Code", "/Active" } // Campos permitidos para PATCH
            )
        {
            _moduleBusiness = moduleBusiness;
        }

        // Si se necesitan endpoints específicos para Module, se pueden añadir aquí
    }
}