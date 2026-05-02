namespace BlogAPI.Infrastructure.Repositories;

public class CommentRepository : BaseRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResponse<List<Comment>>> GetByPostAsync(Guid postId, PaginationFilter filter)
    {
        var totalRecords = await _dbSet.CountAsync(c => c.PostId == postId && !c.IsDeleted);

        var data = await _dbSet
            .Where(c => c.PostId == postId && !c.IsDeleted)
            .Include(c => c.Author)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return PagedResponse<List<Comment>>.Success(data, filter.PageNumber, filter.PageSize, totalRecords);
    }
}