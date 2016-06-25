using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class CMSController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;

        public CMSController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
        }

        // GET: CMS
        public ActionResult Index()
        {
            return View();
        }

        // GET: /cms/newarticle/
        public ActionResult NewArticle()
        {
            return View();
        }

        // POST: Article
        [HttpPost]
        public ActionResult NewArticle(
            [Bind(Include = "Header, Description, Body, Image, IsPublished")] AddArticleViewModel addArticleViewModel)
        {
            Article newArticle;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                        newArticle = _articleService.AddNewArticle(addArticleViewModel.Header,
                            addArticleViewModel.Description, addArticleViewModel.Body,
                            addArticleViewModel.Image, addArticleViewModel.IsPublished, loggedOnUser);

                        // TODO: Tilføj tags.

                        unitOfWork.Commit();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return View(); // TODO: Gå til artikel "newArticle"
        }


        [HttpGet] // Er du sikker på det skal bruges?
        public ActionResult EditArticle(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var article = _articleService.Get(id.Value);
            if (article == null)
                return HttpNotFound();
            return View(article);
        }

        [HttpPost]
        public ActionResult EditArticle(AddArticleViewModel model)
        {
            return View();
        }
        
        // GET: cms/deletearticle/id
        [HttpGet]
        public ActionResult DeleteArticle(Guid? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var article = _articleService.Get(id.Value);
            if (article == null)
                return HttpNotFound();
            return View(article);
        }
        // POST: cms/deletearticle/id
        [HttpPost, ActionName("DeleteArticle")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteArticleConfirmed(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Vi skal have noget logging over hvem der har slettet artikler
                       
                        _articleService.Delete(_articleService.Get(id));
                        unitOfWork.Commit();
                        return RedirectToAction("Articles");
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return View();
        }
        public ActionResult Articles()
        {
            var viewmodel = new ArticlesViewModel();
            viewmodel.Articles = _articleService.GetAll();
            return View(viewmodel);
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
        [HttpPost]
        public ActionResult Tags(TestViewModel vm)
        {
            throw new NotImplementedException();
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var comments = _articleCommentService.GetByArticle(new Guid("5775e572-bf61-4c53-a180-a626013d35b6"));

                _articleCommentService.Delete(comments.First());
                unitOfWork.Commit();
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddComment(TestViewModel vm)
        {
            throw new NotImplementedException();
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                var article = _articleService.GetNewest(1).First();
                var comment = new ArticleComment { CommentBody = vm.S };

                _articleCommentService.Add(comment, article, loggedOnUser);
                unitOfWork.Commit();
                return View();
            }
        }
        public string Upload(HttpPostedFileBase file)
        {
            string path;
            var saveloc = "~/Images/";
            var relativeloc = "/Images/";
            var filename = file.FileName;

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    path = Path.Combine(HttpContext.Server.MapPath(saveloc), Path.GetFileName(filename));
                    file.SaveAs(path);
                }
                catch (Exception e)
                {
                    return "<script>alert('Failed: " + e + "');</script>";
                }
            }
            else
            {
                return "<script>alert('Failed: Unkown Error. This form only accepts valid images.');</script>";
            }

            return "<script>top.$('.mce-btn.mce-open').parent().find('.mce-textbox').val('" + relativeloc + filename +
                   "').closest('.mce-window').find('.mce-primary').click();</script>";
        }
    }
}