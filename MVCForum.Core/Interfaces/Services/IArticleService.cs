using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleService
    {
        Article AddNewArticle(Article article, MembershipUser user);
        IEnumerable<Article> GetAll();
        Article Get(Guid articleId);
        int ArticleCount();
        IList<Article> GetNewestArticles(int amountToTake);
        IList<Article> GetByUser(Guid memberId, int amountToTake); // Skal man bruge MemberShipUser?
        bool Delete(Article article);
    }
}