using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Website.ViewModels
{
    public class ArticleSearchViewModel
    {
        public string Slug { get; set; }
        public string Header { get; set; }
        public DateTime PublishDate { get; set; }
        public string UserName { get; set; }
    }
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