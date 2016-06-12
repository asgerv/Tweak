using System.Data.Entity.ModelConfiguration;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Services.Data.Mapping
{
    internal class ArticleCommentMapping : EntityTypeConfiguration<ArticleComment>
    {
        public ArticleCommentMapping()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).IsRequired();
            Property(x => x.CommentBody).IsRequired();
            Property(x => x.DateCreated).IsRequired();
            Property(x => x.IsDeleted).IsRequired();
        }
    }
}