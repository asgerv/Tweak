using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
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
        public int ArticlesPublished { get; set; }
        public int Comments { get; set; }
        public int PageViews { get; set; }
    }

    public class AddArticleViewModel
    {
        public AddArticleViewModel()
        {
            Image = "/Images/Default.jpeg";
        }
        //[Required] <-- ??
        [Display(Name = "Overskrift")]
        [StringLength(70, ErrorMessage = "Header cannot be longer than 70 characters.")]
        public string Header { get; set; }
        [Display(Name = "Beskrivelse")]
        [StringLength(240, ErrorMessage = "Description cannot be longer than 240 characters.")]
        public string Description { get; set; }
        [Display(Name = "Brødtekst")]
        [AllowHtml]
        public string Body { get; set; }
        [Display(Name = "Billede (URL)")]
        public string Image { get; set; }
        [Display(Name = "Status")]
        public bool IsPublished { get; set; }
        [Display(Name = "Tags")]
        public IEnumerable<string> SelectedTags { get; set; }
        public IEnumerable<SelectListItem> AvailableTags { get; set; }
    }

    public class EditArticleViewModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Overskrift")]
        [StringLength(70, ErrorMessage = "Header cannot be longer than 70 characters.")]
        public string Header { get; set; }
        [Display(Name = "Beskrivelse")]
        [StringLength(240, ErrorMessage = "Description cannot be longer than 240 characters.")]
        public string Description { get; set; }
        [Display(Name = "Brødtekst")]
        [AllowHtml]
        public string Body { get; set; }
        [Display(Name = "Billede (URL)")]
        public string Image { get; set; }
        [Display(Name = "Status")]
        public bool IsPublished { get; set; }
        [Display(Name = "Tags")]
        public IEnumerable<string> SelectedTags { get; set; }
        public IEnumerable<SelectListItem> AvailableTags { get; set; }
    }

    public class ArticlesViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
    }

    public class CommentsViewModel
    {
        public IList<ArticleComment> ArticleComments { get; set; }
    }

    public class TagsViewModel
    {
        public List<ArticleTag> ArticleTags { get; set; }
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