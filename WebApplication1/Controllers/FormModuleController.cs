using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Entity.DTOs;
using Business;

namespace WebApplication1.Controllers
{

[ApiController]
[Route("api/[controller]")]
public class FormModuleController : ControllerBase
{
    private readonly FormModuleBusiness _formModuleBusiness;
    private readonly ILogger<FormModuleController> _logger;

    public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
    {
        _formModuleBusiness = formModuleBusiness ?? throw new ArgumentNullException(nameof(formModuleBusiness));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea una nueva asignación de formulario a módulo.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FormModuleDto formModuleDto)
    {
        try
        {
            var createdFormModule = await _formModuleBusiness.CreateAsync(formModuleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdFormModule.Id }, createdFormModule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la asignación de formulario a módulo.");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de formularios a módulos.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _formModuleBusiness.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una asignación por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _formModuleBusiness.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Actualiza una asignación de formulario a módulo.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] FormModuleDto formModuleDto)
    {
        var success = await _formModuleBusiness.UpdateAsync(formModuleDto);
        if (!success)
            return BadRequest("No se pudo actualizar la asignación.");
        return NoContent();
    }

    /// <summary>
    /// Elimina una asignación de formulario a módulo.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _formModuleBusiness.DeleteAsync(id);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
}