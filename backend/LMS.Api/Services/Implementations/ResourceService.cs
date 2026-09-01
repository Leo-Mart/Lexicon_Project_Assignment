using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Resources;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class ResourceService : IResourceService
{
    private readonly IResourceRepository _resourceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ResourceService(
        IResourceRepository resourceRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _resourceRepository = resourceRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ResourceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Resource> resources = await _resourceRepository.GetAllAsync(cancellationToken);

        return _mapper.Map<List<ResourceDto>>(resources);
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        Resource? resource =
            await _resourceRepository.GetByIdAsync(
                resourceId,
                cancellationToken
            );

        return resource is null
            ? null
            : _mapper.Map<ResourceDto>(resource);
    }

    public async Task<List<ResourceDto>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByCourseIdAsync(
                courseId,
                cancellationToken
            );

        return _mapper.Map<List<ResourceDto>>(resources);
    }

    public async Task<List<ResourceDto>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByModuleIdAsync(
                moduleId,
                cancellationToken
            );

        return _mapper.Map<List<ResourceDto>>(resources);
    }

    public async Task<List<ResourceDto>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByActivityIdAsync(
                activityId,
                cancellationToken
            );

        return _mapper.Map<List<ResourceDto>>(resources);
    }

    public async Task<ResourceDto> CreateAsync(
        Guid createdByTeacherId,
        ResourceCreateDto request,
        CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        Resource resource = _mapper.Map<Resource>(request);

        resource.ResourceId = Guid.NewGuid();
        resource.CreatedByTeacherId = createdByTeacherId;
        resource.CreatedAt = now;
        resource.UpdatedAt = now;

        await _resourceRepository.AddAsync(
            resource,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ResourceDto>(resource);
    }

    public async Task<bool> UpdateAsync(
        Guid resourceId,
        ResourceUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        Resource? resource =
            await _resourceRepository.GetByIdAsync(
                resourceId,
                cancellationToken
            );

        if (resource is null)
        {
            return false;
        }

        _mapper.Map(request, resource);

        resource.UpdatedAt = DateTime.UtcNow;

        _resourceRepository.Update(resource);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        Guid resourceId,
        CancellationToken cancellationToken = default)
    {
        Resource? resource =
            await _resourceRepository.GetByIdAsync(
                resourceId,
                cancellationToken
            );

        if (resource is null)
        {
            return false;
        }

        _resourceRepository.Delete(resource);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AddToCourseAsync(Guid resourceId, Guid courseId, CancellationToken cancellationToken = default)
    {
        Resource? resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (resource is null)
        {
            return false;
        }

        await _resourceRepository.AddToCourseAsync(resourceId, courseId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AddToModuleAsync(Guid resourceId, Guid moduleId, CancellationToken cancellationToken = default)
    {
        Resource? resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (resource is null)
        {
            return false;
        }

        await _resourceRepository.AddToModuleAsync(resourceId, moduleId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> AddToActivityAsync(Guid resourceId, Guid activityId, CancellationToken cancellationToken = default)
    {
        Resource? resource = await _resourceRepository.GetByIdAsync(resourceId, cancellationToken);

        if (resource is null)
        {
            return false;
        }

        await _resourceRepository.AddToActivityAsync(resourceId, activityId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
