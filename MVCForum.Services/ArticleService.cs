using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;

namespace MVCForum.Services
{
    public class ArticleService : IArticleService
    {
        private readonly MVCForumContext _context;

        public ArticleService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public Article AddNewArticle(Article article, MembershipUser user)
        {
            article.User = user;
            article.CreateDate = DateTime.Now;
            article.IsDeleted = false;
            article.IsPublished = true;
            // Skal objektets lister muligvis initialiseres?
            return _context.Article.Add(article);
        }

        public IEnumerable<Article> GetAll()
        {
            throw new NotImplementedException();
        }

        public Article Get(Guid articleId)
        {
            throw new NotImplementedException();
        }

        public int ArticleCount()
        {
            throw new NotImplementedException();
        }

        public IList<Article> GetNewestArticles(int amountToTake)
        {
            throw new NotImplementedException();
        }

        public IList<Article> GetByUser(Guid memberId, int amountToTake)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Article article)
        {
            throw new NotImplementedException();
        }
    }
}