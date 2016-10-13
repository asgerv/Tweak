using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Areas.Admin.ViewModels;
using MVCForum.Website.ViewModels;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.DomainModel.Enums;
using DoksoftUploaderLibrary;
using System.Web.Helpers;

namespace MVCForum.Website.Controllers
{
    public class CMSController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICMSSettingsService _CMSSettingsService;
        private readonly IArticleCategoryService _articleCategoryService;

        public CMSController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService,
            ICMSSettingsService cmsSettingsService, IArticleCategoryService articleCategoryService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
            _CMSSettingsService = cmsSettingsService;
            _articleCategoryService = articleCategoryService;
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
                    var vm = new DashboardViewModel();
                    var articles = _articleService.GetNewestPublished(200000).Where(x => x.PublishDate > DateTime.Now.AddDays(-30)).ToList();
                    vm.Comments = _articleCommentService.GetAll().Count(x => x.DateCreated > DateTime.Now.AddDays(-30));
                    vm.ArticlesPublished = articles.Count;
                    vm.PageViews = articles.Sum(x => x.Views);
                    return View(vm);
                }
            }
            // Ingen permission til CMS
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
                            AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name)),
                            AvailableCategories = CategoriesToSelectListItems(_articleCategoryService.GetAll())
                        };
                        return View(vm);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til CMS
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
                            var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                            var category = _articleCategoryService.Get(vm.Category);
                            var newArticle = _articleService.AddNewArticle(vm.Header,
                                vm.Description, vm.Body, vm.Image, category, loggedOnUser);

                            // Gemmer article i database, så tags kan oprettes på den
                            unitOfWork.SaveChanges();

                            // Publish
                            if (vm.IsPublished)
                            {
                                if (!permissions["Publish Articles"].IsTicked)
                                {
                                    ShowMessage(new GenericMessageViewModel
                                    {
                                        Message = "Ingen rettigheder til at publicere artikler.",
                                        MessageType = GenericMessages.danger
                                    });
                                    vm.AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name));
                                    vm.AvailableCategories =
                                        CategoriesToSelectListItems(_articleCategoryService.GetAll());
                                    vm.SelectedTags = null;
                                    return View(vm);
                                }
                                _articleService.PublishArticle(newArticle);
                            }

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
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });
                        vm.AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name));
                        vm.AvailableCategories = CategoriesToSelectListItems(_articleCategoryService.GetAll());
                        vm.SelectedTags = null;
                        return View(vm);
                    }
                }
            }
            // Ingen permission til CMS
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
                            AvailableCategories = CategoriesToSelectListItems(_articleCategoryService.GetAll()),
                            AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name)),
                            SelectedTags = article.Tags.Select(x => x.Name)
                        };
                        var listItem = editArticleViewModel.AvailableCategories.FirstOrDefault(x => x.Value == article.ArticleCategory.Slug);
                        if (listItem != null)
                            listItem.Selected = true;
                        return View(editArticleViewModel);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til Edit
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                            // Publish
                            if (article.IsPublished != vm.IsPublished)
                            {
                                if (!permissions["Publish Articles"].IsTicked)
                                {
                                    ShowMessage(new GenericMessageViewModel
                                    {
                                        Message = "Ingen rettigheder til at publicere artikler.",
                                        MessageType = GenericMessages.danger
                                    });
                                    vm.AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name));
                                    vm.AvailableCategories = CategoriesToSelectListItems(_articleCategoryService.GetAll());
                                    return View(vm);
                                }
                                if (vm.IsPublished)
                                    _articleService.PublishArticle(article);
                                else
                                    article.IsPublished = false;
                            }

                            // Overfører data
                            var category = _articleCategoryService.Get(vm.Category);
                            article.ArticleCategory = category;
                            article.Header = vm.Header;
                            article.Description = vm.Description;
                            article.Body = vm.Body;
                            article.DateModified = DateTime.Now;
                            article.Image = vm.Image;
                            
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
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });
                        vm.AvailableTags = new SelectList(_articleTagService.GetAll().Select(x => x.Name));
                        vm.AvailableCategories = CategoriesToSelectListItems(_articleCategoryService.GetAll());
                        return View(vm);
                    }
                }
            }
            // Ingen permission til Edit
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                        // Slet og Log
                        _articleService.Delete(article);
                        LoggingService.Error(LoggedOnReadOnlyUser.UserName + "slettede artikel " + article.Slug);

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
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til Delete
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                            : _articleService.GetByUserAndPublished(LoggedOnReadOnlyUser.Id);
                        return View(viewmodel);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til CMS
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/comments/
        [Authorize]
        public ActionResult Comments()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked && permissions["Comment Moderation"].IsTicked)
                {
                    var vm = new CommentsViewModel
                    {
                        ArticleComments = _articleCommentService.GetAll().ToList()
                    };
                    return View(vm);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/section/
        [Authorize]
        public ActionResult Section(ArticleSection section)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Edit Article Categories"].IsTicked) // add ny permission
                {
                    var articles = _articleCategoryService.GetAllBySection(section);
                    ViewBag.SectionName = section;
                    return View(articles);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/newcategory/
        [Authorize]
        public ActionResult NewCategory()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Edit Article Categories"].IsTicked) // add ny permission
                {
                    var vm = new AddCategoryViewModel
                    {
                        SortOrder = 0,
                        AvailableSections = new SelectList(new List<ArticleSection> { ArticleSection.Nyhed, ArticleSection.Video, ArticleSection.Test })
                    };
                    return View(vm);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/newcategory/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult NewCategory(AddCategoryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Edit Article Categories"].IsTicked) // add ny permission
                        {
                            var category = _articleCategoryService.Add(vm.Name, vm.Description, vm.SortOrder, vm.Section);
                            unitOfWork.Commit();

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Kategorien blev oprettet.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("Section", "CMS", new { section = category.ArticleSection });
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });
                        return View(vm);
                    }
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/editcategory/
        [Authorize]
        public ActionResult EditCategory(string slug)
        {
            if (slug == null)
                return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var category = _articleCategoryService.Get(slug);
                    if (category == null)
                        return HttpNotFound();

                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Edit Article Categories"].IsTicked)
                    {
                        var vm = new EditCategoryViewModel
                        {
                            Slug = category.Slug,
                            Name = category.Name,
                            Description = category.Description,
                            SortOrder = category.SortOrder,
                            Section = category.ArticleSection,
                            AvailableSections = new SelectList(new List<ArticleSection> { ArticleSection.Nyhed, ArticleSection.Video, ArticleSection.Test })
                        };
                        return View(vm);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til Edit
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/editcategory/
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult EditCategory(EditCategoryViewModel vm)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var category = _articleCategoryService.Get(vm.Slug);

                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Edit Article Categories"].IsTicked)
                        {
                            // Overfører data
                            category.Name = vm.Name;
                            category.Description = vm.Description;
                            category.SortOrder = vm.SortOrder;
                            category.ArticleSection = vm.Section;

                            _articleCategoryService.Edit(category);

                            // Commit
                            unitOfWork.Commit();

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Kategorien blev redigeret.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("Section", "CMS", new { section = category.ArticleSection });
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);

                        // Fejl besked
                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Errors.GenericMessage"),
                            MessageType = GenericMessages.danger
                        });

                        // Sætter AvailableSections ind igen
                        vm.AvailableSections =
                            new SelectList(new List<ArticleSection>
                            {
                                ArticleSection.Nyhed,
                                ArticleSection.Video,
                                ArticleSection.Test
                            });

                        return View(vm);
                    }
                }
            }
            // Ingen permission til Edit
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/deletecategory/
        [Authorize]
        public ActionResult DeleteCategory(string slug)
        {
            if (slug == null)
                return RedirectToAction("Index");
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // Get
                    var category = _articleCategoryService.Get(slug);
                    if (category == null)
                        return HttpNotFound();

                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Edit Article Categories"].IsTicked)
                    {
                        // Slet 
                        _articleCategoryService.Delete(category);

                        // Commit
                        unitOfWork.Commit();

                        ShowMessage(new GenericMessageViewModel
                        {
                            Message = "Kategorien blev slettet.",
                            MessageType = GenericMessages.success
                        });
                        return RedirectToAction("Section", new { section = category.ArticleSection });
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            // Ingen permission til Delete
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/frontpagesettings/
        [Authorize]
        public ActionResult FrontpageSettings()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked && permissions["Edit Frontpage"].IsTicked)
                {
                    var viewmodel = new ArticlesViewModel { Articles = _articleService.GetNewestPublished(500) };
                    return View(viewmodel);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/setsticky/
        [Authorize]
        public ActionResult SetSticky(Guid? id, int choice)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    if (permissions["Access CMS"].IsTicked && permissions["Edit Frontpage"].IsTicked)
                    {
                        if (ModelState.IsValid)
                        {
                            var settings = _CMSSettingsService.GetOrCreate();
                            if (choice == 1)
                            {
                                settings.StickyArticle1 = _articleService.Get(id.Value);

                            }
                            else if (choice == 2)
                            {
                                settings.StickyArticle2 = _articleService.Get(id.Value);

                            }
                            else if (choice == 3)
                            {
                                settings.StickyArticle3 = _articleService.Get(id.Value);

                            }
                            else if (choice == 4)
                            {
                                settings.StickyArticle4 = _articleService.Get(id.Value);

                            }
                            else
                            {
                                settings.StickyArticle4 = settings.StickyArticle3;
                                settings.StickyArticle3 = settings.StickyArticle2;
                                settings.StickyArticle2 = settings.StickyArticle1;
                                settings.StickyArticle1 = _articleService.Get(id.Value);
                                //_CMSSettingsService.Edit(settings);
                            }
                            unitOfWork.Commit();

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Artikel blev sat som sticky.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("FrontpageSettings");
                        }
                    }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/frontpagetags/
        [Authorize]
        public ActionResult FrontPageTags()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                CMSSettings settings = _CMSSettingsService.GetOrCreate();
                ArticleTagViewModel model = new ArticleTagViewModel();
                IList<ArticleTagViewModel> vm = new List<ArticleTagViewModel>();
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked && permissions["Edit Frontpage"].IsTicked)
                {
                    var tags = new TagsViewModel { ArticleTags = _articleTagService.GetAll().ToList() };
                    foreach (var item in tags.ArticleTags)
                    {
                        model.ArticleCount = item.Articles.Count();
                        model.Id = item.Id;
                        model.Name = item.Name;
                        if (settings.StickyTags.Contains(item))
                            model.IsFrontpage = true;
                        vm.Add(model);
                        model = new ArticleTagViewModel();
                    }
                    return View(vm);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/setstickytag/
        [Authorize]
        public ActionResult SetStickyTag(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Access CMS"].IsTicked && permissions["Edit Frontpage"].IsTicked)
                        {
                            var settings = _CMSSettingsService.GetOrCreate();
                            settings.StickyTags.Add(_articleTagService.Get(id));

                            unitOfWork.Commit();

                            ShowMessage(new GenericMessageViewModel
                            {
                                Message = "Tagget blev sat som sticky.",
                                MessageType = GenericMessages.success
                            });
                            return RedirectToAction("FrontPageTags");
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // POST: /cms/removestickytag/
        [Authorize]
        public ActionResult RemoveStickyTag(Guid id)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Access CMS"].IsTicked && permissions["Edit Frontpage"].IsTicked)
                        {
                            var settings = _CMSSettingsService.GetOrCreate();
                            settings.StickyTags.Remove(_articleTagService.Get(id));
                            unitOfWork.Commit();


                            return RedirectToAction("FrontPageTags");
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        // GET: /cms/generalsettings
        [Authorize]
        public ActionResult GeneralSettings()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var permissions = RoleService.GetPermissions(null, UsersRole);
                if (permissions["Access CMS"].IsTicked)
                {
                    var thisArticle = _articleService.Get(new Guid("d2162bfd-fc5b-43c2-b150-a68001276e8c"));
                    var related = _articleService.GetRelated(thisArticle, 50);
                    var mostpopular = _articleService.GetMostPopular(thisArticle.Id, 50, 100);
                    return View(related);
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                    if (permissions["Edit Tags"].IsTicked)
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
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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
                    return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToCMSDashboard(LocalizationService.GetResourceString("Errors.NoPermission"));
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


        /// <summary>
        ///     Upload metode til NewArticle og EditArticle (Article.Image)
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile()
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);
                    _imgname = fileName;

                    //_imgname = Guid.NewGuid().ToString();
                    var _comPath = Server.MapPath("~/Images/ArticleImages/") + _imgname;
             
                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    //if (img.Width > 200)
                    //    img.Resize(200, 200);
                    img.Save(_comPath);
                    // end resize
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }



        #region Helpers

        public List<SelectListItem> CategoriesToSelectListItems(IEnumerable<ArticleCategory> categories)
        {
            var items = new List<SelectListItem>();
            var nyhedGroup = new SelectListGroup { Name = "Nyhed" };
            var videoGroup = new SelectListGroup { Name = "Video" };
            var testGroup = new SelectListGroup { Name = "Test" };

            foreach (var category in categories)
            {
                var item = new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Slug
                };
                if (category.ArticleSection == ArticleSection.Nyhed)
                    item.Group = nyhedGroup;
                if (category.ArticleSection == ArticleSection.Video)
                    item.Group = videoGroup;
                if (category.ArticleSection == ArticleSection.Test)
                    item.Group = testGroup;
                items.Add(item);
            }
            return items;
        }

        #endregion


        #region Uploader
        public void DoksoftUploader()
        {
            UploaderHandler uploadHandler = new UploaderHandler();
            uploadHandler.ProcessRequest(System.Web.HttpContext.Current);
        }
        #endregion
    }
}