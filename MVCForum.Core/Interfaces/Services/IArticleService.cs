using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleService
    {
        Article Add(Article article);
        void PublishArticle(Article article);
        Article AddNewArticle(string header, string description, string body, string image, MembershipUser user);
        bool Delete(Article article);
        IEnumerable<Article> GetAll();
        Article Get(Guid articleId);
        Article Get(string slug);
        /// <summary>
        /// Returns a list of articles, sorted by Views
        /// </summary>
        /// <param name="thisArticleId">Set to null if not needed</param>
        /// <param name="maxAgeInDays"></param>
        /// <param name="amountToTake"></param>
        /// <returns></returns>
        IList<Article> GetMostPopular(Guid? thisArticleId, int maxAgeInDays, int amountToTake);

        /// <summary>
        /// Returns a list of related articles, based on 
        /// and ordered by, the amount of ArticleTags in common
        /// </summary>
        /// <param name="article"></param>
        /// <param name="amountToTake"></param>
        /// <returns></returns>
        IList<Article> GetRelated(Article article, int amountToTake);
        IList<Article> GetNewest(int amountToTake);
        IList<Article> GetNewestPublished(int amountToTake);
        IList<Article> GetByUser(Guid memberId, int amountToTake); // Skal man bruge MemberShipUser?
        IList<Article> GetAllAllowed(Guid memberId);
        IList<Article> Search(int amountToTake, string keyword);
        int Count();
        void Edit(Article article);
        void WipeDatabase();
        void CreateTestData(MembershipUser user);
    }
}