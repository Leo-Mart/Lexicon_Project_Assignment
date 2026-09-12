using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

public class ModuleProfile : Profile
{
    public ModuleProfile()
    {
        CreateMap<Module, ModuleDto>()
            .ForMember(
                dto => dto.ModuleResources,
                opt => opt.MapFrom(x => x.ModuleResources.Select(cr => cr.Resource))
            );
        CreateMap<CreateNewModuleDto, Module>();
        CreateMap<UpdateModuleDto, Module>();
    }
}
