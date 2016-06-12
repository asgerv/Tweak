using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Services.Data.Mapping
{
    public class ArticleMapping : EntityTypeConfiguration<Article>
    {
        public ArticleMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.CreateDate).IsRequired();
            Property(x => x.DateModified).IsOptional();
            Property(x => x.Header).IsRequired();
            Property(x => x.Description).IsRequired();
            Property(x => x.Body).IsRequired();
            Property(x => x.IsDeleted).IsRequired();
            Property(x => x.IsPublished).IsRequired();

            HasMany(a => a.Comments)
                .WithRequired(c => c.Article)
                .Map(x => x.MapKey("Article_Id"));

            HasMany(x => x.Tags)
                .WithMany(a => a.Articles)
                .Map(m =>
                {
                    m.ToTable("Article_Tag");
                    m.MapLeftKey("Article_Id");
                    m.MapRightKey("ArticleTag_Id");
                });
        }
    }
}