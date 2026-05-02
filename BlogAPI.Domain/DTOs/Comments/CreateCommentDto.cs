namespace BlogAPI.Domain.DTOs.Comments;

public class CreateCommentDto
{
    public string Body { get; set; } = string.Empty;
    public Guid PostId { get; set; }
}