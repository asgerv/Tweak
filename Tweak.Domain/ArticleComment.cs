using System;
using MVCForum.Domain.DomainModel;

namespace Tweak.Domain
{
    public class ArticleComment
    {
        public Guid Id { get; set; }
        public MembershipUser User { get; set; }
        public string CommentContent { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Article Article { get; set; }
    }
}