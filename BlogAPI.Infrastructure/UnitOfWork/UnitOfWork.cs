using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IPostRepository Posts { get; }
    public ICommentRepository Comments { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Posts = new PostRepository(context);
        Comments = new CommentRepository(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}