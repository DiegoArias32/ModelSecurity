using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity;
using Entity.Model;

namespace Data
{
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo user en la base de datos.
        /// </summary>
        /// <param name="user">Instancia del user a crear. </param>
        /// <returns>El user creado.</returns>
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el user: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los users almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de user.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        /// <summary>
        /// Obtiene un user específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador del user.</param>
        /// <returns>El user encontrado o null si no existe.</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener user con ID {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un user existente en la base de datos.
        /// </summary>
        /// <param name="user">Objeto con la información actualizada. </param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el user: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un user de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del user a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el user: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
