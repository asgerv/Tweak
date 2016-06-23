using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using System.Web.Mvc;

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

    public class CreateArticleViewModel
    {
        public string Header { get; set; }
        public string Description { get; set; }        
        public string Image { get; set; }
        public bool IsPublished { get; set; }
        [AllowHtml]
        public string Body { get; set; }
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