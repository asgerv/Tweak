using System;
using System.Collections.Generic;
using System.Linq;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;
using MVCForum.Utilities;

namespace MVCForum.Services
{
    public class ArticleTagService : IArticleTagService
    {
        private readonly MVCForumContext _context;

        public ArticleTagService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public ArticleTag Add(ArticleTag articleTag)
        {
            return _context.ArticleTag.Add(articleTag);
        }

        public void Add(string tags, Article article)
        {
            if (!string.IsNullOrEmpty(tags))
            {
                tags = StringUtils.SafePlainText(tags);

                var splitTags = tags.Replace(" ", "").Split(',');

                if (article.Tags == null)
                {
                    article.Tags = new List<ArticleTag>();
                }

                var newTagNames = splitTags.Select(tag => tag);
                var newTags = new List<ArticleTag>();
                var existingTags = new List<ArticleTag>();

                foreach (var newTag in newTagNames.Distinct())
                {
                    var tag = GetTagName(newTag);
                    if (tag != null)
                    {
                        // Exists
                        existingTags.Add(tag);
                    }
                    else
                    {
                        // Doesn't exists
                        var nTag = new ArticleTag
                        {
                            Name = newTag
                            //Slug = ServiceHelpers.CreateUrl(newTag)
                        };

                        Add(nTag);
                        newTags.Add(nTag);
                    }
                }

                newTags.AddRange(existingTags);
                article.Tags = newTags;
            }
        }

        public void Delete(ArticleTag articleTag)
        {
            throw new NotImplementedException();
        }

        public ArticleTag Get(string tag)
        {
            throw new NotImplementedException();
        }

        public ArticleTag Get(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void UpdateTagName()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ArticleTag> GetAll()
        {
            throw new NotImplementedException();
        }

        public ArticleTag GetTagName(string tag)
        {
            tag = StringUtils.SafePlainText(tag);
            return _context.ArticleTag.FirstOrDefault(x => x.Name == tag);
        }
    }
}