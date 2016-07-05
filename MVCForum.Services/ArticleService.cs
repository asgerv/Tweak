using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;

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

        public Article AddNewArticle(string header, string description, string body, string image, bool isPublished,
            DateTime publishdate, MembershipUser user)
        {
            var article = new Article
            {
                Header = header,
                Description = description,
                Body = body,
                User = user,
                CreateDate = DateTime.Now,
                PublishDate = publishdate,
                IsPublished = isPublished,
                Image = image
            };
            // Skal objektets lister muligvis initialiseres?
            return _context.Article.Add(article);
        }

        public bool Delete(Article article)
        {
            // Fjern alle comments
            foreach (var comment in article.Comments.ToList())
            {
                _articleCommentService.Delete(comment);
            }
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

        public int Count()
        {
            return _context.Article.Count();
        }

        public IList<Article> GetNewest(int amountToTake)
        {
            return _context.Article
                .OrderByDescending(x => x.CreateDate)
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

        public void Edit(Article article)
        {
            //EntityState
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
            // Opret 20
            for (var i = 0; i < 20; i++)
            {
                var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*13);
                var article = new Article
                {
                    Header = "Test Article " + NumberToWords(i),
                    Description = "Beskrivelse",
                    Body = "Body",
                    CreateDate = createDate,
                    Image = "/Images/Default.jpeg",
                    PublishDate = createDate,
                
                    IsPublished = true,
                    User = user,
                    Comments = new List<ArticleComment>()
                };
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer et",
                    DateCreated = createDate.AddMinutes(5),
                    IsDeleted = false,
                    User = user
                });
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer to",
                    DateCreated = createDate.AddMinutes(15),
                    IsDeleted = false,
                    User = user
                });
                Add(article);
                _articleTagService.Add("Windows, Linux, OS", article);
            }
            // Opret 20
            for (var i = 80; i < 100; i++)
            {
                var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*7);
                var article = new Article
                {
                    Header = "En anden test artikel " + NumberToWords(i),
                    Description = "Beskrivelse",
                    Body = "Body",
                    CreateDate = createDate,
                    PublishDate = createDate,
                    Image = "Imagepath",
                    IsPublished = true,
                    User = user,
                    Comments = new List<ArticleComment>()
                };
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer et",
                    DateCreated = createDate.AddMinutes(5),
                    IsDeleted = false,
                    User = user
                });
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer to",
                    DateCreated = createDate.AddMinutes(15),
                    IsDeleted = false,
                    User = user
                });
                Add(article);
                _articleTagService.Add("Testtag, BenQ, Monitor", article);
            }
            // Opret 10
            for (var i = 20; i < 30; i++)
            {
                var createDate = DateTime.Now.AddDays(-i).AddMinutes(i*23);
                var article = new Article
                {
                    Header = "En tredje test artikel " + NumberToWords(i),
                    Description = "Beskrivelse",
                    Body = "Body",
                    CreateDate = createDate,
                    PublishDate = createDate,
                    Image = "Imagepath",
                    IsPublished = true,
                    User = user,
                    Comments = new List<ArticleComment>()
                };
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer et",
                    DateCreated = createDate.AddMinutes(5),
                    IsDeleted = false,
                    User = user
                });
                article.Comments.Add(new ArticleComment
                {
                    Article = article,
                    CommentBody = "Test comment nummer to",
                    DateCreated = createDate.AddMinutes(15),
                    IsDeleted = false,
                    User = user
                });
                Add(article);
                _articleTagService.Add("Højtalere, Bose, Lyd, Chromecast", article);
            }
        }

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number/1000000 > 0)
            {
                words += NumberToWords(number/1000000) + " million ";
                number %= 1000000;
            }

            if (number/1000 > 0)
            {
                words += NumberToWords(number/1000) + " thousand ";
                number %= 1000;
            }

            if (number/100 > 0)
            {
                words += NumberToWords(number/100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[]
                {
                    "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven",
                    "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
                };
                var tensMap = new[]
                {"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"};

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number/10];
                    if (number%10 > 0)
                        words += "-" + unitsMap[number%10];
                }
            }

            return words;
        }
    }
}