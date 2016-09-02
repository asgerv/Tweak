using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
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
            Property(x => x.Image).IsRequired();
            Property(x => x.IsPublished).IsRequired();
            Property(x => x.PublishDate).IsRequired();
            Property(x => x.Views).IsOptional();

            Property(x => x.Slug).IsRequired().HasMaxLength(450).HasColumnAnnotation("Index",
                                   new IndexAnnotation(new IndexAttribute("IX_Article_Slug", 1) { IsUnique = true }));

            HasRequired(t => t.User)
                .WithMany(t => t.Articles)
                .Map(m => m.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);
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