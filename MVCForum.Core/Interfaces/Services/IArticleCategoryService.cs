using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.DomainModel.Enums;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleCategoryService
    {
        ArticleCategory Add(string name, string description, int sortOrder, ArticleSection articleSection);
        void Delete(ArticleCategory articleCategory);
        ArticleCategory Edit(ArticleCategory articleCategory);
        ArticleCategory Get(Guid id);
        ArticleCategory Get(string slug);
        /// <summary>
        /// Returns a collection of all categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<ArticleCategory> GetAll();
        /// <summary>
        /// Returns a collection of all categories with the specified ArticleSection
        /// </summary>
        /// <param name="articleSection"></param>
        /// <returns></returns>
        IEnumerable<ArticleCategory> GetAllBySection(ArticleSection articleSection);
    }
}