using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entity.Contexts;
using Entity.Model;

namespace Data
{
    public class LoginData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginData> _logger;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos y el logger.
        /// </summary>
        /// <param name="context">Instancia del <see cref="ApplicationDbContext"/> para acceso a datos.</param>
        /// <param name="logger">Instancia del <see cref="ILogger"/> para registrar eventos.</param>
        public LoginData(ApplicationDbContext context, ILogger<LoginData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo registro de login en la base de datos.
        /// </summary>
        /// <param name="login">Instancia de <see cref="Login"/> a crear.</param>
        /// <returns>El login creado.</returns>
        public async Task<Login> CreateAsync(Login login)
        {
            try
            {
                // Validación opcional: evitar duplicados por nombre de usuario
                var existingLogin = await _context.Set<Login>()
                    .FirstOrDefaultAsync(l => l.Username == login.Username);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de crear login duplicado para usuario: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                await _context.Set<Login>().AddAsync(login);
                await _context.SaveChangesAsync();
                return login;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear login: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los registros de login.
        /// </summary>
        /// <returns>Lista de objetos <see cref="Login"/>.</returns>
        public async Task<IEnumerable<Login>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Login>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener logins: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un login por su ID.
        /// </summary>
        /// <param name="id">Identificador del login.</param>
        /// <returns>Objeto <see cref="Login"/> o null si no se encuentra.</returns>
        public async Task<Login?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Login>()
                    .FirstOrDefaultAsync(l => l.LoginId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al obtener login por ID {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var login = await _context.Set<Login>()
            .FirstOrDefaultAsync(l => l.LoginId == id);

        if (login == null)
            return false;

        _context.Set<Login>().Remove(login);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError("Error al eliminar permanentemente el login: {Message}", ex.Message);
        return false;
    }
}

        /// <summary>
        /// Actualiza un login existente.
        /// </summary>
        /// <param name="login">Instancia con los nuevos datos.</param>
        /// <returns>True si la operación fue exitosa, False si no.</returns>
        public async Task<bool> UpdateAsync(Login login)
        {
            try
            {
                // Validar duplicado (opcional)
                var existingLogin = await _context.Set<Login>()
                    .FirstOrDefaultAsync(l => l.Username == login.Username && l.LoginId != login.LoginId);

                if (existingLogin != null)
                {
                    _logger.LogWarning("Intento de actualizar login con username duplicado: {Username}", login.Username);
                    throw new InvalidOperationException($"Ya existe un login con el username '{login.Username}'");
                }

                _context.Set<Login>().Update(login);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar login: {Message}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un login por su ID.
        /// </summary>
        /// <param name="id">ID del login a eliminar.</param>
        /// <returns>True si se eliminó correctamente, False si no se encontró.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var login = await _context.Set<Login>()
                    .FirstOrDefaultAsync(l => l.LoginId == id);

                if (login == null)
                    return false;

                _context.Set<Login>().Remove(login);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar login: {Message}", ex.Message);
                return false;
            }
        }
    }
}
