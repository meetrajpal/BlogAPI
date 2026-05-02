namespace BlogAPI.Domain.DTOs.Comments;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserDto Author { get; set; } = null!;
    public Guid PostId { get; set; }
}