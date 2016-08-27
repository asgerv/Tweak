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
            Property(x => x.FrontPageCategory1).IsOptional();
            Property(x => x.FrontPageCategory2).IsOptional();
            Property(x => x.FrontPageCategory3).IsOptional();
            Property(x => x.FrontPageCategory4).IsOptional();
            Property(x => x.ArticleSticky1).IsOptional();
            Property(x => x.ArticleSticky2).IsOptional();
            Property(x => x.ArticleSticky3).IsOptional();
            Property(x => x.ArticleSticky4).IsOptional();
        }

    }
}
