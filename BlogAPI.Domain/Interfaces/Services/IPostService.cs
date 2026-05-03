namespace BlogAPI.Domain.Interfaces.Services;

public interface IPostService
{
    Task<PagedResponse<List<PostDto>>> GetAllPostsAsync(PaginationFilter filter);
    Task<ApiResponse<PostDto>> GetPostBySlugAsync(string slug);
    Task<ApiResponse<PostDto>> GetPostByIdAsync(Guid id);
    Task<ApiResponse<PostDto>> CreatePostAsync(CreatePostDto dto, Guid authorId);
    Task<ApiResponse<PostDto>> UpdatePostAsync(Guid id, UpdatePostDto dto, Guid authorId, bool isAdmin);
    Task<ApiResponse<bool>> DeletePostAsync(Guid id, Guid authorId, bool isAdmin);
    Task<PagedResponse<List<PostDto>>> GetPostsByAuthorAsync(Guid authorId, PaginationFilter filter);
}