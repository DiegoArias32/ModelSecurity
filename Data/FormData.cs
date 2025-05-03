using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos y el logger.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger{FormData}"/> para registrar eventos.</param>
        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo form en la base de datos.
        /// </summary>
        /// <param name="form">Instancia del form a crear.</param>
        /// <returns>El form creado.</returns>
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el form: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los forms almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de form.</returns>
       public async Task<IEnumerable<Form>> GetAllAsync()
{
    return await _context.Set<Form>()
        .Where(f => f.DeleteAt == null)
        .ToListAsync();
}


        /// <summary>
        /// Obtiene un form específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del form.</param>
        /// <returns>El form encontrado o null si no existe.</returns>
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener form con ID {FormId}", id);
                throw;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var form = await _context.Set<Form>().FindAsync(id);
        if (form == null)
            return false;

        _context.Set<Form>().Remove(form);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el form: {ErrorMessage}", ex.Message);
        return false;
    }
}

        /// <summary>
        /// Actualiza un form existente en la base de datos.
        /// </summary>
        /// <param name="form">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el form: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un form de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del form a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var form = await _context.Set<Form>().FindAsync(id);
                if (form == null)
                    return false;

                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el form: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
