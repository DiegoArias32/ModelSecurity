using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Exceptions;

namespace Business
{
    public class FormBusiness : GenericBusiness<FormDto, Form>
    {
        public FormBusiness(IGenericData<Form> data, ILogger<FormBusiness> logger)
            : base(data, logger)
        {
        }

        protected override void ValidateDto(FormDto dto)
        {
            if (dto == null)
                throw new ValidationException("formDto", "El formulario no puede ser nulo.");
            
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("Name", "El nombre del formulario es obligatorio.");
            
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ValidationException("Code", "El código del formulario es obligatorio.");
        }

        protected override Form MapToEntity(FormDto dto)
        {
            return new Form
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                Active = dto.Active
            };
        }

        protected override FormDto MapToDto(Form entity)
        {
            return new FormDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Active = entity.Active
            };
        }

        protected override IEnumerable<FormDto> MapToDtoList(IEnumerable<Form> entities)
        {
            return entities.Select(MapToDto);
        }
    }
}