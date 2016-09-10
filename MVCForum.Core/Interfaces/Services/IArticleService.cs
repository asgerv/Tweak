using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleService
    {
        Article Add(Article article); 
        Article AddNewArticle(string header, string description, string body, string image, bool isPublished, DateTime publishDate, MembershipUser user);
        bool Delete(Article article);
        IEnumerable<Article> GetAll();
        Article Get(Guid articleId);
        Article Get(string slug);
        IList<Article> Search(int amountToTake, string keyword);
        int Count();
        IList<Article> GetNewest(int amountToTake);
        IList<Article> GetNewestPublished(int amountToTake);
        IList<Article> GetByUser(Guid memberId, int amountToTake); // Skal man bruge MemberShipUser?
        IList<Article> GetAllAllowed(Guid memberId);
        void Edit(Article article);
        void WipeDatabase();
        void CreateTestData(MembershipUser user);
    }
}