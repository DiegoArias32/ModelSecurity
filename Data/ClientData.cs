using Entity.Contexts;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class ClientData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        // Constructor que recibe el contexto de la base de datos y el logger
        public ClientData(ApplicationDbContext context, ILogger<ClientData> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Crea un nuevo cliente en la base de datos.
        /// </summary>
        /// <param name="client">Instancia de Client a crear.</param>
        /// <returns>El cliente creado.</returns>
        public async Task<Client> CreateAsync(Client client)
        {
            try
            {
                await _context.Set<Client>().AddAsync(client);
                await _context.SaveChangesAsync();
                return client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los clientes de la base de datos.
        /// </summary>
        /// <returns>Lista de todos los clientes.</returns>
        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            try
            {
                return await _context.Set<Client>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los clientes: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>El cliente encontrado o null si no existe.</returns>
        public async Task<Client?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Client>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID {ClientId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un cliente en la base de datos.
        /// </summary>
        /// <param name="client">Cliente con los datos actualizados.</param>
        /// <returns>True si la actualización fue exitosa, False si no lo fue.</returns>
        public async Task<bool> UpdateAsync(Client client)
{
    try
    {
        var existingClient = await _context.Set<Client>().FindAsync(client.ClientId);
        if (existingClient == null)
        {
            _logger.LogWarning("No se encontró el cliente con ID {ClientId} para actualizar.", client.ClientId);
            return false;
        }

        // Actualizar valores evitando conflictos de tracking
        _context.Entry(existingClient).CurrentValues.SetValues(client);
        await _context.SaveChangesAsync();

        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar el cliente con ID {ClientId}: {ErrorMessage}", client.ClientId, ex.Message);
        return false;
    }
}


        /// <summary>
        /// Elimina un cliente por su ID.
        /// </summary>
        /// <param name="id">ID del cliente a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa, False si no lo fue.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var client = await _context.Set<Client>().FindAsync(id);
                if (client == null) return false;

                _context.Set<Client>().Remove(client);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID {ClientId}: {ErrorMessage}", id, ex.Message);
                return false;
            }
        }

        public async Task<bool> PermanentDeleteAsync(int id)
{
    try
    {
        var client = await _context.Set<Client>().FindAsync(id);
        if (client == null) return false;

        _context.Set<Client>().Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el cliente con ID {ClientId}: {ErrorMessage}", id, ex.Message);
        return false;
    }
}

    }
}
