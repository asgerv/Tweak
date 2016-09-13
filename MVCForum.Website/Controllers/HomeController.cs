using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                var articleSectionViewModels =
                    _CMSSettingsService.GetOrCreate().StickyTags.Select(x => new ArticleSectionViewModel
                    {
                        Header = x.Name,
                        ShowHeader = true,
                        ArticleFrontpageViewModels = x.Articles.Take(4).Select(a => new ArticleFrontpageViewModel
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
                var tagNavElementViewModels = articleTags.Select(x => new TagNavElementViewModel {Name = x.Name});
                return PartialView(tagNavElementViewModels);
            }
        }
    }
}