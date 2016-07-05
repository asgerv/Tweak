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
            // Tilføjer en ArticleComment
            comment.Article = article;
            comment.User = user;
            comment.DateCreated = DateTime.Now;
            comment.IsDeleted = false;
            return _context.ArticleComment.Add(comment);
        }

        public void DeleteFromDb(ArticleComment articleComment)
        {
            // Fjerner ArticleComment fra Article - Er dette nødvendigt?
            var article = articleComment.Article;
            article.Comments.Remove(articleComment);
            // Sletter ArticleComment
            _context.ArticleComment.Remove(articleComment);
        }

        public void Delete(ArticleComment articleComment)
        {
            articleComment.IsDeleted = true;
            Update(articleComment);
        }

        public void Update(ArticleComment articleComment)
        {
            //if (TryUpdateModel)
            _context.Entry(articleComment).State = EntityState.Modified;
        }

        public void UpdateBody(string newBody, Guid articleCommentId)
        {
            var article = _context.ArticleComment.FirstOrDefault(a => a.Id == articleCommentId);
            if (article != null)
            {
                article.CommentBody = newBody;
                Update(article);
            }
        }

        public IList<ArticleComment> GetByArticle(Guid articleId)
        {
            return _context.ArticleComment
                .Include(x => x.Article)
                //.Include(x => x.Article)
                .Where(x => x.Article.Id == articleId).ToList();
        }

        public IEnumerable<ArticleComment> GetAll()
        {
            return _context.ArticleComment;
        }
    }
}