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
    public class FormController : GenericController<FormDto, Form>
    {
        public FormController(IGenericBusiness<FormDto, Form> business, ILogger<FormController> logger)
            : base(
                business, 
                logger, 
                "formulario", 
                dto => dto.Id,
                new[] { "/Name", "/Code", "/Active" }
            )
        {
        }
        
        // Solo añadir métodos si necesitas endpoints personalizados
        // Si solo necesitas CRUD básico, no necesitas ningún método adicional
    }
}