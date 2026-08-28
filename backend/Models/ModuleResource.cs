namespace LMS.backend.Models;

public class ModuleResource
{
    public Guid ModuleId { get; set; }
    public Guid ResourceId { get; set; }

    public Module Module { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
}