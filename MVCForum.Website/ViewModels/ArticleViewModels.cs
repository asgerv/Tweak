using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.Constants;

namespace MVCForum.Website.ViewModels
{
    public class CommentViewModel
    {
        [UIHint(AppConstants.EditorType), AllowHtml]
        public string CommentBody { get; set; }

        public Guid? InReplyTo { get; set; }
        public string ArticleSlug { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? ArticleId { get; set; }
    }
}