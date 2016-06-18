using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleCommentService
    {
        ArticleComment Add(ArticleComment comment, Article article, MembershipUser user);
        void DeleteFromDb(ArticleComment articleComment);
        void Delete(ArticleComment articleComment);
        void Update(ArticleComment articleComment);
        void UpdateBody(string newBody, Guid articleCommentId);
        IList<ArticleComment> GetByArticle(Guid articleId);
    }
}