namespace BlogAPI.Domain.DTOs.Posts;

public class UpdatePostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}