using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public PermissionData(ApplicationDbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo permiso en la base de datos.
        /// </summary>
        /// <param name="permission">Instancia del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los permisos almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de permisos.</returns>
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Permission>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un permiso específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del permiso.</param>
        /// <returns>El permiso encontrado o null si no existe.</returns>
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID {PermissionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un permiso existente en la base de datos.
        /// </summary>
        /// <param name="permission">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del permiso a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var permission = await _context.Set<Permission>().FindAsync(id);
                if (permission == null)
                    return false;

                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
