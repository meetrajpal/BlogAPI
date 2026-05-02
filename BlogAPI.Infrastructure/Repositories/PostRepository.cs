namespace BlogAPI.Infrastructure.Repositories;

public class PostRepository : BaseRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context) { }

    public async Task<Post?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted);
    }

    public async Task<PagedResponse<List<Post>>> GetByAuthorAsync(Guid authorId, PaginationFilter filter)
    {
        var totalRecords = await _dbSet.CountAsync(p => p.AuthorId == authorId && !p.IsDeleted);

        var data = await _dbSet
            .Where(p => p.AuthorId == authorId && !p.IsDeleted)
            .Include(p => p.Author)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return PagedResponse<List<Post>>.Success(data, filter.PageNumber, filter.PageSize, totalRecords);
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        return await _dbSet.AnyAsync(p => p.Slug == slug && !p.IsDeleted);
    }
}