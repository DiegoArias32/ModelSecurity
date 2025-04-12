using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Data;
using Entity.Model;
using Entity.DTOs;

namespace Business
{
    public class WorkerLoginBusiness
    {
        private readonly WorkerLoginData _loginData;
        private readonly ILogger<WorkerLoginBusiness> _logger;

        /// <summary>
        /// Constructor que recibe la instancia de WorkerLoginData y el logger.
        /// </summary>
        /// <param name="loginData">Instancia de <see cref="WorkerLoginData"/> para acceder a la capa de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public WorkerLoginBusiness(WorkerLoginData loginData, ILogger<WorkerLoginBusiness> logger)
        {
            _loginData = loginData ?? throw new ArgumentNullException(nameof(loginData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo login de trabajador tras realizar validaciones de negocio.
        /// </summary>
        /// <param name="loginDto">Instancia de <see cref="WorkerLoginDto"/> a crear.</param>
        /// <returns>La instancia del login creado.</returns>
        public async Task<WorkerLoginDto> CreateAsync(WorkerLoginDto loginDto)
        {
            try
            {
                // Validaciones de negocio antes de crear
                if (string.IsNullOrWhiteSpace(loginDto.Username))
                {
                    _logger.LogWarning("El nombre de usuario no puede estar vacío.");
                    throw new ArgumentException("El nombre de usuario no puede estar vacío.");
                }

                // Mapeo DTO -> Entidad
                var login = MapToEntity(loginDto);

                // Llamar al método de la capa de datos para crear el login
                var createdLogin = await _loginData.CreateAsync(login);

                // Mapeo de la entidad creada a DTO
                return MapToDTO(createdLogin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el login del trabajador.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los logins de trabajadores.
        /// </summary>
        /// <returns>Lista de DTOs <see cref="WorkerLoginDto"/>.</returns>
        public async Task<IEnumerable<WorkerLoginDto>> GetAllAsync()
        {
            try
            {
                var logins = await _loginData.GetAllAsync();
                return MapToDTOList(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los logins de trabajadores.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un login de trabajador específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del login.</param>
        /// <returns>El DTO <see cref="WorkerLoginDto"/> encontrado o null si no existe.</returns>
        public async Task<WorkerLoginDto?> GetByIdAsync(int id)
        {
            try
            {
                var login = await _loginData.GetByIdAsync(id);
                return login != null ? MapToDTO(login) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el login del trabajador con ID {LoginId}.", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un login de trabajador existente.
        /// </summary>
        /// <param name="loginDto">DTO con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False si no.</returns>
        public async Task<bool> UpdateAsync(WorkerLoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username))
                {
                    _logger.LogWarning("El nombre de usuario no puede estar vacío.");
                    throw new ArgumentException("El nombre de usuario no puede estar vacío.");
                }

                var existingLogin = await _loginData.GetByIdAsync(loginDto.LoginId);
                if (existingLogin == null)
                {
                    _logger.LogWarning("Login de trabajador no encontrado para actualizar.");
                    return false;
                }

                MapToEntity(loginDto, existingLogin);

                var updated = await _loginData.UpdateAsync(existingLogin);
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el login del trabajador con ID {LoginId}.", loginDto.LoginId);
                return false;
            }
        }

        /// <summary>
        /// Elimina un login de trabajador.
        /// </summary>
        /// <param name="id">Identificador del login a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False si no.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var login = await _loginData.GetByIdAsync(id);
                if (login == null)
                {
                    _logger.LogWarning("Login de trabajador no encontrado para eliminar.");
                    return false;
                }

                return await _loginData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el login del trabajador con ID {LoginId}.", id);
                return false;
            }
        }

        // Método para mapear de WorkerLogin a WorkerLoginDto
        private WorkerLoginDto MapToDTO(WorkerLogin workerLogin)
        {
            return new WorkerLoginDto
            {
                id = workerLogin.id,
                LoginId = workerLogin.LoginId,
                WorkerId = workerLogin.WorkerId,
                Username = workerLogin.Username,
                Password = workerLogin.Password,
                CreationDate = workerLogin.CreationDate,
                Status = workerLogin.Status
            };
        }

        // Método para mapear de WorkerLoginDto a WorkerLogin
        private WorkerLogin MapToEntity(WorkerLoginDto WorkerLoginDto)
        {
            return new WorkerLogin
            {
                id = WorkerLoginDto.id,
                LoginId = WorkerLoginDto.LoginId,
                WorkerId = WorkerLoginDto.WorkerId,
                Username = WorkerLoginDto.Username,
                Password = WorkerLoginDto.Password,
                CreationDate = WorkerLoginDto.CreationDate,
                Status = WorkerLoginDto.Status
            };
        }

        // Método para mapear de WorkerLoginDto a WorkerLogin cuando ya tenemos la entidad
        private void MapToEntity(WorkerLoginDto dto, WorkerLogin login)
        {
            login.WorkerId = dto.WorkerId;
            login.Username = dto.Username;
            login.Status = dto.Status;
        }

        // Método para mapear lista de entidades a lista de DTOs
        private IEnumerable<WorkerLoginDto> MapToDTOList(IEnumerable<WorkerLogin> logins)
        {
            return logins.Select(login => MapToDTO(login)).ToList();
        }
    }
}
