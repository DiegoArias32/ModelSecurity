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
    public class LoginController : GenericController<LoginDto, Login>
    {
        public LoginController(IGenericBusiness<LoginDto, Login> business, ILogger<LoginController> logger)
            : base(
                business,
                logger,
                "login",
                dto => dto.LoginId, // Nótese que usamos LoginId en lugar de Id como en otras entidades
                new[] { "/Username", "/Password" } // Campos permitidos para PATCH
            )
        {
        }
        
        // Si se necesitan endpoints específicos para Login, se pueden añadir aquí
    }
}