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
    public class LoginBusiness : GenericBusiness<LoginDto, Login>
    {
        public LoginBusiness(IGenericData<Login> data, ILogger<LoginBusiness> logger)
            : base(data, logger)
        {
        }

        // Sobrescribimos el método de validación según las reglas específicas de Login
        protected override void ValidateDto(LoginDto dto)
        {
            if (dto == null)
                throw new ArgumentException("El objeto login no puede ser nulo.");
                
            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío.");
                
            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("La contraseña no puede estar vacía.");
        }

        // Implementamos los mapeos requeridos por la clase base
        protected override Login MapToEntity(LoginDto dto)
        {
            return new Login
            {
                LoginId = dto.LoginId,
                Username = dto.Username,
                Password = dto.Password
            };
        }

        protected override LoginDto MapToDto(Login entity)
        {
            return new LoginDto
            {
                LoginId = entity.LoginId,
                Username = entity.Username,
                Password = entity.Password,
            };
        }

        protected override IEnumerable<LoginDto> MapToDtoList(IEnumerable<Login> entities)
        {
            return entities.Select(MapToDto).ToList();
        }

        // Si se necesitan métodos específicos para Login, se pueden añadir aquí
    }
}