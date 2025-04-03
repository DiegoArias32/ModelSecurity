using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/modules")]
    public class ModuleController : ControllerBase
    {
        private readonly ModuleBusiness _moduleBusiness;

        public ModuleController(ModuleBusiness moduleBusiness)
        {
            _moduleBusiness = moduleBusiness ?? throw new ArgumentNullException(nameof(moduleBusiness));
        }

        /// <summary>
        /// Obtiene todos los módulos.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAll()
        {
            try
            {
                var modules = await _moduleBusiness.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un módulo por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleDto>> GetById(int id)
        {
            try
            {
                var module = await _moduleBusiness.GetModuleByIdAsync(id);
                return Ok(module);
            }
            catch (EntityNotFoundException)
            {
                return NotFound($"No se encontró un módulo con ID {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo módulo.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ModuleDto>> Create([FromBody] ModuleDto moduleDto)
        {
            try
            {
                var createdModule = await _moduleBusiness.CreateModuleAsync(moduleDto);
                return CreatedAtAction(nameof(GetById), new { id = createdModule.Id }, createdModule);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un módulo existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ModuleDto moduleDto)
        {
            try
            {
                if (id != moduleDto.Id)
                    return BadRequest("El ID del módulo no coincide con el parámetro de la URL.");

                bool result = await _moduleBusiness.UpdateModuleAsync(moduleDto);
                if (!result) return NotFound($"No se encontró un módulo con ID {id}");

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un módulo por ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _moduleBusiness.DeleteModuleAsync(id);
                if (!result) return NotFound($"No se encontró un módulo con ID {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
