using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;
using Entity;
using Entity.Enum;

namespace Data{

public class RolFormPermissionData
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolFormPermissionData> _logger;

    public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea un nuevo permiso de formulario para un rol.
    /// </summary>
    /// <param name="rolFormPermission">Instancia de RolFormPermission a crear.</param>
    /// <returns>El permiso de formulario creado.</returns>
    public async Task<RolFormPermission> CreateAsync(RolFormPermission rolFormPermission)
    {
        try
        {
            rolFormPermission.CreateAt = DateTime.UtcNow;
            await _context.Set<RolFormPermission>().AddAsync(rolFormPermission);
            await _context.SaveChangesAsync();
            return rolFormPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el permiso de formulario para rol: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de formularios para roles.
    /// </summary>
    /// <returns>Lista de permisos de formularios para roles.</returns>
    public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
    {
        return await _context.Set<RolFormPermission>()
            .Include(rfp => rfp.Rol)
            .Include(rfp => rfp.Form)
            .Include(rfp => rfp.Permission)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un permiso de formulario para rol por su identificador.
    /// </summary>
    /// <param name="id">Identificador del permiso.</param>
    /// <returns>El permiso de formulario para rol encontrado.</returns>
    public async Task<RolFormPermission?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<RolFormPermission>()
                .Include(rfp => rfp.Rol)
                .Include(rfp => rfp.Form)
                .Include(rfp => rfp.Permission)
                .FirstOrDefaultAsync(rfp => rfp.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permiso de formulario para rol con ID {PermissionId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de formularios para un rol específico.
    /// </summary>
    /// <param name="rolId">Identificador del rol.</param>
    /// <returns>Lista de permisos de formularios del rol.</returns>
    public async Task<IEnumerable<RolFormPermission>> GetByRolIdAsync(int rolId)
    {
        try
        {
            return await _context.Set<RolFormPermission>()
                .Where(rfp => rfp.RolId == rolId)
                .Include(rfp => rfp.Rol)
                .Include(rfp => rfp.Form)
                .Include(rfp => rfp.Permission)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos de formularios para rol con ID {RolId}", rolId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los permisos de formularios para un formulario específico.
    /// </summary>
    /// <param name="formId">Identificador del formulario.</param>
    /// <returns>Lista de permisos de formularios para el formulario.</returns>
    public async Task<IEnumerable<RolFormPermission>> GetByFormIdAsync(int formId)
    {
        try
        {
            return await _context.Set<RolFormPermission>()
                .Where(rfp => rfp.FormId == formId)
                .Include(rfp => rfp.Rol)
                .Include(rfp => rfp.Form)
                .Include(rfp => rfp.Permission)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos de formularios para formulario con ID {FormId}", formId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un permiso de formulario para rol existente.
    /// </summary>
    /// <param name="rolFormPermission">Objeto con la información actualizada.</param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(RolFormPermission rolFormPermission)
    {
        try
        {
            _context.Set<RolFormPermission>().Update(rolFormPermission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el permiso de formulario para rol: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina un permiso de formulario para rol.
    /// </summary>
    /// <param name="id">Identificador del permiso a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var rolFormPermission = await _context.Set<RolFormPermission>().FindAsync(id);
            if (rolFormPermission == null)
                return false;

            rolFormPermission.DeleteAt = DateTime.UtcNow;
            _context.Set<RolFormPermission>().Update(rolFormPermission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el permiso de formulario para rol: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina permanentemente un permiso de formulario para rol.
    /// </summary>
    /// <param name="id">Identificador del permiso a eliminar permanentemente.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> PermanentDeleteAsync(int id)
    {
        try
        {
            var rolFormPermission = await _context.Set<RolFormPermission>().FindAsync(id);
            if (rolFormPermission == null)
                return false;

            _context.Set<RolFormPermission>().Remove(rolFormPermission);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar permanentemente el permiso de formulario para rol: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}
}