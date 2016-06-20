using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;

namespace MVCForum.Services
{
    public class ArticleTagService : IArticleTagService
    {
        private readonly MVCForumContext _context; // lol

        public ArticleTagService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public ArticleTag Add(ArticleTag articleTag)
        {
            throw new NotImplementedException();
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
    }
}