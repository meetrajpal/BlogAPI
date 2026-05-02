namespace BlogAPI.Domain.Interfaces.Repositories;

public interface ICommentRepository : IBaseRepository<Comment>
{
    Task<PagedResponse<List<Comment>>> GetByPostAsync(Guid postId, PaginationFilter filter);
}