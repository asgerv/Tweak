using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;
using MVCForum.Utilities;

namespace MVCForum.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleTagService _articleTagService;
        private readonly MVCForumContext _context;

        public ArticleService(IArticleCommentService articleCommentService, IArticleTagService articleTagService,
            IMVCForumContext context)
        {
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
            _context = context as MVCForumContext;
        }

        public Article Add(Article article)
        {
            return _context.Article.Add(article);
        }

        public void PublishArticle(Article article)
        {
            if (article.IsPublished)
                return;
            article.IsPublished = true;
            article.PublishDate = DateTime.Now;
        }
        public Article AddNewArticle(string header, string description, string body, string image, ArticleCategory category, MembershipUser user)
        {
            var article = new Article
            {
                Header = header,
                Description = description,
                Body = body,
                User = user,
                CreateDate = DateTime.Now,
                Image = image,
                Slug =
                    ServiceHelpers.GenerateSlug(header, GetArticleBySlugLike(ServiceHelpers.CreateUrl(header)), null),
                PublishDate = new DateTime(1800,1,1),
                ArticleCategory = category
            };
            // Skal objektets lister muligvis initialiseres?
            return _context.Article.Add(article);
        }

        public bool Delete(Article article)
        {
            // Fjern alle comments
            //foreach (var comment in article.Comments.ToList())
            //{
            //    _articleCommentService.Delete(comment);
            //}
            // Slet article
            _context.Article.Remove(article);
            return false;
        }

        public IEnumerable<Article> GetAll()
        {
            return _context.Article;
        }

        public Article Get(Guid articleId)
        {
            return _context.Article.FirstOrDefault(x => x.Id == articleId);
        }

        public Article Get(string slug)
        {
            return _context.Article
                .Include(x => x.User)
                .FirstOrDefault(x => x.Slug == slug);
        }

        public IList<Article> GetMostPopular(Guid? thisArticleId, int maxAgeInDays, int amountToTake)
        {
            var lastDate = DateTime.Now.AddDays(-maxAgeInDays);
            return _context.Article
                .Include(x => x.User)
                .Include(x => x.Tags)
                .Where(x => x.Id != thisArticleId)
                .Where(x => x.IsPublished)
                .Where(x => x.PublishDate >= lastDate)
                .OrderByDescending(x => x.Views)
                .Take(amountToTake)
                .ToList();
        }

        public IList<Article> GetRelated(Article article, int amountToTake)
        {
            var ids = article.Tags.Select(t => t.Id).ToList();
            var query = (from a in _context.Article
                         where a.Id != article.Id
                         let commonTags = a.Tags.Where(tag => ids.Contains(tag.Id))
                         let commonCount = commonTags.Count()
                         where commonCount > 0
                         orderby commonCount descending
                         select a).Take(amountToTake);
            return query.ToList();
        }

        public IList<Article> GetNewest(int amountToTake)
        {
            return _context.Article
                .OrderByDescending(x => x.CreateDate)
                .Take(amountToTake)
                .ToList();
        }

        public IList<Article> GetNewestPublished(int amountToTake)
        {
            return _context.Article
                .OrderByDescending(x => x.PublishDate)
                .Where(x => x.IsPublished)
                .Take(amountToTake)
                .ToList();
        }

        public IList<Article> GetByUser(Guid memberId, int amountToTake)
        {
            return _context.Article
                .Include(x => x.User)
                .Where(x => x.User.Id == memberId)
                .Take(amountToTake)
                .ToList();
        }

        public IList<Article> GetAllAllowed(Guid memberId)
        {
            return _context.Article
                .Include(x => x.User)
                .Where(x => x.User.Id == memberId || x.IsPublished)
                .ToList();
        }

        public IList<Article> Search(int amountToTake, string keyword)
        {
            var articles = _context.Article
                .Include(x => x.Tags)
                .Where(x => x.IsPublished);
            var search = StringUtils.ReturnSearchString(keyword);
            var splitSearch = search.Split(' ').ToList();
            foreach (var term in splitSearch)
            {
                var sTerm = term.Trim().ToUpper();
                articles =
                    articles.Where(
                        x =>
                            x.Header.ToUpper().Contains(sTerm) || x.Description.ToUpper().Contains(sTerm) ||
                            x.Tags.Any(t => t.Name.ToUpper().Contains(sTerm)));
            }
            return articles.Take(amountToTake).ToList();
        }

        public int Count()
        {
            return _context.Article.Count();
        }

        public void Edit(Article article)
        {
            article.Slug = ServiceHelpers.GenerateSlug(article.Header,
                GetArticleBySlugLike(ServiceHelpers.CreateUrl(article.Header)), article.Slug);
            _context.Entry(article).State = EntityState.Modified;
        }

        public void WipeDatabase()
        {
            //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [ArticleComments]");
            //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Article_Tags]");
            //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Articles]");
            //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [ArticleTags]");
            foreach (var comment in _articleCommentService.GetAll())
            {
                _articleCommentService.DeleteFromDb(comment);
            }
            foreach (var article in GetAll())
            {
                Delete(article);
            }

            foreach (var articleTag in _articleTagService.GetAll())
            {
                _articleTagService.Delete(articleTag);
            }
        }

        public void CreateTestData(MembershipUser user)
        {
            //    // Opret 20
            //    for (var i = 0; i < 20; i++)
            //    {
            //        var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*13);
            //        var article = new Article
            //        {
            //            Header = "Lorem ipsum dolor sit amet, consectetur adipiscing elit volutpat." + NumberToWords(i),
            //            Description =
            //                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer posuere posuere metus nec dignissim. Curabitur eget enim ac ex sodales mollis nec aliquet urna. Mauris ut ultrices sapien. Curabitur tempor sem non dapibus volutpat.",
            //            Body = "Body",
            //            CreateDate = createDate,
            //            Image = "/Images/Default.jpeg",
            //            PublishDate = createDate,
            //            IsPublished = true,
            //            User = user,
            //            Comments = new List<ArticleComment>()
            //        };
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer et",
            //            DateCreated = createDate.AddMinutes(5),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer to",
            //            DateCreated = createDate.AddMinutes(15),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        Add(article);
            //        _articleTagService.Add("Windows, Linux, OS", article);
            //    }
            //    // Opret 20
            //    for (var i = 80; i < 100; i++)
            //    {
            //        var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*7);
            //        var article = new Article
            //        {
            //            Header = "En anden test artikel " + NumberToWords(i),
            //            Description = "Beskrivelse",
            //            Body = "Body",
            //            CreateDate = createDate,
            //            PublishDate = createDate,
            //            Image = "Imagepath",
            //            IsPublished = true,
            //            User = user,
            //            Comments = new List<ArticleComment>()
            //        };
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer et",
            //            DateCreated = createDate.AddMinutes(5),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer to",
            //            DateCreated = createDate.AddMinutes(15),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        Add(article);
            //        _articleTagService.Add("Testtag, BenQ, Monitor", article);
            //    }
            //    // Opret 10
            //    for (var i = 20; i < 30; i++)
            //    {
            //        var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*23);
            //        var article = new Article
            //        {
            //            Header = "En tredje test artikel " + NumberToWords(i),
            //            Description = "Beskrivelse",
            //            Body = "Body",
            //            CreateDate = createDate,
            //            PublishDate = createDate,
            //            Image = "Imagepath",
            //            IsPublished = true,
            //            User = user,
            //            Comments = new List<ArticleComment>()
            //        };
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer et",
            //            DateCreated = createDate.AddMinutes(5),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        article.Comments.Add(new ArticleComment
            //        {
            //            Article = article,
            //            CommentBody = "Test comment nummer to",
            //            DateCreated = createDate.AddMinutes(15),
            //            IsDeleted = false,
            //            User = user
            //        });
            //        Add(article);
            //        _articleTagService.Add("Højtalere, Bose, Lyd, Chromecast", article);
            //    }
        }

        public IList<Article> GetArticleBySlugLike(string slug)
        {
            return _context.Article
                .Where(x => x.Slug.Contains(slug))
                .ToList();
        }
    }
}