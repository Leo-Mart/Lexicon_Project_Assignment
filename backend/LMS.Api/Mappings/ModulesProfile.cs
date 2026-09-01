using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

public class ModuleProfile : Profile
{
    public ModuleProfile()
    {
        CreateMap<Module, ModuleDto>();
        CreateMap<CreateNewModuleDto, Module>();
        CreateMap<UpdateModuleDto, Module>();
    }
}
