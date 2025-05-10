// Data/UserData.cs
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Data
{
    public class UserData : BaseData<User>
    {
        public UserData(IRepository<User> repository, ILogger<UserData> logger)
            : base(repository, logger)
        {
        }

        // Sobrescribir GetAllAsync para incluir solo users activos
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                var users = await base.GetAllAsync();
                return users.Where(u => u.DeleteAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        // Sobrescribir CreateAsync para validar email único
        public override async Task<User> CreateAsync(User user)
        {
            try
            {
                // Verificar si ya existe un usuario con el mismo email
                var allUsers = await base.GetAllAsync();
                var existingUser = allUsers.FirstOrDefault(u => u.Email == user.Email && u.DeleteAt == null);
                    
                if (existingUser != null)
                {
                    _logger.LogWarning("Intento de crear usuario con email duplicado: {Email}", user.Email);
                    throw new InvalidOperationException($"Ya existe un usuario con el email '{user.Email}'");
                }

                // Establecer la fecha de creación
                user.CreateAt = DateTime.Now;
                
                return await base.CreateAsync(user);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al crear el usuario: {Message}", ex.Message);
                throw;
            }
        }

        // Sobrescribir UpdateAsync para validar email único
        public override async Task<bool> UpdateAsync(User user)
        {
            try
            {
                // Verificar si ya existe otro usuario con el mismo email
                var allUsers = await base.GetAllAsync();
                var existingUser = allUsers.FirstOrDefault(u => u.Email == user.Email && u.Id != user.Id && u.DeleteAt == null);
                    
                if (existingUser != null)
                {
                    _logger.LogWarning("Intento de actualizar usuario con email duplicado: {Email}", user.Email);
                    throw new InvalidOperationException($"Ya existe un usuario con el email '{user.Email}'");
                }
                
                return await base.UpdateAsync(user);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al actualizar el usuario: {Message}", ex.Message);
                return false;
            }
        }

        // Sobrescribir DeleteAsync para implementar borrado lógico
        public override async Task<bool> DeleteAsync(object id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                if (user == null)
                    return false;

                // Soft delete - actualizar fecha de eliminación
                user.DeleteAt = DateTime.Now;
                
                return await UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al eliminar el usuario: {Message}", ex.Message);
                return false;
            }
        }
    }
}