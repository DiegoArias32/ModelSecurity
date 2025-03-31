using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity;
using Entity.Contexts;
using Entity.Model;

public class ModuleData
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos.
    /// </summary>
    /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
    /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
    public ModuleData(ApplicationDbContext context, ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea un nuevo module en la base de datos.
    /// </summary>
    /// <param name="module">Instancia del module a crear. </param>
    /// <returns>El module creado.</returns>
    public async Task<Module> CreateAsync(Module module)
    {
        try
        {
            await _context.Set<Module>().AddAsync(module);
            await _context.SaveChangesAsync();
            return module;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el module: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los modules almacenados en la base de datos.
    /// </summary>
    /// <returns>Lista de module.</returns>
    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await _context.Set<Module>().ToListAsync();
    }

    /// <summary>
    /// Obtiene un module específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador del module.</param>
    /// <returns>El module encontrado o null si no existe.</returns>
    public async Task<Module?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<Module>().FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener module con ID {ModuleId}", id);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un module existente en la base de datos.
    /// </summary>
    /// <param name="module">Objeto con la información actualizada. </param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(Module module)
    {
        try
        {
            _context.Set<Module>().Update(module);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el module: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina un module de la base de datos.
    /// </summary>
    /// <param name="id">Identificador único del module a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var module = await _context.Set<Module>().FindAsync(id);
            if (module == null)
                return false;

            _context.Set<Module>().Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el module: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}