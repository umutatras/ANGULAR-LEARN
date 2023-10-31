namespace BlogCore.Api.Models;

public class CommentAdd
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string Name { get; set; } = null!;
    public string ContentMain { get; set; } = null!;
}
