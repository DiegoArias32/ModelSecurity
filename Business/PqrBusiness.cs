using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Business
{
    public class PqrBusiness
    {
        private readonly PqrData _pqrData;
        private readonly ILogger<PqrBusiness> _logger;

        public PqrBusiness(PqrData pqrData, ILogger<PqrBusiness> logger)
        {
            _pqrData = pqrData ?? throw new ArgumentNullException(nameof(pqrData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo PQR.
        /// </summary>
        public async Task<PqrDto> CreateAsync(PqrDto pqrDto)
        {
            try
            {
                var pqr = MapToEntity(pqrDto);
                var created = await _pqrData.CreateAsync(pqr);
                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el PQR.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los PQRs.
        /// </summary>
        public async Task<IEnumerable<PqrDto>> GetAllAsync()
        {
            try
            {
                var pqrList = await _pqrData.GetAllAsync();
                return pqrList.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los PQRs.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un PQR por ID.
        /// </summary>
        public async Task<PqrDto?> GetByIdAsync(int id)
        {
            try
            {
                var pqr = await _pqrData.GetByIdAsync(id);
                return pqr is not null ? MapToDto(pqr) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el PQR con ID {PqrId}", id);
                throw;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var pqr = await _pqrData.GetByIdAsync(id);
        if (pqr == null)
            return false;

        return await _pqrData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el PQR.");
        return false;
    }
}

        /// <summary>
        /// Actualiza un PQR.
        /// </summary>
        public async Task<bool> UpdateAsync(PqrDto pqrDto)
        {
            try
            {
                var pqr = MapToEntity(pqrDto);
                return await _pqrData.UpdateAsync(pqr);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el PQR.");
                throw;
            }
        }

        /// <summary>
        /// Elimina un PQR.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _pqrData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el PQR.");
                throw;
            }
        }

        // MÉTODOS DE MAPEOS

        private static Pqr MapToEntity(PqrDto dto)
        {
            return new Pqr
            {
                PqrId = dto.PqrId,
                PqrType = dto.PqrType,
                Description = dto.Description,
                PqrStatus = dto.PqrStatus,
                ResolutionDate = dto.ResolutionDate,
                WorkerId = dto.WorkerId,
                ClientId = dto.ClientId
                // Las relaciones Worker y Client se podrían asignar desde otra capa si es necesario
            };
        }

        private static PqrDto MapToDto(Pqr entity)
        {
            return new PqrDto
            {
                PqrId = entity.PqrId,
                PqrType = entity.PqrType,
                Description = entity.Description,
                PqrStatus = entity.PqrStatus,
                ResolutionDate = entity.ResolutionDate,
                WorkerId = entity.WorkerId,
                ClientId = entity.ClientId
            };
        }
    }
}
