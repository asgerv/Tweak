using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class DashboardViewModel
    {
        
    }
    public class NewArticleViewModel
    {

    }

    public class ArticlesViewModel
    {

    }
    public class CommentsViewModel
    {

    }

    public class StatisticsViewModel
    {
        
    }
    
}