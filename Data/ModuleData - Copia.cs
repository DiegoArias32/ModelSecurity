using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

public class PersonData
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor que recibe el contexto de base de datos.
    /// </summary>
    /// <param name="context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
    /// <param name="logger">Instancia de <see cref="ILogger"/> para registrar eventos.</param>
    public PersonData(ApplicationDbContext context, ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Crea un nuevo person en la base de datos.
    /// </summary>
    /// <param name="person">Instancia del person a crear. </param>
    /// <returns>El person creado.</returns>
    public async Task<Person> CreateAsync(Person person)
    {
        try
        {
            await _context.Set<Person>().AddAsync(person);
            await _context.SaveChangesAsync();
            return person;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el person: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los persons almacenados en la base de datos.
    /// </summary>
    /// <returns>Lista de person.</returns>
    public async Task<IEnumerable<Person>> GetAllAsync()
    {
        return await _context.Set<Person>().ToListAsync();
    }

    /// <summary>
    /// Obtiene un person específico por su identificador.
    /// </summary>
    /// <param name="id">Identificador del person.</param>
    /// <returns>El person encontrado o null si no existe.</returns>
    public async Task<Person?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Set<Person>().FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener person con ID {PersonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un person existente en la base de datos.
    /// </summary>
    /// <param name="person">Objeto con la información actualizada. </param>
    /// <returns>True si la operación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> UpdateAsync(Person person)
    {
        try
        {
            _context.Set<Person>().Update(person);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el person: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Elimina un person de la base de datos.
    /// </summary>
    /// <param name="id">Identificador único del person a eliminar.</param>
    /// <returns>True si la eliminación fue exitosa, False en caso contrario.</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var person = await _context.Set<Person>().FindAsync(id);
            if (person == null)
                return false;

            _context.Set<Person>().Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el person: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}