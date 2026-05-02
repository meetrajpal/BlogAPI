namespace BlogAPI.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;

    public Guid AuthorId { get; set; }

    public ApplicationUser Author { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}