namespace LMS.Api.Models;

public class ActivityResource
{
    public Guid ActivityId { get; set; }
    public Guid ResourceId { get; set; }

    public Activity Activity { get; set; } = null!;
    public Resource Resource { get; set; } = null!;
}
