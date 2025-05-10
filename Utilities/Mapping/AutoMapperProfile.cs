using AutoMapper;
using Entity.DTOs;
using Entity.Model;
using Module = Entity.Model.Module;

namespace Utilities.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDto>().ReverseMap();
            
            // Rol mappings
            CreateMap<Rol, RolDto>().ReverseMap();
            
            // Form mappings
            CreateMap<Form, FormDto>().ReverseMap();
            
            // Module mappings
            CreateMap<Module, ModuleDto>().ReverseMap();
            
            // Permission mappings
            CreateMap<Permission, PermissionDto>()
                .ForMember(dest => dest.CanRead, opt => opt.MapFrom(src => src.Can_Read))
                .ForMember(dest => dest.CanCreate, opt => opt.MapFrom(src => src.Can_Create))
                .ForMember(dest => dest.CanUpdate, opt => opt.MapFrom(src => src.Can_Update))
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.Can_Delete))
                .ReverseMap()
                .ForMember(dest => dest.Can_Read, opt => opt.MapFrom(src => src.CanRead))
                .ForMember(dest => dest.Can_Create, opt => opt.MapFrom(src => src.CanCreate))
                .ForMember(dest => dest.Can_Update, opt => opt.MapFrom(src => src.CanUpdate))
                .ForMember(dest => dest.Can_Delete, opt => opt.MapFrom(src => src.CanDelete));
            
            // RolFormPermission mappings
            CreateMap<RolFormPermission, RolFormPermissionDto>().ReverseMap();
            
            // RolUser mappings
            CreateMap<RolUser, RolUserDto>().ReverseMap();
            
            // FormModule mappings
            CreateMap<FormModule, FormModuleDto>().ReverseMap();
            
            // Worker mappings
            CreateMap<Worker, WorkerDto>().ReverseMap();
            
            // Login mappings
            CreateMap<Login, LoginDto>().ReverseMap();
            
            // WorkerLogin mappings
            CreateMap<WorkerLogin, WorkerLoginDto>().ReverseMap();
            
            // ActivityLog mappings
            CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
        }
    }
}