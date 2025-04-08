using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;
using System;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
        private readonly ClientBusiness _clientBusiness;
        private readonly ILogger<ClientController> _logger;

        public ClientController(ClientBusiness clientBusiness, ILogger<ClientController> logger)
        {
            _clientBusiness = clientBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _clientBusiness.GetAllClientsAsync();
                return Ok(clients);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener clientes");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetClientById(int id)
        {
            try
            {
                var client = await _clientBusiness.GetClientByIdAsync(id);
                return Ok(client);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "ID de cliente inválido: {ClientId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Cliente no encontrado con ID: {ClientId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener cliente con ID: {ClientId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                var createdClient = await _clientBusiness.CreateClientAsync(clientDto);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.ClientId }, createdClient);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear cliente");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear cliente");

                if (ex.InnerException is InvalidOperationException inner &&
                    inner.Message.Contains("Ya existe un cliente con"))
                {
                    return BadRequest(new { message = inner.Message });
                }

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ClientDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                var updatedClient = await _clientBusiness.UpdateClientAsync(clientDto);
                return Ok(updatedClient);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar cliente");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Cliente no encontrado con ID: {ClientId}", clientDto.ClientId);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar cliente");

                if (ex.InnerException is InvalidOperationException inner &&
                    inner.Message.Contains("Ya existe un cliente con"))
                {
                    return BadRequest(new { message = inner.Message });
                }

                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var deleted = await _clientBusiness.DeleteClientAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = "No se encontró el cliente a eliminar" });
                }

                return NoContent();
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al eliminar cliente con ID: {ClientId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Cliente no encontrado con ID: {ClientId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar cliente con ID: {ClientId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
