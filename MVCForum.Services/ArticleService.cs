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
        private readonly MVCForumContext _context;

        public ArticleService(IArticleCommentService articleCommentService, IMVCForumContext context)
        {
            _articleCommentService = articleCommentService;
            _context = context as MVCForumContext;
        }
        public Article Add(Article article)
        {
            return _context.Article.Add(article);
        }
        public Article AddNewArticle(string header, string description, string body, string image, bool isPublished, MembershipUser user)
        {
            var article = new Article
            {
                Header = header,
                Description = description,
                Body = body,
                User = user,
                CreateDate = DateTime.Now,
                IsPublished = isPublished,
                Image = image
            };            
            // Skal objektets lister muligvis initialiseres?
            return _context.Article.Add(article);
        }

        public void Delete(Article article)
        {
            // Fjern alle comments
            foreach (var comment in article.Comments.ToList())
            {
                _articleCommentService.Delete(comment);
            }
            // Slet article
            _context.Article.Remove(article);
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
    }
}