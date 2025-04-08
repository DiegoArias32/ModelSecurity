using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class ClientBusiness
    {
        private readonly ClientData _clientData;
        private readonly ILogger<ClientBusiness> _logger;

        public ClientBusiness(ClientData clientData, ILogger<ClientBusiness> logger)
        {
            _clientData = clientData ?? throw new ArgumentNullException(nameof(clientData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            try
            {
                var clients = await _clientData.GetAllAsync();
                return MapToDTOList(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de clientes", ex);
            }
        }

        public async Task<ClientDto> GetClientByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un cliente con ID inválido: {ClientId}", id);
                throw new ValidationException("id", "El ID del cliente debe ser mayor que cero");
            }

            try
            {
                var client = await _clientData.GetByIdAsync(id);
                if (client == null)
                {
                    _logger.LogInformation("No se encontró ningún cliente con ID: {ClientId}", id);
                    throw new EntityNotFoundException("Cliente", id);
                }

                return MapToDTO(client);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID: {ClientId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el cliente con ID {id}", ex);
            }
        }

        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            try
            {
                ValidateClient(clientDto);

                var client = MapToEntity(clientDto);
                var clienteCreado = await _clientData.CreateAsync(client);

                return MapToDTO(clienteCreado);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo cliente: {ClientName} {ClientLastName}", 
                    clientDto?.FirstName ?? "null", clientDto?.LastName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el cliente", ex);
            }
        }

        public async Task<bool> UpdateClientAsync(ClientDto clientDto)
        {
            if (clientDto.ClientId <= 0)
            {
                _logger.LogWarning("Se intentó actualizar un cliente con ID inválido: {ClientId}", clientDto.ClientId);
                throw new ValidationException("id", "El ID del cliente debe ser mayor que cero");
            }

            try
            {
                ValidateClient(clientDto);

                // Verificar si el cliente existe
                var clienteExistente = await _clientData.GetByIdAsync(clientDto.ClientId);
                if (clienteExistente == null)
                {
                    _logger.LogInformation("No se encontró ningún cliente con ID: {ClientId}", clientDto.ClientId);
                    throw new EntityNotFoundException("Cliente", clientDto.ClientId);
                }

                var client = MapToEntity(clientDto);
                var actualizado = await _clientData.UpdateAsync(client);

                return actualizado;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente con ID: {ClientId}", clientDto.ClientId);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el cliente con ID {clientDto.ClientId}", ex);
            }
        }

        public async Task<bool> DeleteClientAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un cliente con ID inválido: {ClientId}", id);
                throw new ValidationException("id", "El ID del cliente debe ser mayor que cero");
            }

            try
            {
                // Verificar si el cliente existe
                var clienteExistente = await _clientData.GetByIdAsync(id);
                if (clienteExistente == null)
                {
                    _logger.LogInformation("No se encontró ningún cliente con ID: {ClientId}", id);
                    throw new EntityNotFoundException("Cliente", id);
                }

                return await _clientData.DeleteAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID: {ClientId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el cliente con ID {id}", ex);
            }
        }

        private void ValidateClient(ClientDto clientDto)
        {
            if (clientDto == null)
            {
                throw new ValidationException("El objeto cliente no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(clientDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cliente con FirstName vacío");
                throw new ValidationException("FirstName", "El nombre del cliente es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(clientDto.LastName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cliente con LastName vacío");
                throw new ValidationException("LastName", "El apellido del cliente es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(clientDto.Email))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cliente con Email vacío");
                throw new ValidationException("Email", "El email del cliente es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(clientDto.IdentityDocument))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cliente con IdentityDocument vacío");
                throw new ValidationException("IdentityDocument", "El documento de identidad del cliente es obligatorio");
            }

            if (clientDto.Phone <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un cliente con Phone inválido");
                throw new ValidationException("Phone", "El número de teléfono del cliente es obligatorio y debe ser válido");
            }
        }

        private ClientDto MapToDTO(Client client)
        {
            return new ClientDto
            {
                ClientId = client.ClientId,
                FirstName = client.FirstName,
                LastName = client.LastName,
                IdentityDocument = client.IdentityDocument,
                ClientType = client.ClientType,
                Phone = client.Phone,
                Email = client.Email,
                Address = client.Address,
                SocioeconomicStratification = client.SocioeconomicStratification ?? 0,
            };
        }

        private Client MapToEntity(ClientDto clientDto)
        {
            return new Client
            {
                ClientId = clientDto.ClientId,
                FirstName = clientDto.FirstName,
                LastName = clientDto.LastName,
                IdentityDocument = clientDto.IdentityDocument,
                ClientType = clientDto.ClientType,
                Phone = clientDto.Phone,
                Email = clientDto.Email,
                Address = clientDto.Address,
                SocioeconomicStratification = clientDto.SocioeconomicStratification,
            };
        }

        private IEnumerable<ClientDto> MapToDTOList(IEnumerable<Client> clients)
        {
            return clients.Select(client => MapToDTO(client)).ToList();
        }
    }
}