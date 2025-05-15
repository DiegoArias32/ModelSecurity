using System;
using System.Threading.Tasks;
using Business;
using Entity.DTOs;
using Entity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class FormModuleController : GenericController<FormModuleDto, FormModule>
{
    private readonly FormModuleBusiness _formModuleBusiness;

    public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        : base(
            formModuleBusiness,
            logger,
            "asignación de formulario a módulo",
            dto => dto.Id,
            new[] { "/ModuleId", "/FormId" }
        )
    {
        _formModuleBusiness = formModuleBusiness;
    }
}