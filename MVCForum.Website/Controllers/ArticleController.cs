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
    public class ArticleController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        public ArticleController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
        }
        public ActionResult _ArticleMain()
        {
            ArticlesPreviewViewModel viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Articles = _articleService.GetNewest(4);


            return PartialView(viewmodel);
        }

        public ActionResult _Article_Grid4x2(int? number)
        {
            ArticlesPreviewViewModel viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Tag = "Chosen Tag";
            viewmodel.Articles = _articleService.GetNewest(4);
            return PartialView(viewmodel);
        }

        
    }
}