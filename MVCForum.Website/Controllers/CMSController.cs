using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Areas.Admin.ViewModels;
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

        // GET: /cms/
        [Authorize]
        public ActionResult Index()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked)
                {
                    return View();
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/newarticle/
        [Authorize]
        public ActionResult NewArticle()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Access CMS"].IsTicked)
                    {
                        var vm = new AddArticleViewModel
                        {
                            AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name))
                        };
                        return View(vm);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/newarticle/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult NewArticle(AddArticleViewModel vm)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Access CMS"].IsTicked)
                        {
                            // Tjekker om der er permissions til at publish
                            if (!permissions["Publish Articles"].IsTicked && vm.IsPublished)
                                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

                            var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                            var newArticle = _articleService.AddNewArticle(vm.Header,
                                vm.Description, vm.Body, vm.Image, vm.IsPublished, DateTime.Now, loggedOnUser);
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

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Artiklen blev oprettet.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("Show", "Article", new { slug = newArticle.Slug });
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/editarticle/
        [Authorize]
        public ActionResult EditArticle(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Articles");
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var article = _articleService.Get(id.Value);
                    if (article == null)
                        return HttpNotFound();

                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (LoggedOnReadOnlyUser.Id == article.User.Id || permissions["Edit All Articles"].IsTicked)
                    {
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
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/editarticle/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditArticle(EditArticleViewModel vm)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Henter article fra Id fra viewmodel
                        var article = _articleService.Get(vm.Id);

                        // Tjekker om det er brugerens egen artikel eller man har rettigheder
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (LoggedOnReadOnlyUser.Id == article.User.Id || permissions["Edit All Articles"].IsTicked)
                        {
                            if (article.IsPublished != vm.IsPublished && !permissions["Publish Articles"].IsTicked)
                            {
                                return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                            }
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

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Artiklen blev redigeret.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("Articles");
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/deletearticle/
        [Authorize]
        public ActionResult DeleteArticle(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Articles");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // Get
                    var article = _articleService.Get(id.Value);
                    if (article == null)
                        return HttpNotFound();

                    // Tjekker om det er brugerens egen artikel eller man har rettigheder
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (article.User.Id == LoggedOnReadOnlyUser.Id || permissions["Delete All Articles"].IsTicked)
                    {
                        // Slet
                        _articleService.Delete(article);

                        // TODO: Vi skal have noget logging over hvem der har slettet artikler
                        LoggingService.Error("Artikel slettet: " + article.Slug);

                        // Commit
                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Artiklen blev slettet.",
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("Articles");
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/articles/
        [Authorize]
        public ActionResult Articles()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Access CMS"].IsTicked)
                    {
                        var viewmodel = new ArticlesViewModel();

                        viewmodel.Articles = permissions["Edit All Articles"].IsTicked
                            ? _articleService.GetAll()
                            : _articleService.GetAllAllowed(LoggedOnReadOnlyUser.Id);
                        return View(viewmodel);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/comments/
        [Authorize]
        public ActionResult Comments()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked && permissions["Comment moderation"].IsTicked)
                {
                    var vm = new CommentsViewModel
                    {
                        ArticleComments = _articleCommentService.GetAll().ToList()
                    };
                    return View(vm);
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/statistics/
        [Authorize]
        public ActionResult Statistics()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked && permissions["View Statistics"].IsTicked)
                {
                    return View();
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        public ActionResult FrontpageSettings()
        {
            var viewmodel = new ArticlesViewModel { Articles = _articleService.GetAll() };
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

        // GET: /cms/tags/
        [Authorize]
        public ActionResult Tags()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked)
                {
                    var vm = new TagsViewModel { ArticleTags = _articleTagService.GetAll().ToList() };
                    return View(vm);
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/deletetag/
        [Authorize]
        public ActionResult DeleteTag(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Tags");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Edit tags"].IsTicked)
                    {
                        var articleTag = _articleTagService.Get(id.Value);
                        if (articleTag == null)
                            return HttpNotFound();

                        _articleTagService.Delete(articleTag);

                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Tagget blev slettet.",
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("Tags");
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/undeletecomment/
        [Authorize]
        public ActionResult UnDeleteComment(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Comments");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Comment Moderation"].IsTicked)
                    {
                        var articleComment = _articleCommentService.GetComment(id.Value);
                        if (articleComment == null)
                            return HttpNotFound();

                        articleComment.IsDeleted = false;

                        // Commit
                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Kommentaren blev u-slettet.",
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("Comments");
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/deletecomment/
        [Authorize]
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

                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (LoggedOnReadOnlyUser.Id == articleComment.User.Id || permissions["Comment Moderation"].IsTicked)
                    {
                        articleComment.IsDeleted = true;

                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Kommentaren blev slettet.",
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("Comments");
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
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