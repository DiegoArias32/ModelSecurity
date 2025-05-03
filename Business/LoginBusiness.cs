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
    public class LoginBusiness
    {
        private readonly LoginData _loginData;
        private readonly ILogger<LoginBusiness> _logger;

        public LoginBusiness(LoginData loginData, ILogger<LoginBusiness> logger)
        {
            _loginData = loginData ?? throw new ArgumentNullException(nameof(loginData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginDto> CreateAsync(LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    _logger.LogWarning("Username o Password no pueden estar vacíos.");
                    throw new ArgumentException("Username o Password no pueden estar vacíos.");
                }

                var login = MapToEntity(loginDto);
                var createdLogin = await _loginData.CreateAsync(login);
                return MapToDto(createdLogin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el login.");
                throw;
            }
        }

        public async Task<IEnumerable<LoginDto>> GetAllAsync()
        {
            try
            {
                var logins = await _loginData.GetAllAsync();
                return MapToDtoList(logins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los logins.");
                throw;
            }
        }

        public async Task<LoginDto?> GetByIdAsync(int id)
        {
            try
            {
                var login = await _loginData.GetByIdAsync(id);
                return login != null ? MapToDto(login) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el login con ID {LoginId}.", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(LoginDto loginDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    _logger.LogWarning("Username o Password no pueden estar vacíos.");
                    throw new ArgumentException("Username o Password no pueden estar vacíos.");
                }

                var login = await _loginData.GetByIdAsync(loginDto.LoginId);
                if (login == null)
                {
                    _logger.LogWarning("Login no encontrado para actualizar.");
                    return false;
                }

                MapToEntity(loginDto, login);
                return await _loginData.UpdateAsync(login);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el login con ID {LoginId}.", loginDto.LoginId);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var login = await _loginData.GetByIdAsync(id);
        if (login == null)
        {
            _logger.LogWarning("Login no encontrado para eliminación permanente.");
            return false;
        }

        return await _loginData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el login con ID {LoginId}.", id);
        return false;
    }
}


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var login = await _loginData.GetByIdAsync(id);
                if (login == null)
                {
                    _logger.LogWarning("Login no encontrado para eliminar.");
                    return false;
                }

                return await _loginData.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el login con ID {LoginId}.", id);
                return false;
            }
        }

        private LoginDto MapToDto(Login login)
        {
            return new LoginDto
            {
                LoginId = login.LoginId,
                Username = login.Username,
                Password = login.Password,
            };
        }

        private Login MapToEntity(LoginDto loginDto)
        {
            return new Login
            {
                LoginId = loginDto.LoginId,
                Username = loginDto.Username,
                Password = loginDto.Password
            };
        }

        private void MapToEntity(LoginDto loginDto, Login login)
        {
            login.Username = loginDto.Username;
            login.Password = loginDto.Password;
        }

        private IEnumerable<LoginDto> MapToDtoList(IEnumerable<Login> logins)
        {
            return logins.Select(login => MapToDto(login)).ToList();
        }
    }
}
