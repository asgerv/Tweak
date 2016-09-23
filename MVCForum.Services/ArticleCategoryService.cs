using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.DomainModel.Enums;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;
using MVCForum.Utilities;

namespace MVCForum.Services
{
    public class ArticleCategoryService : IArticleCategoryService
    {
        private readonly MVCForumContext _context;

        public ArticleCategoryService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public ArticleCategory Add(string name, string description, int sortOrder, ArticleSection articleSection)
        {
           return _context.ArticleCategory.Add(
               new ArticleCategory
               {
                   Name = name,
                   Description = description,
                   SortOrder = sortOrder,
                   ArticleSection = articleSection,
                   Slug = ServiceHelpers.GenerateSlug(name, GetArticleCategoryBySlugLike(ServiceHelpers.CreateUrl(name)), null),
                   Articles = new List<Article>()
               });
        }

        public void Delete(ArticleCategory articleCategory)
        {
            _context.ArticleCategory.Remove(articleCategory);
        }

        public ArticleCategory Edit(ArticleCategory articleCategory)
        {
            articleCategory.Slug = ServiceHelpers.GenerateSlug(articleCategory.Name,
               GetArticleCategoryBySlugLike(ServiceHelpers.CreateUrl(articleCategory.Name)), articleCategory.Slug);
            _context.Entry(articleCategory).State = EntityState.Modified;
            return articleCategory;
        }

        public ArticleCategory Get(Guid id)
        {
            return _context.ArticleCategory.FirstOrDefault(x => x.Id == id);
        }

        public ArticleCategory Get(string slug)
        {
            return _context.ArticleCategory
                .Include(x => x.Articles)
                .FirstOrDefault(x => x.Slug == slug);
        }

        public IEnumerable<ArticleCategory> GetAll()
        {
            return _context.ArticleCategory.Include(x => x.Articles);
        }

        public IEnumerable<ArticleCategory> GetAllBySection(ArticleSection articleSection)
        {
            return _context.ArticleCategory
                .Include(x => x.Articles)
                .Where(x => x.ArticleSection == articleSection);
        }

        #region Helpers

        public IList<ArticleCategory> GetArticleCategoryBySlugLike(string slug)
        {
            return _context.ArticleCategory
               .Where(x => x.Slug.Contains(slug))
               .ToList();
        }

        #endregion
    }
}