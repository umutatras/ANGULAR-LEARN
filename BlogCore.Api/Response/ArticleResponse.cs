namespace BlogCore.Api.Response;

public class ArticleResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string SummaryContent { get; set; }
    public string Picture { get; set; }
    public DateTime PublishDate { get; set; }
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
    public CategoryResponse Category { get; set; } = new CategoryResponse();
}
