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
    public class WorkerBusiness
    {
        private readonly WorkerData _workerData;
        private readonly ILogger<WorkerBusiness> _logger;

        /// <summary>
        /// Constructor que recibe la instancia de WorkerData y el logger.
        /// </summary>
        /// <param name="workerData">Instancia de <see cref="WorkerData"/> para acceder a la capa de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public WorkerBusiness(WorkerData workerData, ILogger<WorkerBusiness> logger)
        {
            _workerData = workerData ?? throw new ArgumentNullException(nameof(workerData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo trabajador tras realizar validaciones de negocio.
        /// </summary>
        /// <param name="workerDto">Instancia de <see cref="WorkerDto"/> a crear.</param>
        /// <returns>La instancia del trabajador creado.</returns>
        public async Task<WorkerDto> CreateAsync(WorkerDto workerDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workerDto.FirstName) ||
                    string.IsNullOrWhiteSpace(workerDto.LastName) ||
                    string.IsNullOrWhiteSpace(workerDto.IdentityDocument) ||
                    string.IsNullOrWhiteSpace(workerDto.JobTitle) ||
                    string.IsNullOrWhiteSpace(workerDto.Email))
                {
                    _logger.LogWarning("Los campos obligatorios no pueden estar vacíos.");
                    throw new ArgumentException("Los campos obligatorios no pueden estar vacíos.");
                }

                var worker = MapToEntity(workerDto);
                var createdWorker = await _workerData.CreateAsync(worker);
                return MapToDTO(createdWorker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el trabajador.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los trabajadores registrados.
        /// </summary>
        /// <returns>Lista de DTOs workerDto.</returns>
        public async Task<IEnumerable<WorkerDto>> GetAllAsync()
        {
            try
            {
                var workers = await _workerData.GetAllAsync();
                return MapToDTOList(workers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los trabajadores.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un trabajador específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del trabajador.</param>
        /// <returns>El DTO WorkerDto encontrado o null si no existe.</returns>
        public async Task<WorkerDto?> GetByIdAsync(int id)
        {
            try
            {
                var worker = await _workerData.GetByIdAsync(id);
                return worker != null ? MapToDTO(worker) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el trabajador con ID {WorkerId}.", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un trabajador existente.
        /// </summary>
        /// <param name="workerDto">DTO con la información actualizada del trabajador.</param>
        /// <returns>True si la operación fue exitosa, False si no.</returns>
        public async Task<bool> UpdateAsync(WorkerDto workerDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(workerDto.FirstName) ||
                    string.IsNullOrWhiteSpace(workerDto.LastName) ||
                    string.IsNullOrWhiteSpace(workerDto.IdentityDocument) ||
                    string.IsNullOrWhiteSpace(workerDto.JobTitle) ||
                    string.IsNullOrWhiteSpace(workerDto.Email))
                {
                    _logger.LogWarning("Los campos obligatorios no pueden estar vacíos.");
                    throw new ArgumentException("Los campos obligatorios no pueden estar vacíos.");
                }

                var existingWorker = await _workerData.GetByIdAsync(workerDto.WorkerId);
                if (existingWorker == null)
                {
                    _logger.LogWarning("Trabajador no encontrado para actualizar.");
                    return false;
                }

                MapToEntity(workerDto, existingWorker);
                return await _workerData.UpdateAsync(existingWorker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el trabajador con ID {WorkerId}.", workerDto.WorkerId);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var existing = await _workerData.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Trabajador no encontrado para eliminación permanente.");
            return false;
        }

        return await _workerData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el trabajador con ID {WorkerId}.", id);
        return false;
    }
}

        /// <summary>
        /// Elimina un trabajador.
        /// </summary>
        /// <param name="id">Identificador del trabajador a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False si no.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existing = await _workerData.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Trabajador no encontrado para eliminar.");
                    return false;
                }

                return await _workerData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el trabajador con ID {WorkerId}.", id);
                return false;
            }
        }

        // Método para mapear de Worker a WorkerDto
        private WorkerDto MapToDTO(Worker worker)
        {
            return new WorkerDto
            {
                WorkerId = worker.WorkerId,
                FirstName = worker.FirstName,
                LastName = worker.LastName,
                IdentityDocument = worker.IdentityDocument,
                JobTitle = worker.JobTitle,
                Email = worker.Email,
                Phone = worker.Phone,
                HireDate = worker.HireDate
            };
        }

        // Método para mapear de WorkerDto a Worker
        private Worker MapToEntity(WorkerDto workerDto)
        {
            return new Worker
            {
                WorkerId = workerDto.WorkerId,
                FirstName = workerDto.FirstName,
                LastName = workerDto.LastName,
                IdentityDocument = workerDto.IdentityDocument,
                JobTitle = workerDto.JobTitle,
                Email = workerDto.Email,
                Phone = workerDto.Phone,
                HireDate = workerDto.HireDate
            };
        }

        // Método para mapear de WorkerDto a Worker (cuando ya tenemos un trabajador)
        private void MapToEntity(WorkerDto workerDto, Worker entity)
        {
            entity.FirstName = workerDto.FirstName;
            entity.LastName = workerDto.LastName;
            entity.IdentityDocument = workerDto.IdentityDocument;
            entity.JobTitle = workerDto.JobTitle;
            entity.Email = workerDto.Email;
            entity.Phone = workerDto.Phone;
            entity.HireDate = workerDto.HireDate;
        }

        // Método para mapear una lista de Worker a una lista de WorkerDto
        private IEnumerable<WorkerDto> MapToDTOList(IEnumerable<Worker> workers)
        {
            return workers.Select(worker => MapToDTO(worker)).ToList();
        }
    }
}
