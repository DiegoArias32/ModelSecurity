using Data;
using Entity.Contexts;
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
        private readonly ClientData _clientData; // Clase que maneja las operaciones de base de datos
        private readonly ILogger _logger;

        // Constructor que recibe la instancia de ClientData y ILogger
        public ClientBusiness(ClientData clientData, ILogger<ClientBusiness> logger)
        {
            _clientData = clientData ?? throw new ArgumentNullException(nameof(clientData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los clientes.
        /// </summary>
        /// <returns>Lista de clientes en formato DTO.</returns>
        public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
        {
            try
            {
                var clients = await _clientData.GetAllAsync();
                return MapToDTOList(clients); // Usamos el método MapToDTOList aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes");
                throw new ExternalServiceException("Base de datos", "Error al obtener los clientes", ex);
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>Cliente en formato DTO.</returns>
        public async Task<ClientDto> GetClientByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un cliente con ID inválido: {ClientId}", id);
                throw new ValidationException("id", "El ID del cliente debe ser mayor que cero.");
            }

            try
            {
                var client = await _clientData.GetByIdAsync(id);
                if (client == null)
                {
                    _logger.LogInformation("Cliente con ID {ClientId} no encontrado.", id);
                    throw new EntityNotFoundException("Cliente", id);
                }

                return MapToDTO(client); // Usamos el método MapToDTO aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID {ClientId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al obtener el cliente con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo cliente.
        /// </summary>
        /// <param name="clientDto">DTO con la información del cliente.</param>
        /// <returns>El cliente creado en formato DTO.</returns>
        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto)
        {
            try
            {
                ValidateClient(clientDto); // Validar los datos del cliente

                var client = MapToEntity(clientDto); // Usamos el método MapToEntity aquí
                var createdClient = await _clientData.CreateAsync(client);

                return MapToDTO(createdClient); // Usamos el método MapToDTO aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente: {ClientName}", clientDto?.FirstName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el cliente", ex);
            }
        }

        /// <summary>
        /// Actualiza un cliente existente.
        /// </summary>
        /// <param name="clientDto">DTO con la información actualizada del cliente.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        public async Task<bool> UpdateClientAsync(ClientDto clientDto)
        {
            try
            {
                var existingClient = await _clientData.GetByIdAsync(clientDto.ClientId);
                if (existingClient == null)
                {
                    _logger.LogWarning("Cliente con ID {ClientId} no encontrado.", clientDto.ClientId);
                    throw new EntityNotFoundException("Cliente", clientDto.ClientId);
                }

                existingClient = MapToEntity(clientDto); // Usamos el método MapToEntity aquí

                var updated = await _clientData.UpdateAsync(existingClient);

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente con ID {ClientId}", clientDto.ClientId);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el cliente", ex);
            }
        }

        /// <summary>
        /// Elimina un cliente.
        /// </summary>
        /// <param name="id">ID del cliente a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        public async Task<bool> DeleteClientAsync(int id)
        {
            try
            {
                var client = await _clientData.GetByIdAsync(id);
                if (client == null)
                {
                    _logger.LogWarning("Cliente con ID {ClientId} no encontrado.", id);
                    throw new EntityNotFoundException("Cliente", id);
                }

                var deleted = await _clientData.DeleteAsync(id);
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID {ClientId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el cliente", ex);
            }
        }

        /// <summary>
        /// Método para validar el DTO del cliente.
        /// </summary>
        private void ValidateClient(ClientDto clientDto)
        {
            if (clientDto == null)
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con datos nulos.");
                throw new ValidationException("clientDto", "El cliente no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con FirstName vacío.");
                throw new ValidationException("FirstName", "El nombre del cliente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.LastName))
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con LastName vacío.");
                throw new ValidationException("LastName", "El apellido del cliente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.Email))
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con Email vacío.");
                throw new ValidationException("Email", "El email del cliente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.IdentityDocument))
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con IdentityDocument vacío.");
                throw new ValidationException("IdentityDocument", "El documento de identidad del cliente es obligatorio.");
            }

            if (clientDto.Phone <= 0)
            {
                _logger.LogWarning("Se intentó crear o actualizar un cliente con Phone inválido.");
                throw new ValidationException("Phone", "El número de teléfono del cliente es obligatorio.");
            }
        }

        // Método para mapear de Client a ClientDTO
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
                RegistrationDate = client.RegistrationDate ?? DateTime.MinValue
            };
        }

        // Método para mapear de ClientDTO a Client
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
                RegistrationDate = clientDto.RegistrationDate
            };
        }

        // Método para mapear una lista de Client a una lista de ClientDTO
        private IEnumerable<ClientDto> MapToDTOList(IEnumerable<Client> clients)
        {
            return clients.Select(client => MapToDTO(client)).ToList();
        }
    }
}
