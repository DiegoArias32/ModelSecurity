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
    public class UserBusiness : GenericBusiness<UserDto, User>
    {
        private readonly UserData _userData;

        public UserBusiness(UserData userData, ILogger<UserBusiness> logger)
            : base(userData, logger)
        {
            _userData = userData;
        }

        // Sobrescribimos la creación para validar email único
        public override async Task<UserDto> CreateAsync(UserDto userDto)
        {
            try
            {
                ValidateDto(userDto);
                
                // Verificar si ya existe un email
                if (await _userData.ExistsEmailAsync(userDto.Email))
                {
                    throw new InvalidOperationException($"Ya existe un usuario con el email '{userDto.Email}'");
                }

                var user = MapToEntity(userDto);
                var createdUser = await _userData.CreateAsync(user);
                return MapToDto(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario");
                throw;
            }
        }

        // Sobrescribimos para validar email único
        public override async Task<bool> UpdateAsync(UserDto userDto)
        {
            try
            {
                ValidateDto(userDto);
                
                // Verificar si ya existe otro usuario con el mismo email
                if (await _userData.ExistsEmailAsync(userDto.Email, userDto.Id))
                {
                    throw new InvalidOperationException($"Ya existe un usuario con el email '{userDto.Email}'");
                }

                var user = MapToEntity(userDto);
                return await _userData.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario");
                return false;
            }
        }

        protected override void ValidateDto(UserDto dto)
        {
            if (dto == null)
                throw new ArgumentException("El objeto usuario no puede ser nulo");
                
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("El nombre del usuario es obligatorio");
                
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("El email del usuario es obligatorio");
        }

        protected override User MapToEntity(UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                CreateAt = dto.Id == 0 ? DateTime.UtcNow : DateTime.UtcNow, // Nueva entidad o actualización
                DeleteAt = null
            };
        }

        protected override UserDto MapToDto(User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Password = entity.Password, // Nota: por seguridad, considerar no devolver la contraseña
            };
        }

        protected override IEnumerable<UserDto> MapToDtoList(IEnumerable<User> entities)
        {
            return entities.Select(MapToDto).ToList();
        }
    }
}