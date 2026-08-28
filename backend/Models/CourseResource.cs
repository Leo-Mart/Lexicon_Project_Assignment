  namespace LFS.backend.Models
  
  public class CourseResource
  {
      public Guid CourseId { get; set; }
      public Guid ResourceId { get; set; }

      public Course Course { get; set; } = null!;
      public Resource Resource { get; set; } = null!;
  }