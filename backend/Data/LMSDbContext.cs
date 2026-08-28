using LMS.Data.Configuration;
namespace LMS.Data;

public class LMSDbContext : DbContext
{
    public LMSDbContext(DbContextOptions<LMSDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<CourseResource> CourseResources => Set<CourseResource>();
    public DbSet<ModuleResource> ModuleResources => Set<ModuleResource>();
    public DbSet<ActivityResource> ActivityResources => Set<ActivityResource>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LMSDbContext).Assembly);
    }
}