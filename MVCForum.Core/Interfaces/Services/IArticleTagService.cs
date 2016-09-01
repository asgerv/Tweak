using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleTagService
    {
        ArticleTag Add(ArticleTag articleTag);
        void Add(string tags, Article article);
        void Delete(ArticleTag articleTag);
        ArticleTag Get(string tag);
        ArticleTag Get(Guid id);
        void UpdateTagName();
        IEnumerable<ArticleTag> GetAll();
        void CreateTestTags();
    }
}