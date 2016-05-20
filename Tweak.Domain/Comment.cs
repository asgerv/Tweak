using System;
using MVCForum.Domain.DomainModel;

namespace Tweak.Domain
{
    internal class Comment
    {
        public Guid Id { get; set; }
        public MembershipUser User { get; set; }
        public string CommentContent { get; set; }
        public DateTime DateCreated { get; set; }
    }
}