using System;
using MVCForum.Utilities;
using MVCForum.Domain.Constants;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MVCForum.Domain.DomainModel.CMS
{
    public class ArticleComment : Entity
    {
        public ArticleComment()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string CommentBody { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public MembershipUser User { get; set; }
        public virtual Article Article { get; set; }
        public Guid? InReplyTo { get; set; }


    }
}