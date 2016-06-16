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
    public class ArticleCommentService : IArticleCommentService
    {
        private readonly MVCForumContext _context;

        public ArticleCommentService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public ArticleComment Add(ArticleComment comment, Article article, MembershipUser user)
        {
            // TODO: TEST
            comment.Article = article;
            comment.User = user;
            comment.DateCreated = DateTime.Now;
            comment.IsDeleted = false;
            return _context.ArticleComment.Add(comment);
        }

        public void Delete(ArticleComment articleComment)
        {
            // TODO: TEST
            // Fjerner ArticleComment fra Article
            var article = articleComment.Article;
            _context.Article.Remove(article);
            // Sletter ArticleComment
            _context.ArticleComment.Remove(articleComment);
        }

        public IList<ArticleComment> GetByArticle(Guid articleId)
        {
            // TODO: TEST
            return _context.ArticleComment
                .Include(x => x.Article)
                //.Include(x => x.Article)
                .Where(x => x.Article.Id == articleId).ToList();
        }
    }
}