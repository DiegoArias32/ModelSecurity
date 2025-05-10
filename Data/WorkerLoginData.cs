// Data/WorkerLoginData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class WorkerLoginData : BaseData<WorkerLogin>
    {
        public WorkerLoginData(IRepository<WorkerLogin> repository, ILogger<WorkerLoginData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir CreateAsync para validar username único
        public override async Task<WorkerLogin> CreateAsync(WorkerLogin login)
        {
            try
            {
                // Validar duplicado por username
                var allWorkerLogins = await base.GetAllAsync();
                var existingLogin = allWorkerLogins.FirstOrDefault(l => l.Username == login.Username);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de crear login duplicado para Worker con username: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                login.CreationDate = DateTime.Now;

                return await base.CreateAsync(login);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear el WorkerLogin: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<WorkerLogin> GetByLoginIdAsync(int loginId)
        {
            try
            {
                var allWorkerLogins = await base.GetAllAsync();
                return allWorkerLogins.FirstOrDefault(w => w.LoginId == loginId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener WorkerLogin con LoginId {LoginId}: {Message}", loginId, ex.Message);
                throw;
            }
        }

        // Sobrescribir UpdateAsync para validar username único
        public override async Task<bool> UpdateAsync(WorkerLogin login)
        {
            try
            {
                // Validar duplicado por username
                var allWorkerLogins = await base.GetAllAsync();
                var existing = allWorkerLogins.FirstOrDefault(l => l.Username == login.Username && l.id != login.id);

                if (existing != null)
                {
                    _logger.LogWarning("Intento de actualizar WorkerLogin con username duplicado: {Username}", login.Username);
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
                _logger.LogError("Error al actualizar WorkerLogin: {Message}", ex.Message);
                return false;
            }
        }
    }
}