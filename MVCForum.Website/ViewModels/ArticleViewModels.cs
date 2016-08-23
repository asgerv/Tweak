using MVCForum.Domain.Constants;
using MVCForum.Domain.DomainModel.CMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCForum.Website.ViewModels
{
    public class ArticleViewModels
    {

        public class CommentViewModel
        {
            [UIHint(AppConstants.EditorType), AllowHtml]
            public string CommentBody { get; set; }
            public Guid? InReplyTo { get; set; }
            public Guid? ArticleId { get; set; }
            
            

        }
    }
}