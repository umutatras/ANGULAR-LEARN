using BlogCore.Api.Models;
using BlogCore.Api.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public ArticlesController(BlogDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("SearchArticles/{searchText}/{page}/{pageSize}")]
        public IActionResult SearchArticles(string searchText, int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query;
            query = _context.Articles.Include(x => x.Category).Include(x => x.Comments).Where(z => z.Title.Contains(searchText)).OrderByDescending(x => x.PublishDate);
            var resultQuery = ArticlesPagination(query, page, pageSize);
            var response = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };
            return Ok(response);
        }
        [HttpGet]
        [Route("GetArticleWithCategory/{categoryId}/{page}/{pageSize}")]
        public IActionResult GetArticleWithCategory(int categoryId, int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query = _context.Articles.Include(x => x.Category).Include(x => x.Comments).Where(x => x.CategoryId == categoryId).OrderByDescending(x => x.PublishDate);
            var queryResult = ArticlesPagination(query, page, pageSize);
            var result = new
            {
                TotalCount = queryResult.Item2,
                Articles = queryResult.Item1
            };
            return Ok(result);
        }
        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleResponse>>> GetArticles()
        {
            var result = await _context.Articles
                .Include(x => x.Category).Include(x => x.Comments).OrderByDescending(x => x.PublishDate)
                .Select(x => new ArticleResponse
                {
                    Id = x.Id,
                    PublishDate = x.PublishDate,
                    Title = x.Title,
                    Picture = x.Picture,
                    Category = new CategoryResponse() { Id = x.CategoryId, Name = x.Category.Name },
                    CommentCount = x.Comments.Count,
                    ViewCount = x.ViewCount

                })
                .ToListAsync();
            return Ok(result);
        }
        [HttpGet("{page}/{pageSize}")]
        public IActionResult GetArticle(int page = 1, int pageSize = 5)
        {
            IQueryable<Article> query;
            query = _context.Articles.Include(i => i.Category).Include(i => i.Comments).OrderByDescending(i => i.PublishDate);
            int totalCount = query.Count();
            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(5).Select(s => new ArticleResponse()
            {
                Id = s.Id,
                Content = s.ContentMain,
                SummaryContent = s.ContentSummary,
                Picture = s.Picture,
                PublishDate = s.PublishDate,
                Title = s.Title,
                ViewCount = s.ViewCount,
                CommentCount = s.Comments.Count,
                Category = new CategoryResponse() { Id = s.Category.Id, Name = s.Category.Name }
            }).ToList();
            var result = new
            {
                TotalCount = totalCount,
                Articles = articlesResponse,
            };
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleResponse>> GetArticleById(int id)
        {
            var article = await _context.Articles.Include(x => x.Category).Include(x => x.Comments).FirstOrDefaultAsync(f => f.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            ArticleResponse response = new ArticleResponse()
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.ContentMain,
                SummaryContent = article.ContentSummary,
                PublishDate = article.PublishDate,
                Picture = article.Picture,
                CommentCount = article.Comments.Count,
                ViewCount = article.ViewCount,
                Category = new CategoryResponse()
                {
                    Id = article.Category.Id,
                    Name = article.Category.Name
                }

            };
            return Ok(response);
        }
        // GET: api/Articles/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Article>> GetArticle(int id)
        //{
        //    if (_context.Articles == null)
        //    {
        //        return NotFound();
        //    }
        //    var article = await _context.Articles.FindAsync(id);

        //    if (article == null)
        //    {
        //        return NotFound();
        //    }

        //    return article;
        //}

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            Article firsArticle = _context.Articles.Find(id);
            firsArticle.Title = article.Title;
            firsArticle.ContentMain = article.ContentMain;
            firsArticle.ContentSummary = article.ContentSummary;
            firsArticle.PublishDate = article.PublishDate;
            firsArticle.Picture = article.Picture;
            firsArticle.CategoryId = article.Category.Id;

            _context.Articles.Update(firsArticle);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostArticle(Article article)
        {
            if (article.Category != null)
            {
                article.CategoryId = article.Category.Id;

            }
            article.Category = null;
            article.ViewCount = 0;
            article.PublishDate = DateTime.Now;
            if (_context.Articles == null)
            {
                return Problem("Entity set 'BlogDbContext.Articles'  is null.");
            }
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            if (_context.Articles == null)
            {
                return NotFound();
            }
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet]
        [Route("GetByMostViewArticle")]
        public IActionResult GetByMostViewArticle()
        {
            var article = _context.Articles.OrderByDescending(x => x.ViewCount).Take(5).Select(x => new ArticleResponse()
            {
                Title = x.Title,
                Id = x.Id
            });
            return Ok(article);
        }
        [HttpGet]
        [Route("GetArticlesArchive")]
        public IActionResult GetArticlesArchive()
        {
            var query = _context.Articles.GroupBy(x => new { x.PublishDate.Year, x.PublishDate.Month }).Select(x => new
            {
                year = x.Key.Year,
                month = x.Key.Month,
                count = x.Count(),
                monthName = new DateTime(x.Key.Year, x.Key.Month, 1).ToString("MMMM")

            })
                .ToList();
            return Ok(query);
        }

        [HttpGet]
        [Route("GetArchiveArticleList/{year}/{month}/{page}/{pageSize}")]
        public IActionResult GetArchiveArticleList(int year, int month, int page, int pageSize)
        {
            IQueryable<Article> query;
            query = _context.Articles.Include(x => x.Category).Include(x => x.Comments).Where(x => x.PublishDate.Year == year && x.PublishDate.Month == month)
                .OrderByDescending(x => x.PublishDate);
            var resultQuery = ArticlesPagination(query, page, pageSize);
            var response = new
            {
                Articles = resultQuery.Item1,
                TotalCount = resultQuery.Item2
            };
            return Ok(response);

        }
        [HttpGet]
        [Route("ArticleViewCountUp/{id}")]
        public IActionResult ArticleViewCountUp(int id)
        {
            Article article = _context.Articles.Find(id);
            article.ViewCount += 1;
            _context.SaveChanges();
            return Ok();
        }
        private bool ArticleExists(int id)
        {
            return (_context.Articles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        [Route("SaveArticlePicture")]
        public async Task<IActionResult> SaveArticlePicture(IFormFile picture)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/articlepicture", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await picture.CopyToAsync(stream);
            var result = new
            {
                path = "https://" + Request.Host + "/articlepicture/" + fileName
            };
            return Ok(result);
        }
        public System.Tuple<IEnumerable<ArticleResponse>, int> ArticlesPagination(IQueryable<Article> query, int page, int pageSize)
        {
            int totalCount = query.Count();
            var articlesResponse = query.Skip((pageSize * (page - 1))).Take(pageSize).Select(s => new ArticleResponse()
            {
                Id = s.Id,
                Content = s.ContentMain,
                SummaryContent = s.ContentSummary,
                Picture = s.Picture,
                PublishDate = s.PublishDate,
                Title = s.Title,
                ViewCount = s.ViewCount,
                CommentCount = s.Comments.Count,
                Category = new CategoryResponse() { Id = s.Category.Id, Name = s.Category.Name }
            });
            return new System.Tuple<IEnumerable<ArticleResponse>, int>(articlesResponse, totalCount);
        }
    }
}
