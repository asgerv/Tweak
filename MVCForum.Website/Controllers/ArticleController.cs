using System;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class ArticleController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;

        public ArticleController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
        }

        public ActionResult _ArticleMain()
        {
            var viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Articles = _articleService.GetNewest(4);


            return PartialView(viewmodel);
        }

        public ActionResult _Article_Grid4x2(int? number)
        {
            var viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Tag = "Chosen Tag";
            viewmodel.Articles = _articleService.GetNewest(13);
            return PartialView(viewmodel);
        }


        public ActionResult Nyhed(Guid id)
        {
            // TODO ??
            var viewmodel = new Article();
            viewmodel = _articleService.Get(id);

            return View(viewmodel);
        }
    }
}