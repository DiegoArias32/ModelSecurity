using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

public class RolUserData(ApplicationDbContext context, ILogger logger)
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public DateTime CreateAt { get; private set; }
    public object User { get; private set; }
    public object Rol { get; private set; }
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public DateTime DeleteAt { get; private set; }


    /// <summary>
    /// Crea una nueva asignación de rol a usuario.
    /// </summary>
    /// <param name="rolUser">Instancia de RolUser a crear.</param>
    /// <returns>La asignación de rol creada.</returns>
    public async Task<RolUserData> CreateAsync(RolUserData rolUser)
    {
        try
        {
            rolUser.CreateAt = DateTime.UtcNow;
            await _context.Set<RolUserData>().AddAsync(rolUser);
            await _context.SaveChangesAsync();
            return rolUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la asignación de rol de usuario: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de roles de usuarios.
    /// </summary>
    /// <returns>Lista de asignaciones de roles de usuarios.</returns>
    public async Task<IEnumerable<RolUserData>> GetAllAsync()
    {
        return await _context.Set<RolUserData>()
            .Include(ru => ru.User)
            .Include(ru => ru.Rol)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene una asignación de rol de usuario por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la asignación.</param>
    /// <returns>La asignación de rol de usuario encontrada.</returns>
    public async Task<RolUserData?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<RolUserData>()
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .FirstOrDefaultAsync(ru => ru.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asignación de rol de usuario con ID {RolUserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las asignaciones de roles para un usuario específico.
    /// </summary>
    /// <param name="userId">Identificador del usuario.</param>
    /// <returns>Lista de asignaciones de roles del usuario.</returns>
    public async Task<IEnumerable<RolUserData>> GetByUserIdAsync(int userId)
    {
        try
        {
            return await _context.Set<RolUserData>()
                .Where(ru => ru.UserId == userId)
                .Include(ru => ru.User)
                .Include(ru => ru.Rol)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener roles de usuario para ID {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una asignación de rol de usuario existente.
    /// </summary>
    /// <param name="rolUser">Objeto con la información actualizada.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(RolUserData rolUser)
    {
        try
        {
            _context.Set<RolUserData>().Update(rolUser);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la asignación de rol de usuario: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina una asignación de rol de usuario.
    /// </summary>
    /// <param name="id">Identificador de la asignación a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var rolUser = await _context.Set<RolUserData>().FindAsync(id);
            if (rolUser == null)
                return false;

            rolUser.DeleteAt = DateTime.UtcNow;
            _context.Set<RolUserData>().Update(rolUser);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la asignación de rol de usuario: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina permanentemente una asignación de rol de usuario.
    /// </summary>
    /// <param name="id">Identificador de la asignación a eliminar permanentemente.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> PermanentDeleteAsync(int id)
    {
        try
        {
            var rolUser = await _context.Set<RolUserData>().FindAsync(id);
            if (rolUser == null)
                return false;

            _context.Set<RolUserData>().Remove(rolUser);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente la asignación de rol de usuario: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}