using AutoMapper;
using LMS.Api.DTOs.Courses;
using LMS.Api.Models;

namespace LMS.Api.Mappings;

/// <summary>
/// Defines the object-to-object mappings between courses and their DTOs.
/// </summary>
public class CourseProfile : Profile
{
    public CourseProfile()
    {
        // Read path: the entity as the API returns it.
        //
        // Implementation detail:
        // - Every member matches by name, so no ForMember is needed.
        // - Modules maps as ICollection<Module>, so the DTO carries entities.
        //   Worth revisiting alongside a ModuleDto.
        //
        // TODO:
        // - GetAllCourses maps in memory. _mapper.ProjectTo<CourseDto>(query)
        //   would put the column list into SQL instead, but that needs the
        //   repository to expose IQueryable rather than returning the result
        //   of ToListAsync.
        CreateMap<Course, CourseDto>();

        // Create path: a new Course built from the request body.
        //
        // Implementation detail:
        // - Name, Description, StartDate and EndDate map by name convention.
        //   CreateNewCourseDto has no other members.
        // - CourseId is left unset so EF assigns the key on save, and the
        //   timestamps are set in CourseRepository.CreateCourseAsync.
        // - The Ignore calls change nothing today, since nothing maps to those
        //   members anyway. They guard against later, the same way
        //   UserProfile's create map does: a future DTO property named
        //   CreatedAt or Modules would otherwise start mapping by name
        //   convention and let a client set the entity's timestamp or write
        //   related rows.
        CreateMap<CreateNewCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Modules, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
            .ForMember(dest => dest.CourseResources, opt => opt.Ignore());

        // Update path: a full replace applied to a Course already loaded from
        // the database.
        //
        // Implementation detail:
        // - Every member on UpdateCourseDto is [Required], so unlike
        //   UserUpdateDto there is nothing to skip and no PreCondition is used.
        // - Call it as Map(dto, existingCourse). The DTO carries no CourseId,
        //   so mapping onto a fresh Course would lose the key.
        // - UpdatedAt is ignored here, so whichever layer applies this map
        //   needs to set it.
        // - The Ignore calls guard the same way as on the create map, and
        //   matter more here: this map runs against a tracked entity, so a
        //   future DTO property could otherwise replace collections EF has
        //   already loaded.
        //
        // TODO:
        // - This map is the one piece not yet in use. CourseRepository
        //   .UpdateCourseAsync takes UpdateCourseDto and assigns the four
        //   members itself. Applying this map in CourseService and handing the
        //   repository a Course would put it to work and drop the repository's
        //   dependency on LMS.Api.DTOs.
        CreateMap<UpdateCourseDto, Course>()
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Modules, opt => opt.Ignore())
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
            .ForMember(dest => dest.CourseResources, opt => opt.Ignore());
    }
}
