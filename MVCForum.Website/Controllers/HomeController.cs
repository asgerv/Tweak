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
        public HomeController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService) : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
        }

        // GET: Home
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Video()
        {
            return View();
        }

        public ActionResult Review()
        {
            return View();
        }

        public ActionResult _ArticleMain()
        {
            ArticlesPreviewViewModel viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Articles = _articleService.GetAll();

            
            return PartialView(viewmodel);
        }
        public ActionResult _Article_Grid4x2()
        {
            ArticlesPreviewViewModel viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Articles = _articleService.GetAll();
            return PartialView(viewmodel);
        }
    }
}