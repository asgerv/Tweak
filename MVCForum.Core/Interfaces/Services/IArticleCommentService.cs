using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleCommentService
    {
        ArticleComment Add(ArticleComment comment, Article article, MembershipUser user);
        void Delete(ArticleComment articleComment);
        IList<ArticleComment> GetByArticle(Guid articleId);
    }
}