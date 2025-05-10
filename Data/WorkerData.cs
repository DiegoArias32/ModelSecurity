// Data/WorkerData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class WorkerData : BaseData<Worker>
    {
        public WorkerData(IRepository<Worker> repository, ILogger<WorkerData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir CreateAsync para validar documento de identidad único
        public override async Task<Worker> CreateAsync(Worker worker)
        {
            try
            {
                // Validar que no se repita el documento de identidad
                var allWorkers = await base.GetAllAsync();
                var exists = allWorkers.FirstOrDefault(w => w.IdentityDocument == worker.IdentityDocument);
                
                if (exists != null)
                {
                    _logger.LogWarning("Documento de identidad ya registrado: {Identity}", worker.IdentityDocument);
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{worker.IdentityDocument}'");
                }

                return await base.CreateAsync(worker);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear trabajador: {Message}", ex.Message);
                throw;
            }
        }

        // Sobrescribir UpdateAsync para validar documento de identidad único
        public override async Task<bool> UpdateAsync(Worker worker)
        {
            try
            {
                // Validar que no se repita el documento si cambia
                var allWorkers = await base.GetAllAsync();
                var duplicate = allWorkers.FirstOrDefault(w => w.IdentityDocument == worker.IdentityDocument && w.WorkerId != worker.WorkerId);

                if (duplicate != null)
                {
                    _logger.LogWarning("Intento de duplicar documento: {Doc}", worker.IdentityDocument);
                    throw new InvalidOperationException($"Ya existe un trabajador con el documento '{worker.IdentityDocument}'");
                }

                return await base.UpdateAsync(worker);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar trabajador: {Message}", ex.Message);
                return false;
            }
        }
    }
}