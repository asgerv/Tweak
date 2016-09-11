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

        public ArticleComment Add(string commentBody, Guid? InReplyTo, Guid? ArticleId, MembershipUser user)
        {
            // Tilføjer en ArticleComment
            var comment = new ArticleComment();
            comment.Article = _context.Article.Find(ArticleId);
            comment.User = user;
            comment.DateCreated = DateTime.Now;
            comment.IsDeleted = false;
            var reply = _context.ArticleComment.FirstOrDefault(x => x.Id == InReplyTo);
            if (reply != null)
                comment.InReplyTo = _context.ArticleComment.Find(InReplyTo).Id;
            comment.CommentBody = commentBody;
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

        public void Delete(Guid articleCommentId)
        {
            var articleComment = _context.ArticleComment.Find(articleCommentId);
            articleComment.IsDeleted = true;
            Update(articleComment);
        }

        public void Delete(ArticleComment comment)
        {
            comment.IsDeleted = true;
            Update(comment);
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
                .Include(x => x.Article).Include(x => x.User)
                //.Include(x => x.Article)
                .Where(x => x.Article.Id == articleId).ToList();
        }

        public ArticleComment GetComment(Guid? commentId)
        {
            return _context.ArticleComment.Find(commentId);
        }

        public IEnumerable<ArticleComment> GetAll()
        {
            return _context.ArticleComment
                .Include(x => x.User)
                .Include(x => x.Article);
        }

        public void Edit(Guid articleCommentId, string commentBody)
        {
            var articleComment = _context.ArticleComment.Find(articleCommentId);
            articleComment.CommentBody = commentBody;
            Update(articleComment);
        }
    }
}