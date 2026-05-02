using BlogAPI.Domain.DTOs;
using BlogAPI.Domain.DTOs.Comments;
using BlogAPI.Domain.DTOs.Posts;

namespace BlogAPI.Infrastructure.Mapping;

[Mapper]
public partial class BlogMapper
{    
    public partial UserDto ToDto(ApplicationUser user);
    
    public partial Post ToEntity(CreatePostDto dto);
    public partial void UpdateEntity(UpdatePostDto dto, Post entity);
    public partial PostDto ToDto(Post post);

    public partial Comment ToEntity(CreateCommentDto dto);
    public partial CommentDto ToDto(Comment comment);
}