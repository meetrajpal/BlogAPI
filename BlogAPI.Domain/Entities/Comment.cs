namespace BlogAPI.Domain.Entities;

public class Comment : BaseEntity
{
    public string Body { get; set; } = string.Empty;

    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }

    public Post Post { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
}