﻿using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.Activity;
using MVCForum.Website.Application;
using MVCForum.Website.Application.ActionFilterAttributes;

namespace MVCForum.Website.ViewModels
{
    // tilføjelse #ændring
    public class TagNavElementViewModel
    {
        public string Name { get; set; }
    }
    // tilføjelse #ændring
    public class TagPageViewModel
    {
        public string TagName { get; set; }
        public IEnumerable<ArticleFrontpageViewModel> ArticleFrontpageViewModels { get; set; }
    }
    public class ListCategoriesViewModels
    {
        public MembershipUser MembershipUser { get; set; }
        public MembershipRole MembershipRole { get; set; }
    }

    public class AllRecentActivitiesViewModel
    {
        public PagedList<ActivityBase> Activities { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
    }

    public class TermsAndConditionsViewModel
    {
        public string TermsAndConditions { get; set; }

        [ForumMvcResourceDisplayName("TermsAndConditions.Label.Agree")]
        [MustBeTrue(ErrorMessage = "TermsAndConditions.Label.AgreeError")]
        public bool Agree { get; set; }
    }
}