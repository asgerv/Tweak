using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.Enums;

namespace MVCForum.Website.ViewModels
{
    public class ArticleShowViewModel
    {
        public string Slug { get; set; }
        public Guid Id { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }

        [UIHint("WrittenTimeAgo")]
        public DateTime PublishDate { get; set; }

        public string Image { get; set; }
        public MembershipUser User { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public List<string> Tags { get; set; }
    }

    public class ArticleSearchViewModel
    {
        public string Slug { get; set; }
        public string Header { get; set; }
        public DateTime PublishDate { get; set; }
        public string UserName { get; set; }
    }

    public class ArticleFrontpageViewModel
    {
        public string Slug { get; set; }
        public string Header { get; set; }

        [UIHint("WrittenTimeAgo")]
        public DateTime PublishDate { get; set; }

        public string UserName { get; set; }
        public string Image { get; set; }
    }

    public class CommentViewModel
    {
        [UIHint(AppConstants.EditorType)]
        [AllowHtml]
        public string CommentBody { get; set; }

        public Guid? InReplyTo { get; set; }
        public string ArticleSlug { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? ArticleId { get; set; }
    }

    public class ArticleMainViewModel
    {
        public ArticleFrontpageViewModel Article1 { get; set; }
        public ArticleFrontpageViewModel Article2 { get; set; }
        public ArticleFrontpageViewModel Article3 { get; set; }
        public ArticleFrontpageViewModel Article4 { get; set; }
    }

    public class ArticleSectionViewModel
    {
        public string Header { get; set; }
        public bool ShowHeader { get; set; }
        public IEnumerable<ArticleFrontpageViewModel> ArticleFrontpageViewModels { get; set; }
    }

    public class ArticleCategoryNavViewModel
    {
        public ArticleSection ArticleSection { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public int SortOrder { get; set; }
    }

    public class ArticleCategoryViewModel
    {
        public string CategoryName { get; set; }
        public ArticleSection ArticleSection { get; set; }
        public ArticleSectionViewModel ArticleSectionViewModel { get; set; }
    }


    public class AdViewModel
    {
        public int zone { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int id { get; set; }
    }
}