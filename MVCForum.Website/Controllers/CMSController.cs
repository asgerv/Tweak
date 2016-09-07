using System;
using System.IO;
using System.Linq;
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
        private readonly ICMSSettingsService _CMSSettingsService;

        public CMSController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
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

        // GET: CMS
        public ActionResult Index()
        {
            return View();
        }

        // GET: /cms/newarticle/
        public ActionResult NewArticle()
        {
            var vm = new AddArticleViewModel
            {
                AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name))
            };
            return View(vm);
        }

        // POST: Article
        [HttpPost]
        public ActionResult NewArticle(AddArticleViewModel vm)
        {
            Article newArticle;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);

                        newArticle = _articleService.AddNewArticle(vm.Header,
                            vm.Description, vm.Body,
                            vm.Image, vm.IsPublished, DateTime.Now, loggedOnUser);

                        // Gemmer article i database, så comments kan oprettes på den
                        unitOfWork.SaveChanges();

                        // Tilføj tags
                        if (vm.SelectedTags != null)
                        {
                            var tagsString = string.Join(",", vm.SelectedTags);
                            _articleTagService.Add(tagsString, newArticle);
                        }
                        // Commit
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return RedirectToAction("Articles"); // TODO: Gå til artikel "newArticle"
        }


        [HttpGet] // Er du sikker på det skal bruges?
        public ActionResult EditArticle(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Articles");
            var article = _articleService.Get(id.Value);
            if (article == null)
                return HttpNotFound();

            var editArticleViewModel = new EditArticleViewModel
            {
                Id = article.Id,
                Header = article.Header,
                Description = article.Description,
                Body = article.Body,
                Image = article.Image,
                IsPublished = article.IsPublished,
                AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name)),
                SelectedTags = article.Tags.Select(x => x.Name)
            };
            return View(editArticleViewModel);
        }

        [HttpPost]
        public ActionResult EditArticle(EditArticleViewModel vm)
            //[Bind(Include = "Id, Header, Description, Body, Tags, Image, IsPublished, CreateDate, User")]
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    //try
                    //{
                    // Henter article fra Id fra viewmodel
                    var article = _articleService.Get(vm.Id);

                    // Overfører data
                    article.Header = vm.Header;
                    article.Description = vm.Description;
                    article.Body = vm.Body;
                    article.DateModified = DateTime.Now;
                    article.Image = vm.Image;
                    article.IsPublished = vm.IsPublished;
                    _articleService.Edit(article);

                    // Tilføj tags
                    if (vm.SelectedTags != null)
                    {
                        var tagsString = string.Join(",", vm.SelectedTags);
                        _articleTagService.Add(tagsString, article);
                    }
                    // Commit
                    unitOfWork.Commit();
                    //}
                    //catch (Exception ex)
                    //{
                    //    unitOfWork.Rollback();
                    //    LoggingService.Error(ex);
                    //    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    //}
                }
            }
            return RedirectToAction("Articles");
        }

        public ActionResult DeleteArticle(Guid? id)
        {
            // TODO: Vi skal have noget logging over hvem der har slettet artikler
            if (id == null)
                return RedirectToAction("Articles");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var article = _articleService.Get(id.Value);
                    if (article == null)
                        return HttpNotFound();

                    _articleService.Delete(article);

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

        public ActionResult Articles()
        {
            var viewmodel = new ArticlesViewModel {Articles = _articleService.GetAll()};
            return View(viewmodel);
        }

        public ActionResult Comments()
        {
            var vm = new CommentsViewModel
            {
                ArticleComments = _articleCommentService.GetAll().ToList()
            };
            return View(vm);
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
            var viewmodel = new ArticlesViewModel {Articles = _articleService.GetAll()};
            return View(viewmodel);
        }

        public ActionResult SetSticky(Guid id, int choice)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var settings = _CMSSettingsService.GetOrCreate();
                        if (choice == 1)
                        {
                            settings.ArticleSticky1 = id;
                            _CMSSettingsService.Edit(settings);
                        }
                        else if (choice == 2)
                        {
                            settings.ArticleSticky2 = id;
                            _CMSSettingsService.Edit(settings);
                        }
                        else if (choice == 3)
                        {
                            settings.ArticleSticky3 = id;
                            _CMSSettingsService.Edit(settings);
                        }
                        else if (choice == 4)
                        {
                            settings.ArticleSticky4 = id;
                            _CMSSettingsService.Edit(settings);
                        }
                        else
                        {
                            settings.ArticleSticky4 = settings.ArticleSticky3;
                            settings.ArticleSticky3 = settings.ArticleSticky2;
                            settings.ArticleSticky2 = settings.ArticleSticky1;
                            settings.ArticleSticky1 = id;
                            _CMSSettingsService.Edit(settings);
                        }
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
            return View();
        }

        public ActionResult GeneralSettings()
        {
            return View();
        }

        public ActionResult Tags()
        {
            var vm = new TagsViewModel {ArticleTags = _articleTagService.GetAll().ToList()};
            return View(vm);
        }

        public ActionResult UnDeleteComment(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Comments");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var articleComment = _articleCommentService.GetComment(id.Value);
                    if (articleComment == null)
                        return HttpNotFound();

                    articleComment.IsDeleted = false;

                    unitOfWork.Commit();
                    return RedirectToAction("Comments");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
        }

        public ActionResult DeleteComment(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Comments");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var articleComment = _articleCommentService.GetComment(id.Value);
                    if (articleComment == null)
                        return HttpNotFound();

                    articleComment.IsDeleted = true;

                    unitOfWork.Commit();
                    return RedirectToAction("Comments");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
        }

        public ActionResult DeleteTag(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Tags");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var articleTag = _articleTagService.Get(id.Value);
                    if (articleTag == null)
                        return HttpNotFound();

                    _articleTagService.Delete(articleTag);

                    unitOfWork.Commit();
                    return RedirectToAction("Tags");
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
        }

        //[HttpPost]
        //public ActionResult Tags(TestViewModel vm)
        //{
        //    //throw new NotImplementedException();

        //    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
        //    {
        //        _articleService.WipeDatabase();
        //        unitOfWork.Commit();
        //    }
        //    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
        //    {
        //        _articleTagService.CreateTestTags();
        //        unitOfWork.Commit();
        //    }
        //    using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
        //    {
        //        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
        //        _articleService.CreateTestData(loggedOnUser);
        //        unitOfWork.Commit();
        //    }
        //    return View();
        //}


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