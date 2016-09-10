using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Services.Data.Mapping
{
    public class CMSSettingsMapping : EntityTypeConfiguration<CMSSettings>
    {

        public CMSSettingsMapping()
        {
            HasKey(x => x.Id);
            HasMany(x => x.StickyTags);
            HasOptional(x => x.StickyArticle1);
            HasOptional(x => x.StickyArticle1);
            HasOptional(x => x.StickyArticle2);
            HasOptional(x => x.StickyArticle3);
            HasOptional(x => x.StickyArticle1);
        }

    }
}
