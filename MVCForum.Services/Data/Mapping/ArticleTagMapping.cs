using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Services.Data.Mapping
{
    internal class ArticleTagMapping : EntityTypeConfiguration<ArticleTag>
    {
        public ArticleTagMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.Name).IsRequired();

        }
    }
}