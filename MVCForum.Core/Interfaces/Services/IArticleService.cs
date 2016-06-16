using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleService
    {
        Article Add(Article article, MembershipUser user);
        void Delete(Article article);
        IEnumerable<Article> GetAll();
        Article Get(Guid articleId);
        int Count();
        IList<Article> GetNewest(int amountToTake);
        IList<Article> GetByUser(Guid memberId, int amountToTake); // Skal man bruge MemberShipUser?
    }
}