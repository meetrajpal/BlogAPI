namespace BlogAPI.Domain.Interfaces.Repositories;

public interface IPostRepository : IBaseRepository<Post>
{
    Task<Post?> GetBySlugAsync(string slug);
    Task<PagedResponse<List<Post>>> GetByAuthorAsync(Guid authorId, PaginationFilter filter);
    Task<bool> SlugExistsAsync(string slug);
}