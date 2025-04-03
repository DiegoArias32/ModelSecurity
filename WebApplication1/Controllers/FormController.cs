using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase
    {
        private readonly FormBusiness _formBusiness;
        private readonly ILogger<FormController> _logger;

        public FormController(FormBusiness formBusiness, ILogger<FormController> logger)
        {
            _formBusiness = formBusiness ?? throw new ArgumentNullException(nameof(formBusiness));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los formularios.
        /// </summary>
        /// <returns>Lista de formularios.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormDto>>> GetAllForms()
        {
            try
            {
                var forms = await _formBusiness.GetAllFormsAsync();
                return Ok(forms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener los formularios.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un formulario por ID.
        /// </summary>
        /// <param name="id">ID del formulario.</param>
        /// <returns>Formulario encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FormDto>> GetFormById(int id)
        {
            try
            {
                var form = await _formBusiness.GetFormByIdAsync(id);
                return Ok(form);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo formulario.
        /// </summary>
        /// <param name="formDto">Datos del formulario.</param>
        /// <returns>Formulario creado.</returns>
        [HttpPost]
        public async Task<ActionResult<FormDto>> CreateForm([FromBody] FormDto formDto)
        {
            try
            {
                var createdForm = await _formBusiness.CreateFormAsync(formDto);
                return CreatedAtAction(nameof(GetFormById), new { id = createdForm.Id }, createdForm);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear el formulario.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un formulario existente.
        /// </summary>
        /// <param name="id">ID del formulario.</param>
        /// <param name="formDto">Datos actualizados.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] FormDto formDto)
        {
            if (id != formDto.Id)
                return BadRequest(new { message = "El ID en la URL no coincide con el del formulario." });

            try
            {
                var result = await _formBusiness.UpdateFormAsync(formDto);
                if (!result) return NotFound(new { message = "Formulario no encontrado." });

                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario.");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un formulario.
        /// </summary>
        /// <param name="id">ID del formulario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            try
            {
                var result = await _formBusiness.DeleteFormAsync(id);
                if (!result) return NotFound(new { message = "Formulario no encontrado." });

                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar el formulario.");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
