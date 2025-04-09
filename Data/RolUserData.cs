using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;
using Entity.DTOs;

namespace Data
{
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolUserData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea una nueva relación Rol-User en la base de datos.
        /// </summary>
        /// <param name="rolUser">Instancia de <see cref="RolUser"/> a crear.</param>
        /// <returns>La instancia del Rol-User creada.</returns>
        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {
                rolUser.CreateAt = DateTime.UtcNow;
                rolUser.DeleteAt = DateTime.MinValue; // Indicamos que no está eliminado
                
                await _context.Set<RolUser>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la relación Rol-User: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones Rol-User activas almacenadas en la base de datos.
        /// </summary>
        /// <returns>Lista de relaciones Rol-User activas.</returns>
        public async Task<IEnumerable<RolUser>> GetAllAsync()
        {
            return await _context.Set<RolUser>()
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .Where(ru => ru.DeleteAt == DateTime.MinValue || ru.DeleteAt > DateTime.UtcNow) // Solo los activos
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todas las relaciones Rol-User, incluyendo las eliminadas.
        /// </summary>
        /// <returns>Lista completa de relaciones Rol-User.</returns>
        public async Task<IEnumerable<RolUser>> GetAllIncludingDeletedAsync()
        {
            return await _context.Set<RolUser>()
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una relación Rol-User específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la relación Rol-User.</param>
        /// <returns>La relación Rol-User encontrada o null si no existe.</returns>
        public async Task<RolUser?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolUser>()
                    .Include(ru => ru.User)
                    .Include(ru => ru.Rol)
                    .FirstOrDefaultAsync(ru => ru.Id == id && 
                                              (ru.DeleteAt == DateTime.MinValue || ru.DeleteAt > DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la relación Rol-User con ID {RolUserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones de roles para un usuario específico.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Lista de relaciones Rol-User del usuario.</returns>
        public async Task<IEnumerable<RolUser>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Set<RolUser>()
                    .Include(ru => ru.Rol)
                    .Where(ru => ru.UserId == userId && 
                                (ru.DeleteAt == DateTime.MinValue || ru.DeleteAt > DateTime.UtcNow))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User para el usuario con ID {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las relaciones de usuarios para un rol específico.
        /// </summary>
        /// <param name="rolId">ID del rol.</param>
        /// <returns>Lista de relaciones Rol-User del rol.</returns>
        public async Task<IEnumerable<RolUser>> GetByRolIdAsync(int rolId)
        {
            try
            {
                return await _context.Set<RolUser>()
                    .Include(ru => ru.User)
                    .Where(ru => ru.RolId == rolId && 
                                (ru.DeleteAt == DateTime.MinValue || ru.DeleteAt > DateTime.UtcNow))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las relaciones Rol-User para el rol con ID {RolId}", rolId);
                throw;
            }
        }

        /// <summary>
        /// Actualiza una relación Rol-User existente en la base de datos.
        /// </summary>
        /// <param name="rolUser">Objeto con la información actualizada.</param>
        /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolUser rolUser)
        {
            try
            {
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la relación Rol-User: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina lógicamente una relación Rol-User de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la relación Rol-User a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);
                if (rolUser == null)
                    return false;

                // Implementamos soft delete
                rolUser.DeleteAt = DateTime.UtcNow;
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la relación Rol-User: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina permanentemente una relación Rol-User de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único de la relación Rol-User a eliminar permanentemente.</param>
        /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> PermanentDeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);
                if (rolUser == null)
                    return false;

                _context.Set<RolUser>().Remove(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente la relación Rol-User: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Convierte la entidad RolUser en su equivalente DTO (Data Transfer Object).
        /// </summary>
        /// <param name="rolUser">Entidad RolUser.</param>
        /// <returns>El DTO RolUserDto correspondiente.</returns>
        public RolUserDto ToDto(RolUser rolUser)
        {
            return new RolUserDto
            {
                Id = rolUser.Id,
                UserId = rolUser.UserId,
                RolId = rolUser.RolId,
            };
        }

        /// <summary>
        /// Convierte una lista de entidades RolUser en su equivalente DTO.
        /// </summary>
        /// <param name="rolUsers">Lista de entidades RolUser.</param>
        /// <returns>Lista de DTOs RolUserDto correspondientes.</returns>
        public IEnumerable<RolUserDto> ToDtoList(IEnumerable<RolUser> rolUsers)
        {
            return rolUsers.Select(ru => ToDto(ru));
        }
    }
}