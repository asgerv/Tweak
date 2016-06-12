using System;
using MVCForum.Utilities;

namespace MVCForum.Domain.DomainModel.CMS
{
    public class ArticleComment : Entity
    {
        public ArticleComment()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string CommentBody { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public MembershipUser User { get; set; }
        public virtual Article Article { get; set; }
    }
}