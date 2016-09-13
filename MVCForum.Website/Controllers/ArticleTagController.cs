using System.Linq;
using System.Web.Mvc;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Utilities;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class ArticleTagController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICMSSettingsService _CMSSettingsService;

        public ArticleTagController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService,
            ICMSSettingsService cmsSettingsService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
            _CMSSettingsService = cmsSettingsService;
        }

        public ActionResult Index(string tag)
        {
            if (tag.IsNullEmpty())
                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var articleTag = _articleTagService.Get(tag);
                if (articleTag == null)
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NothingToDisplay"));

                var articles = articleTag.Articles.ToList();
                if (!articles.Any())
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                var articleFrontpageViewModels = articles.Select(x => new ArticleFrontpageViewModel
                {
                    Header = x.Header,
                    Image = x.Image,
                    PublishDate = x.PublishDate,
                    Slug = x.Slug,
                    UserName = x.User.UserName
                }).Take(50);
                var tagPageViewModel = new ArticleSectionViewModel
                {
                    Header = articleTag.Name,
                    ArticleFrontpageViewModels = articleFrontpageViewModels
                };
                return View(tagPageViewModel);
            }
        }
    }
}