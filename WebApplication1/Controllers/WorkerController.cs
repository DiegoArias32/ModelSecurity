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
    public class WorkerController : GenericController<WorkerDto, Worker>
    {
        private readonly WorkerBusiness _workerBusiness;

        public WorkerController(WorkerBusiness workerBusiness, ILogger<WorkerController> logger)
            : base(
                workerBusiness,
                logger,
                "trabajador",
                dto => dto.WorkerId, // Nota: Usamos WorkerId en lugar de Id
                new[] { "/FirstName", "/LastName", "/IdentityDocument", "/JobTitle", "/Email", "/Phone", "/HireDate" }
            )
        {
            _workerBusiness = workerBusiness ?? throw new ArgumentNullException(nameof(workerBusiness));
        }
    }
}