using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Data;
using Business;

public class FormModuleBusiness : GenericBusiness<FormModuleDto, FormModule>
{
    private readonly FormModuleData _formModuleData;

    public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormModuleBusiness> logger)
        : base(formModuleData, logger)
    {
        _formModuleData = formModuleData;
    }

    // Implementación de los métodos abstractos requeridos
    protected override void ValidateDto(FormModuleDto dto)
    {
        if (dto == null)
            throw new ArgumentException("El objeto FormModuleDto no puede ser nulo.");
            
        if (dto.ModuleId <= 0)
            throw new ArgumentException("El ModuleId debe ser mayor que cero.");
            
        if (dto.FormId <= 0)
            throw new ArgumentException("El FormId debe ser mayor que cero.");
    }

    protected override FormModule MapToEntity(FormModuleDto dto)
    {
        return new FormModule
        {
            Id = dto.Id,
            ModuleId = dto.ModuleId,
            FormId = dto.FormId,
        };
    }

    protected override FormModuleDto MapToDto(FormModule entity)
    {
        return new FormModuleDto
        {
            Id = entity.Id,
            ModuleId = entity.ModuleId,
            FormId = entity.FormId,
        };
    }

    protected override IEnumerable<FormModuleDto> MapToDtoList(IEnumerable<FormModule> entities)
    {
        return entities.Select(MapToDto).ToList();
    }
}