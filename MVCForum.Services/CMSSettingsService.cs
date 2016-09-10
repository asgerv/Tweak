using MVCForum.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Services.Data.Context;
using System.Data.Entity;
using MVCForum.Domain.Interfaces;

namespace MVCForum.Services
{
    public class CMSSettingsService : ICMSSettingsService

    {
        private readonly MVCForumContext _context;

        public CMSSettingsService(IMVCForumContext context)
        {
            _context = context as MVCForumContext;
        }

        public CMSSettings GetOrCreate()
        {
            CMSSettings settings;
            if (!_context.CMSSetting.Any())
            {
                settings = new CMSSettings();
                settings.StickyTags = new List<ArticleTag>();
                settings.StickyArticle1 = null;
                settings.StickyArticle2 = null;
                settings.StickyArticle3 = null;
                settings.StickyArticle4 = null;
                _context.CMSSetting.Add(settings);
                return settings;
            }
            else
            {
               settings = _context.CMSSetting.FirstOrDefault();
                return settings;
            }
        }

        public void Edit(CMSSettings settings)
        {
            _context.Entry(settings).State = EntityState.Modified;
        }
    }
}
