using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Services.Data.Context;

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
            throw new NotImplementedException();
        }
    }
}