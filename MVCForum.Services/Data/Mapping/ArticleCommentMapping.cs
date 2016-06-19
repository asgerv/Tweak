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

            HasRequired(t => t.User)
                .WithMany(t => t.ArticleComments)
                .Map(m => m.MapKey("MembershipUser_Id"))
                .WillCascadeOnDelete(false);
            //HasRequired(t => t.Article)
            //    .WithMany(t => t.Comments)
            //    .Map(m => m.MapKey("Article_Id"))
            //    .WillCascadeOnDelete(false);
        }
    }
}