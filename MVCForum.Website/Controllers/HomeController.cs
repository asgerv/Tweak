using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel.Enums;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICMSSettingsService _CMSSettingsService;
        public HomeController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService, ICMSSettingsService cmsSettingsService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
            _CMSSettingsService = cmsSettingsService;
        }

        // GET: Home
        public ActionResult Index()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // 

                var articleSectionViewModels = new List<ArticleSectionViewModel>();
                articleSectionViewModels.Add(new ArticleSectionViewModel
                {
                    Header = "Nyhed",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Nyhed)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                });
                articleSectionViewModels.Add(new ArticleSectionViewModel
                {
                    Header = "Video",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Video)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                });
                articleSectionViewModels.Add(new ArticleSectionViewModel
                {
                    Header = "Test",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Test)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                });
                return View(articleSectionViewModels);
            }
        }
        public ActionResult Team()
        {
            return View();
        }
        [Route("om")]
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult _TagsElements()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var articleTags = _CMSSettingsService.GetOrCreate().StickyTags;
                var tagNavElementViewModels = articleTags.OrderByDescending(x => x.Articles.Count).Select(x => new TagNavElementViewModel { Name = x.Name });
                return PartialView(tagNavElementViewModels);
            }
        }
    }
}