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
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ClientBusiness _clientBusiness;

        public ClientController(ClientBusiness clientBusiness)
        {
            _clientBusiness = clientBusiness ?? throw new ArgumentNullException(nameof(clientBusiness));
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAllClients()
        {
            try
            {
                var clients = await _clientBusiness.GetAllClientsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClientById(int id)
        {
            try
            {
                var client = await _clientBusiness.GetClientByIdAsync(id);
                return Ok(client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo cliente.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        {
            try
            {
                var createdClient = await _clientBusiness.CreateClientAsync(clientDto);
                return CreatedAtAction(nameof(GetClientById), new { id = createdClient.ClientId }, createdClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] ClientDto clientDto)
        {
            if (id != clientDto.ClientId)
            {
                return BadRequest("El ID en la URL no coincide con el ID del cliente.");
            }

            try
            {
                var updated = await _clientBusiness.UpdateClientAsync(clientDto);
                if (!updated) return NotFound("Cliente no encontrado.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina un cliente por su ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                var deleted = await _clientBusiness.DeleteClientAsync(id);
                if (!deleted) return NotFound("Cliente no encontrado.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
