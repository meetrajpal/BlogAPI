using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Infrastructure.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly BlogMapper _mapper;

    private const string PostCachePrefix = "post:";
    private const string AllPostsCacheKey = "posts:all";

    public PostService(
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        BlogMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<PagedResponse<List<PostDto>>> GetAllPostsAsync(PaginationFilter filter)
    {
        var cacheKey = $"{AllPostsCacheKey}:{filter.PageNumber}:{filter.PageSize}";

        var cached = await _cacheService.GetAsync<PagedResponse<List<PostDto>>>(cacheKey);
        if (cached != null)
            return cached;

        var result = await _unitOfWork.Posts.GetAllAsync(filter);

        var mappedData = result.Data?.Select(p =>
        {
            var dto = _mapper.ToDto(p);
            dto.CommentCount = p.Comments?.Count ?? 0;
            return dto;
        }).ToList();

        var response = PagedResponse<List<PostDto>>.Success(
            mappedData,
            result.PageNumber,
            result.PageSize,
            result.TotalRecords,
            "Posts fetched successfully.");

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

        return response;
    }

    public async Task<ApiResponse<PostDto>> GetPostBySlugAsync(string slug)
    {
        var cacheKey = $"{PostCachePrefix}slug:{slug}";

        var cached = await _cacheService.GetAsync<PostDto>(cacheKey);
        if (cached != null)
            return ApiResponse<PostDto>.Success(cached, "Post fetched successfully.");

        var post = await _unitOfWork.Posts.GetBySlugAsync(slug);
        if (post == null)
            throw new KeyNotFoundException($"Post with slug '{slug}' was not found.");

        var dto = _mapper.ToDto(post);
        dto.CommentCount = post.Comments?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return ApiResponse<PostDto>.Success(dto, "Post fetched successfully.");
    }

    public async Task<ApiResponse<PostDto>> GetPostByIdAsync(Guid id)
    {
        var cacheKey = $"{PostCachePrefix}{id}";

        var cached = await _cacheService.GetAsync<PostDto>(cacheKey);
        if (cached != null)
            return ApiResponse<PostDto>.Success(cached, "Post fetched successfully.");

        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id '{id}' was not found.");

        var dto = _mapper.ToDto(post);
        dto.CommentCount = post.Comments?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

        return ApiResponse<PostDto>.Success(dto, "Post fetched successfully.");
    }

    public async Task<ApiResponse<PostDto>> CreatePostAsync(CreatePostDto dto, Guid authorId)
    {
        var slug = GenerateSlug(dto.Title);

        var slugExists = await _unitOfWork.Posts.SlugExistsAsync(slug);
        if (slugExists)
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";

        var post = _mapper.ToEntity(dto);
        post.Slug = slug;
        post.AuthorId = authorId;

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync(AllPostsCacheKey);

        var created = await _unitOfWork.Posts.GetByIdAsync(post.Id);
        var postDto = _mapper.ToDto(created!);

        return ApiResponse<PostDto>.Success(postDto, "Post created successfully.");
    }

    public async Task<ApiResponse<PostDto>> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid authorId, bool isAdmin)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id '{id}' was not found.");

        if (post.AuthorId != authorId && !isAdmin)
            throw new UnauthorizedAccessException("You can only edit your own posts.");


        if (post.Title != dto.Title)
        {
            var newSlug = GenerateSlug(dto.Title);
            var slugExists = await _unitOfWork.Posts.SlugExistsAsync(newSlug);
            if (slugExists)
                newSlug = $"{newSlug}-{Guid.NewGuid().ToString()[..8]}";
            post.Slug = newSlug;
        }

        _mapper.UpdateEntity(dto, post);

        await _unitOfWork.Posts.UpdateAsync(post);
        await _unitOfWork.SaveChangesAsync();


        await _cacheService.RemoveAsync($"{PostCachePrefix}{id}");
        await _cacheService.RemoveAsync($"{PostCachePrefix}slug:{post.Slug}");
        await _cacheService.RemoveAsync(AllPostsCacheKey);

        var postDto = _mapper.ToDto(post);
        return ApiResponse<PostDto>.Success(postDto, "Post updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeletePostAsync(Guid id, Guid authorId, bool isAdmin)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(id);
        if (post == null)
            throw new KeyNotFoundException($"Post with id '{id}' was not found.");

        if (post.AuthorId != authorId && !isAdmin)
            throw new UnauthorizedAccessException("You can only delete your own posts.");

        await _unitOfWork.Posts.DeleteAsync(post);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.RemoveAsync($"{PostCachePrefix}{id}");
        await _cacheService.RemoveAsync($"{PostCachePrefix}slug:{post.Slug}");
        await _cacheService.RemoveAsync(AllPostsCacheKey);

        return ApiResponse<bool>.Success(true, "Post deleted successfully.");
    }

    public async Task<PagedResponse<List<PostDto>>> GetPostsByAuthorAsync(Guid authorId, PaginationFilter filter)
    {
        var cacheKey = $"{PostCachePrefix}author:{authorId}:{filter.PageNumber}:{filter.PageSize}";

        var cached = await _cacheService.GetAsync<PagedResponse<List<PostDto>>>(cacheKey);
        if (cached != null)
            return cached;

        var result = await _unitOfWork.Posts.GetByAuthorAsync(authorId, filter);

        var mappedData = result.Data?.Select(p =>
        {
            var dto = _mapper.ToDto(p);
            dto.CommentCount = p.Comments?.Count ?? 0;
            return dto;
        }).ToList();

        var response = PagedResponse<List<PostDto>>.Success(
            mappedData,
            result.PageNumber,
            result.PageSize,
            result.TotalRecords,
            "Posts fetched successfully.");

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));

        return response;
    }

    private static string GenerateSlug(string title)
    {
        return title.ToLower()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("!", "")
            .Replace("?", "")
            .Trim();
    }
}