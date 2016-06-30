using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Website.ViewModels
{
    public class AddArticleCommentViewModel
    {
        public Article Article { get; set; }
        public ArticleComment ArticleComment { get; set; }
        public MembershipUser User { get; set; }
    }

    public class TestViewModel
    {
        public string S { get; set; }
    }

    public class DashboardViewModel
    {
    }

    public class AddArticleViewModel
    {
        public AddArticleViewModel()
        {
            Image = "/Images/Default.jpeg";
        }
        [StringLength(70, ErrorMessage = "Header cannot be longer than 70 characters.")]
        public string Header { get; set; }
        [StringLength(240, ErrorMessage = "Description cannot be longer than 240 characters.")]
        public string Description { get; set; }
        public string Image { get; set; }
        public string Tags { get; set; }
        public bool IsPublished { get; set; }

        [AllowHtml]
        public string Body { get; set; }
    }

    public class EditArticleViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DateModified { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string Image { get; set; }
        public bool IsPublished { get; set; }
        public virtual MembershipUser User { get; set; }
        public string Tags { get; set; }
    }

    public class ArticlesViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }

    public class CommentsViewModel
    {
    }

    public class StatisticsViewModel
    {
    }

    public class ArticlesPreviewViewModel
    {
        public string Tag { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
}