using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    /// <summary>
    /// Capa de acceso a datos para operaciones de Roles.
    /// Proporciona métodos para operaciones CRUD en entidades Rol.
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolData> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase RolData.
        /// </summary>
        /// <param name="context">Contexto de base de datos para acceder a los datos de roles</param>
        /// <param name="logger">Logger para registrar operaciones y errores</param>
        public RolData(ApplicationDbContext context, ILogger<RolData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo rol en la base de datos.
        /// </summary>
        /// <param name="rol">Entidad Rol a crear</param>
        /// <returns>El rol creado con su ID asignado</returns>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe un rol con el mismo nombre</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación de base de datos</exception>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                // Verificar si ya existe un rol con el mismo nombre
                var existingRole = await _context.Set<Rol>().FirstOrDefaultAsync(r => r.Name == rol.Name);
                if (existingRole != null)
                {
                    _logger.LogWarning("Intento de crear rol duplicado: {RolName}", rol.Name);
                    throw new InvalidOperationException($"Ya existe un rol con el nombre '{rol.Name}'");
                }

                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (InvalidOperationException)
            {
                // Re-lanzar sin registro adicional para errores de validación conocidos
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear rol: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los roles activos de la base de datos.
        /// </summary>
        /// <returns>Colección de roles no eliminados</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta</exception>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            try
            {
                // Solo retornar registros donde DeleteAt es null (no eliminados)
                return await _context.Set<Rol>()
                    .Where(r => r.DeleteAt == null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw;
            }
        }

public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var rol = await _context.Set<Rol>().FindAsync(id);
        if (rol == null)
        {
            return false;
        }

        // Eliminación física
        _context.Remove(rol);
        await _context.SaveChangesAsync();

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el rol con ID {RolId}: {ErrorMessage}", id, ex.Message);
        throw;
    }
}


        /// <summary>
        /// Busca un rol por su ID.
        /// </summary>
        /// <param name="id">ID del rol a buscar</param>
        /// <returns>El rol encontrado o null si no existe o está eliminado</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta</exception>
        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                // Retornar null si el registro está eliminado
                var rol = await _context.Set<Rol>().FindAsync(id);
                return rol?.DeleteAt == null ? rol : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID {RolId}", id);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos.
        /// </summary>
        /// <param name="rol">Entidad Rol con los datos actualizados</param>
        /// <returns>El rol actualizado o null si no se encuentra</returns>
        /// <exception cref="InvalidOperationException">Se lanza si ya existe otro rol con el mismo nombre</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la actualización</exception>
        public async Task<Rol> UpdateAsync(Rol rol)
        {
            try
            {
                // Verificar si el rol existe
                var rolExistente = await _context.Set<Rol>().FindAsync(rol.Id);
                if (rolExistente == null)
                {
                    return null;
                }

                // Verificar si existe otro rol con el mismo nombre (excluyendo el rol actual)
                var duplicateRole = await _context.Set<Rol>().FirstOrDefaultAsync(r => 
                    r.Name == rol.Name && r.Id != rol.Id);
                    
                if (duplicateRole != null)
                {
                    throw new InvalidOperationException($"Ya existe un rol con el nombre '{rol.Name}'");
                }

                // Actualizar propiedades
                _context.Entry(rolExistente).CurrentValues.SetValues(rol);
                await _context.SaveChangesAsync();
                
                return rolExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID {RolId}: {ErrorMessage}", rol.Id, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Elimina lógicamente un rol (marcándolo como eliminado).
        /// </summary>
        /// <param name="id">ID del rol a eliminar</param>
        /// <returns>True si se eliminó correctamente, False si no se encontró el rol</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la eliminación</exception>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                {
                    return false;
                }

                // Marcar como eliminado
                rol.DeleteAt = DateTime.Now;
                _context.Update(rol);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {RolId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }
    }
}