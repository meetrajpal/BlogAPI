namespace BlogAPI.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPostRepository Posts { get; }
    ICommentRepository Comments { get; }
    Task<int> SaveChangesAsync();
}