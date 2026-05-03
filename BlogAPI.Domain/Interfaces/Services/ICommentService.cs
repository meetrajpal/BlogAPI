namespace BlogAPI.Domain.Interfaces.Services;

public interface ICommentService
{
    Task<PagedResponse<List<CommentDto>>> GetCommentsByPostAsync(Guid postId, PaginationFilter filter);
    Task<ApiResponse<CommentDto>> GetCommentByIdAsync(Guid id);
    Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto, Guid authorId);
    Task<ApiResponse<bool>> DeleteCommentAsync(Guid id, Guid authorId, bool isAdmin);
}