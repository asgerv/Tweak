using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Services.Data.Mapping
{
    public class ArticleCategoryMapping : EntityTypeConfiguration<ArticleCategory>
    {
        public ArticleCategoryMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired();
            Property(x => x.Description).IsRequired();
            Property(x => x.SortOrder).IsRequired();
            Property(x => x.Description).IsRequired();
            Property(x => x.ArticleSection).IsRequired();

            Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnAnnotation("Index", new IndexAnnotation(
                    new IndexAttribute("IX_ArticleCategory_Slug", 1) { IsUnique = true }));

            HasMany(a => a.Articles)
                .WithRequired(c => c.ArticleCategory)
                .Map(x => x.MapKey("ArticleCategory_Id"));
        }
    }
}