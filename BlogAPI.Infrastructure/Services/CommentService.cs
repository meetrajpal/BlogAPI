using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly BlogMapper _mapper;

    private const string CommentCachePrefix = "comment:";

    public CommentService(
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        BlogMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<List<CommentDto>>> GetCommentsByPostAsync(Guid postId, PaginationFilter filter)
    {
        var cacheKey = $"{CommentCachePrefix}post:{postId}:{filter.PageNumber}:{filter.PageSize}";

        var cached = await _cacheService.GetAsync<PagedResponse<List<CommentDto>>>(cacheKey);
        if (cached != null)
            return cached;

        var result = await _unitOfWork.Comments.GetByPostAsync(postId, filter);

        var mappedData = result.Data?
            .Select(c => _mapper.ToDto(c))
            .ToList();

        var response = PagedResponse<List<CommentDto>>.Success(
            mappedData,
            result.PageNumber,
            result.PageSize,
            result.TotalRecords,
            "Comments fetched successfully.");

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

        return response;
    }

    public async Task<ApiResponse<CommentDto>> GetCommentByIdAsync(Guid id)
    {
        var cacheKey = $"{CommentCachePrefix}{id}";

        var cached = await _cacheService.GetAsync<CommentDto>(cacheKey);
        if (cached != null)
            return ApiResponse<CommentDto>.Success(cached, "Comment fetched successfully.");

        var comment = await _unitOfWork.Comments.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id '{id}' was not found.");

        var dto = _mapper.ToDto(comment);

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return ApiResponse<CommentDto>.Success(dto, "Comment fetched successfully.");
    }

    public async Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto, Guid authorId)
    {
        var postExists = await _unitOfWork.Posts.ExistsAsync(dto.PostId);
        if (!postExists)
            throw new KeyNotFoundException($"Post with id '{dto.PostId}' was not found.");

        var comment = _mapper.ToEntity(dto);
        comment.AuthorId = authorId;

        await _unitOfWork.Comments.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{CommentCachePrefix}post:{dto.PostId}:1:10");

        var created = await _unitOfWork.Comments.GetByIdAsync(comment.Id);
        var commentDto = _mapper.ToDto(created!);

        return ApiResponse<CommentDto>.Success(commentDto, "Comment created successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteCommentAsync(Guid id, Guid authorId, bool isAdmin)
    {
        var comment = await _unitOfWork.Comments.GetByIdAsync(id);
        if (comment == null)
            throw new KeyNotFoundException($"Comment with id '{id}' was not found.");

        if (comment.AuthorId != authorId && !isAdmin)
            throw new UnauthorizedAccessException("You can only delete your own comments.");

        await _unitOfWork.Comments.DeleteAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{CommentCachePrefix}{id}");
        await _cacheService.RemoveAsync($"{CommentCachePrefix}post:{comment.PostId}:1:10");

        return ApiResponse<bool>.Success(true, "Comment deleted successfully.");
    }
}