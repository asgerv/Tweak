﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.DomainModel.Enums;

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

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Overskrift")]
        [StringLength(70, ErrorMessage = "Header cannot be longer than 70 characters.")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Beskrivelse")]
        [StringLength(240, ErrorMessage = "Description cannot be longer than 240 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
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

        [Display(Name = "Kategori")]
        public string Category { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
    }

    public class EditArticleViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Overskrift")]
        [StringLength(70, ErrorMessage = "Header cannot be longer than 70 characters.")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Beskrivelse")]
        [StringLength(240, ErrorMessage = "Description cannot be longer than 240 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
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

        [Display(Name = "Kategori")]
        public string Category { get; set; }

        public List<SelectListItem> AvailableCategories { get; set; }
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

    public class ArticleTagViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ArticleCount { get; set; }
        public bool IsFrontpage { get; set; }
    }

    public class StatisticsViewModel
    {
    }

    public class AddCategoryViewModel
    {
        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Vælg et tal mellem 0 og 100.")]
        [Display(Name = "Sortering")]
        public int SortOrder { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Sektion")]
        public ArticleSection Section { get; set; }

        public IEnumerable<SelectListItem> AvailableSections { get; set; }
    }

    public class EditCategoryViewModel
    {
        public string Slug { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Navn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Beskrivelse")]
        public string Description { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Vælg et tal mellem 0 og 100.")]
        [Display(Name = "Sortering")]
        public int SortOrder { get; set; }

        [Required(ErrorMessage = "Dette felt er krævet.")]
        [Display(Name = "Sektion")]
        public ArticleSection Section { get; set; }

        public IEnumerable<SelectListItem> AvailableSections { get; set; }
    }
}