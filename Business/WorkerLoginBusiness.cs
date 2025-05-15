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
    public class WorkerLoginBusiness : GenericBusiness<WorkerLoginDto, WorkerLogin>
    {
        private readonly WorkerLoginData _loginData;

        public WorkerLoginBusiness(WorkerLoginData loginData, ILogger<WorkerLoginBusiness> logger)
            : base(loginData, logger)
        {
            _loginData = loginData;
        }

        // Sobrescribimos la creación para validar username único
        public override async Task<WorkerLoginDto> CreateAsync(WorkerLoginDto loginDto)
        {
            try
            {
                ValidateDto(loginDto);
                
                // Verificar si ya existe un username
                if (await _loginData.ExistsUsernameAsync(loginDto.Username))
                {
                    throw new InvalidOperationException($"Ya existe un login con el username '{loginDto.Username}'");
                }

                var login = MapToEntity(loginDto);
                login.CreationDate = DateTime.Now;
                
                var createdLogin = await _loginData.CreateAsync(login);
                return MapToDto(createdLogin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el login del trabajador");
                throw;
            }
        }

        // Sobrescribimos para validar username único
        public override async Task<bool> UpdateAsync(WorkerLoginDto loginDto)
        {
            try
            {
                ValidateDto(loginDto);
                
                // Verificar si ya existe otro login con el mismo username
                if (await _loginData.ExistsUsernameAsync(loginDto.Username, loginDto.id))
                {
                    throw new InvalidOperationException($"Ya existe un login con el username '{loginDto.Username}'");
                }

                var login = MapToEntity(loginDto);
                return await _loginData.UpdateAsync(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el login del trabajador");
                return false;
            }
        }

        // Método adicional específico
        public async Task<WorkerLoginDto> GetByLoginIdAsync(int loginId)
        {
            try
            {
                var login = await _loginData.GetByLoginIdAsync(loginId);
                return login != null ? MapToDto(login) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el login del trabajador con LoginId {LoginId}", loginId);
                throw;
            }
        }

        protected override void ValidateDto(WorkerLoginDto dto)
        {
            if (dto == null)
                throw new ArgumentException("El objeto login no puede ser nulo");
                
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("El nombre de usuario es obligatorio");
        }

        protected override WorkerLogin MapToEntity(WorkerLoginDto dto)
        {
            return new WorkerLogin
            {
                id = dto.id,
                LoginId = dto.LoginId,
                WorkerId = dto.WorkerId,
                Username = dto.Username,
                Password = dto.Password,
                Status = dto.Status
            };
        }

        protected override WorkerLoginDto MapToDto(WorkerLogin entity)
        {
            return new WorkerLoginDto
            {
                id = entity.id,
                LoginId = entity.LoginId,
                WorkerId = entity.WorkerId,
                Username = entity.Username,
                Password = entity.Password,
                Status = entity.Status
            };
        }

        protected override IEnumerable<WorkerLoginDto> MapToDtoList(IEnumerable<WorkerLogin> entities)
        {
            return entities.Select(MapToDto).ToList();
        }
    }
}