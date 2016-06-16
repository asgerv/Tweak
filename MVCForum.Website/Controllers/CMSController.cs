using System.Web.Mvc;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;

namespace MVCForum.Website.Controllers
{
    public class CMSController : BaseController
    {
        private readonly IArticleService _articleService;

        public CMSController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IArticleService articleService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
        }

        // GET: CMS
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Articles()
        {
            return View();
        }

        // GET: CMS/NewArticle
        public ActionResult NewArticle()
        {
            return View();
        }

        // POST: Article
        [HttpPost]
        public ActionResult NewArticle([Bind(Include = "Header, Description, Body")] Article article)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    //try
                    //{
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    _articleService.AddNewArticle(article, loggedOnUser);
                    unitOfWork.Commit();
                    return RedirectToAction("Index");
                    //}
                    //catch (Exception ex)
                    //{

                    //    unitOfWork.Rollback();
                    //    LoggingService.Error(ex);
                    //    throw;
                    //}
                }
            }

            return View(article);
        }

        public ActionResult Comments()
        {
            return View();
        }

        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult Nyheder()
        {
            return View();
        }

        public ActionResult FrontpageSettings()
        {
            return View();
        }

        public ActionResult GeneralSettings()
        {
            return View();
        }

        public ActionResult Tags()
        {
            return View();
        }
    }
}