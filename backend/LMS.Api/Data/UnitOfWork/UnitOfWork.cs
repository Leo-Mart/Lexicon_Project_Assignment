namespace LMS.Api.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly LMSDbContext _context;

    public UnitOfWork(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
