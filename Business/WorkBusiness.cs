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
    public class WorkerBusiness : GenericBusiness<WorkerDto, Worker>
    {
        private readonly WorkerData _workerData;

        public WorkerBusiness(WorkerData workerData, ILogger<WorkerBusiness> logger)
            : base(workerData, logger)
        {
            _workerData = workerData;
        }

        // Sobrescribimos la creación para validar documento único
        public override async Task<WorkerDto> CreateAsync(WorkerDto workerDto)
        {
            try
            {
                ValidateDto(workerDto);
                
                // Verificar si ya existe un documento
                if (await _workerData.ExistsIdentityDocumentAsync(workerDto.IdentityDocument))
                {
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{workerDto.IdentityDocument}'");
                }

                var worker = MapToEntity(workerDto);
                var createdWorker = await _workerData.CreateAsync(worker);
                return MapToDto(createdWorker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el trabajador");
                throw;
            }
        }

        // Sobrescribimos para validar documento único
        public override async Task<bool> UpdateAsync(WorkerDto workerDto)
        {
            try
            {
                ValidateDto(workerDto);
                
                // Verificar si ya existe otro trabajador con el mismo documento
                if (await _workerData.ExistsIdentityDocumentAsync(workerDto.IdentityDocument, workerDto.WorkerId))
                {
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{workerDto.IdentityDocument}'");
                }

                var worker = MapToEntity(workerDto);
                return await _workerData.UpdateAsync(worker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el trabajador");
                return false;
            }
        }

        protected override void ValidateDto(WorkerDto dto)
        {
            if (dto == null)
                throw new ArgumentException("El objeto trabajador no puede ser nulo");
                
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("El nombre del trabajador es obligatorio");
                
            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("El apellido del trabajador es obligatorio");
                
            if (string.IsNullOrWhiteSpace(dto.IdentityDocument))
                throw new ArgumentException("El documento de identidad es obligatorio");
                
            if (string.IsNullOrWhiteSpace(dto.JobTitle))
                throw new ArgumentException("El cargo es obligatorio");
                
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("El email es obligatorio");
        }

        protected override Worker MapToEntity(WorkerDto dto)
        {
            return new Worker
            {
                WorkerId = dto.WorkerId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                IdentityDocument = dto.IdentityDocument,
                JobTitle = dto.JobTitle,
                Email = dto.Email,
                Phone = dto.Phone,
                HireDate = dto.HireDate
            };
        }

        protected override WorkerDto MapToDto(Worker entity)
        {
            return new WorkerDto
            {
                WorkerId = entity.WorkerId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                IdentityDocument = entity.IdentityDocument,
                JobTitle = entity.JobTitle,
                Email = entity.Email,
                Phone = entity.Phone,
                HireDate = entity.HireDate
            };
        }

        protected override IEnumerable<WorkerDto> MapToDtoList(IEnumerable<Worker> entities)
        {
            return entities.Select(MapToDto).ToList();
        }
    }
}