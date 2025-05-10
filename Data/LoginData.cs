// Data/LoginData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class LoginData : BaseData<Login>
    {
        public LoginData(IRepository<Login> repository, ILogger<LoginData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir CreateAsync para validar username único
        public override async Task<Login> CreateAsync(Login login)
        {
            try
            {
                // Validación opcional: evitar duplicados por nombre de usuario
                var allLogins = await base.GetAllAsync();
                var existingLogin = allLogins.FirstOrDefault(l => l.Username == login.Username);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de crear login duplicado para usuario: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                return await base.CreateAsync(login);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear login: {Message}", ex.Message);
                throw;
            }
        }

        // Sobrescribir UpdateAsync para validar username único
        public override async Task<bool> UpdateAsync(Login login)
        {
            try
            {
                // Validar duplicado (opcional)
                var allLogins = await base.GetAllAsync();
                var existingLogin = allLogins.FirstOrDefault(l => l.Username == login.Username && l.LoginId != login.LoginId);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de actualizar login con username duplicado: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                return await base.UpdateAsync(login);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar login: {Message}", ex.Message);
                return false;
            }
        }
    }
}