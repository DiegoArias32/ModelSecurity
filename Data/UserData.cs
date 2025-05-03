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
        // Verificar si ya existe un usuario con el mismo email
        var existingUser = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == user.Email && u.DeleteAt == default);
            
        if (existingUser != null)
        {
            _logger.LogWarning("Intento de crear usuario con email duplicado: {Email}", user.Email);
            throw new InvalidOperationException($"Ya existe un usuario con el email '{user.Email}'");
        }

        // Establecer la fecha de creación
        user.CreateAt = DateTime.Now;
        // Asegurarse que DeleteAt tiene el valor por defecto (DateTime.MinValue)
        user.DeleteAt = default;
        
        await _context.Set<User>().AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }
    catch (InvalidOperationException)
    {
        // Re-lanzar sin logging adicional para errores conocidos
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al crear el usuario: {Message}", ex.Message);
        throw;
    }
}



        /// <summary>
        /// Obtiene todos los users almacenados en la base de datos.
        /// </summary>
        /// <returns>Lista de user.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
{
    try 
    {
        // Solo retornar usuarios no eliminados (DeleteAt = default/MinValue)
        return await _context.Set<User>()
            .Where(u => u.DeleteAt == default)
            .ToListAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al obtener todos los usuarios: {Message}", ex.Message);
        throw;
    }
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
        return await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeleteAt == default);
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al obtener usuario con ID {UserId}", id);
        throw;
    }
}

public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null)
            return false;

        // Eliminación física
        _context.Set<User>().Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al eliminar permanentemente el usuario: {Message}", ex.Message);
        return false;
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
        // Verificar si ya existe otro usuario con el mismo email
        var existingUser = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != user.Id && u.DeleteAt == default);
            
        if (existingUser != null)
        {
            _logger.LogWarning("Intento de actualizar usuario con email duplicado: {Email}", user.Email);
            throw new InvalidOperationException($"Ya existe un usuario con el email '{user.Email}'");
        }
        
        _context.Set<User>().Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (InvalidOperationException)
    {
        // Re-lanzar sin logging adicional para errores conocidos
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al actualizar el usuario: {Message}", ex.Message);
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
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeleteAt == default);
            
        if (user == null)
            return false;

        // Soft delete - actualizar fecha de eliminación
        user.DeleteAt = DateTime.Now;
        _context.Set<User>().Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al eliminar el usuario: {Message}", ex.Message);
        return false;
    }
}
    }
}
