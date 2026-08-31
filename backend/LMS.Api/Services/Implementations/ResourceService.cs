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

    public ResourceService(
        IResourceRepository resourceRepository,
        IUnitOfWork unitOfWork)
    {
        _resourceRepository = resourceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ResourceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Resource> resources = await _resourceRepository.GetAllAsync(cancellationToken);

        return resources.Select(MapToDto).ToList();
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
            : MapToDto(resource);
    }

    public async Task<List<ResourceDto>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByCourseIdAsync(
                courseId,
                cancellationToken
            );

        return resources.Select(MapToDto).ToList();
    }

    public async Task<List<ResourceDto>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByModuleIdAsync(
                moduleId,
                cancellationToken
            );

        return resources.Select(MapToDto).ToList();
    }

    public async Task<List<ResourceDto>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        List<Resource> resources =
            await _resourceRepository.GetByActivityIdAsync(
                activityId,
                cancellationToken
            );

        return resources.Select(MapToDto).ToList();
    }

    public async Task<ResourceDto> CreateAsync(
        Guid createdByTeacherId,
        ResourceCreateDto request,
        CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        var resource = new Resource
        {
            ResourceId = Guid.NewGuid(),
            CreatedByTeacherId = createdByTeacherId,
            Name = request.Name,
            Description = request.Description,
            Content = request.Content,
            Uri = request.Uri,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _resourceRepository.AddAsync(
            resource,
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(resource);
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

        resource.Name = request.Name;
        resource.Description = request.Description;
        resource.Content = request.Content;
        resource.Uri = request.Uri;
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

    private static ResourceDto MapToDto(Resource resource)
    {
        return new ResourceDto
        {
            ResourceId = resource.ResourceId,
            CreatedByTeacherId = resource.CreatedByTeacherId,
            Name = resource.Name,
            Description = resource.Description,
            Content = resource.Content,
            Uri = resource.Uri,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt
        };
    }
}
