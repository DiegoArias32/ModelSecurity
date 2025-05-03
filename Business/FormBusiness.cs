using Data;
using Entity.Contexts;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Business
{
    public class FormBusiness
    {
        private readonly FormData _formData; // Clase para manejar operaciones de base de datos
        private readonly ILogger _logger;

        // Constructor que recibe la instancia de FormData y ILogger
        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData ?? throw new ArgumentNullException(nameof(formData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los formularios.
        /// </summary>
        /// <returns>Lista de formularios en formato DTO.</returns>
        public async Task<IEnumerable<FormDto>> GetAllFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                return MapToDTOList(forms); // Usamos el método MapToDTOList aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al obtener los formularios", ex);
            }
        }

        /// <summary>
        /// Obtiene un formulario por su ID.
        /// </summary>
        /// <param name="id">ID del formulario.</param>
        /// <returns>Formulario en formato DTO.</returns>
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un formulario con ID inválido: {FormId}", id);
                throw new ValidationException("id", "El ID del formulario debe ser mayor que cero.");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("Formulario con ID {FormId} no encontrado.", id);
                    throw new EntityNotFoundException("Formulario", id);
                }

                return MapToDTO(form); // Usamos el método MapToDTO aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al obtener el formulario con ID {id}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo formulario.
        /// </summary>
        /// <param name="formDto">DTO con la información del formulario.</param>
        /// <returns>El formulario creado en formato DTO.</returns>
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                ValidateForm(formDto); // Validar los datos del formulario

                var form = MapToEntity(formDto); // Usamos el método MapToEntity aquí
                form.CreateAt = DateTime.UtcNow; // Establecer la fecha de creación

                var createdForm = await _formData.CreateAsync(form);

                return MapToDTO(createdForm); // Usamos el método MapToDTO aquí
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el formulario: {FormName}", formDto.Name);
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        public async Task<bool> PermanentDeleteFormAsync(int id)
{
    try
    {
        var form = await _formData.GetByIdAsync(id);
        if (form == null)
        {
            _logger.LogWarning("Formulario con ID {FormId} no encontrado para eliminación permanente.", id);
            throw new EntityNotFoundException("Formulario", id);
        }

        return await _formData.PermanentDeleteAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar permanentemente el formulario con ID {FormId}", id);
        throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el formulario", ex);
    }
}

        /// <summary>
        /// Actualiza un formulario existente.
        /// </summary>
        /// <param name="formDto">DTO con la información actualizada del formulario.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        public async Task<bool> UpdateFormAsync(FormDto formDto)
        {
            try
            {
                var existingForm = await _formData.GetByIdAsync(formDto.Id);
                if (existingForm == null)
                {
                    _logger.LogWarning("Formulario con ID {FormId} no encontrado.", formDto.Id);
                    throw new EntityNotFoundException("Formulario", formDto.Id);
                }

                existingForm.Name = formDto.Name;
                existingForm.Code = formDto.Code;
                existingForm.Active = formDto.Active;

                var updated = await _formData.UpdateAsync(existingForm);

                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario con ID {FormId}", formDto.Id);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el formulario", ex);
            }
        }

        /// <summary>
        /// Elimina un formulario.
        /// </summary>
        /// <param name="id">ID del formulario a eliminar.</param>
        /// <returns>True si la eliminación fue exitosa.</returns>
        public async Task<bool> DeleteFormAsync(int id)
{
    try
    {
        var form = await _formData.GetByIdAsync(id);
        if (form == null)
        {
            _logger.LogWarning("Formulario con ID {FormId} no encontrado.", id);
            throw new EntityNotFoundException("Formulario", id);
        }

        form.DeleteAt = DateTime.UtcNow;
        var deleted = await _formData.UpdateAsync(form);
        return deleted;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar lógicamente el formulario con ID {FormId}", id);
        throw new ExternalServiceException("Base de datos", "Error al eliminar el formulario", ex);
    }
}


        /// <summary>
        /// Método para validar el DTO del formulario.
        /// </summary>
        private void ValidateForm(FormDto formDto)
        {
            if (formDto == null)
            {
                _logger.LogWarning("Se intentó crear o actualizar un formulario con datos nulos.");
                throw new ValidationException("formDto", "El formulario no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear o actualizar un formulario con Name vacío.");
                throw new ValidationException("Name", "El nombre del formulario es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(formDto.Code))
            {
                _logger.LogWarning("Se intentó crear o actualizar un formulario con Code vacío.");
                throw new ValidationException("Code", "El código del formulario es obligatorio.");
            }
        }

        // Método para mapear de Form a FormDto
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Code = form.Code,
                Active = form.Active
            };
        }

        // Método para mapear de FormDto a Form
        private Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                Id = formDto.Id,
                Name = formDto.Name,
                Code = formDto.Code,
                Active = formDto.Active
            };
        }

        // Método para mapear una lista de Form a una lista de FormDto
        private IEnumerable<FormDto> MapToDTOList(IEnumerable<Form> forms)
        {
            return forms.Select(form => MapToDTO(form)).ToList();
        }
    }
}
