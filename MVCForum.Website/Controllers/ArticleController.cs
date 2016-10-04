﻿using System;
using System.Linq;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Utilities;
using MVCForum.Website.Application;
using MVCForum.Website.ViewModels;
using RssItem = MVCForum.Domain.DomainModel.RssItem;
using MVCForum.Domain.DomainModel.Enums;
//using static MVCForum.Website.ViewModels.ArticleViewModels; wtf nicklas :D 

namespace MVCForum.Website.Controllers
{
    public class ArticleController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICMSSettingsService _CMSSettingsService;
        private readonly IArticleCategoryService _articleCategoryService;

        public ArticleController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IArticleService articleService,
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

        public ActionResult _LatestArticles()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {


                    // Får artikler
                    var articles = _articleService.GetNewestPublished(4);
                    // Laver dem om til viewmodels
                    var articleSectionViewModel = new ArticleSectionViewModel
                    {
                        Header = "Seneste artikler",
                        ShowHeader = true,
                        ArticleFrontpageViewModels = articles.Select(x => new ArticleFrontpageViewModel
                        {
                            Header = x.Header,
                            Slug = x.Slug,
                            PublishDate = x.PublishDate,
                            Image = x.Image,
                            UserName = x.User.UserName
                        })
                    };

                    return PartialView("_ArticleSection", articleSectionViewModel);
                
            }
        }

        public ActionResult _ArticleMain()
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var settings = _CMSSettingsService.GetOrCreate();
                if (settings.StickyArticle1 == null || settings.StickyArticle2 == null ||
                    settings.StickyArticle3 == null || settings.StickyArticle4 == null)
                    return new EmptyResult();
                var articleMainViewModel = new ArticleMainViewModel
                {
                    Article1 = new ArticleFrontpageViewModel
                    {
                        Header = settings.StickyArticle1.Header,
                        Image = settings.StickyArticle1.Image,
                        PublishDate = settings.StickyArticle1.PublishDate,
                        Slug = settings.StickyArticle1.Slug,
                        UserName = settings.StickyArticle1.User.UserName
                    },
                    Article2 = new ArticleFrontpageViewModel
                    {
                        Header = settings.StickyArticle2.Header,
                        Image = settings.StickyArticle2.Image,
                        PublishDate = settings.StickyArticle2.PublishDate,
                        Slug = settings.StickyArticle2.Slug,
                        UserName = settings.StickyArticle2.User.UserName
                    },
                    Article3 = new ArticleFrontpageViewModel
                    {
                        Header = settings.StickyArticle3.Header,
                        Image = settings.StickyArticle3.Image,
                        PublishDate = settings.StickyArticle3.PublishDate,
                        Slug = settings.StickyArticle3.Slug,
                        UserName = settings.StickyArticle3.User.UserName
                    },
                    Article4 = new ArticleFrontpageViewModel
                    {
                        Header = settings.StickyArticle4.Header,
                        Image = settings.StickyArticle4.Image,
                        PublishDate = settings.StickyArticle4.PublishDate,
                        Slug = settings.StickyArticle4.Slug,
                        UserName = settings.StickyArticle4.User.UserName
                    }
                };
                return PartialView("_ArticleMain", articleMainViewModel);
            }
        }

        public ActionResult Show(string slug)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (slug == null)
                    return ErrorToHomePage("Artiklen blev ikke fundet.");
                try
                {
                    var article = _articleService.Get(slug);
                    if (article == null)
                        return HttpNotFound();

                    // Tjekker permissions LoggedOnReadOnlyUser.Id != article.User.Id 
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    var userOwns = false;
                    if (LoggedOnReadOnlyUser != null)
                        userOwns = LoggedOnReadOnlyUser.Id == article.User.Id;

                    if (permissions["Edit All Articles"].IsTicked || article.IsPublished || userOwns)
                    {
                        // Do all logic here
                        if (!BotUtils.UserIsBot())
                            article.Views++;
                        // Commit the transaction
                        unitOfWork.Commit();

                        return View(article);
                    }
                }
                catch (Exception ex)
                {
                    // Roll back database changes 
                    unitOfWork.Rollback();
                    // Log the error
                    LoggingService.Error(ex);

                    // Do what you want
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        public ActionResult Category(string slug)
        {

            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
         
                var articleSectionViewModel = new ArticleSectionViewModel();
                articleSectionViewModel = new ArticleSectionViewModel
                {
                    Header = slug,
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleCategoryService.Get(slug).Articles
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                };
                return View("SubCategory", articleSectionViewModel);
            }
        }


        public ActionResult Nyheder()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var articleSectionViewModel = new ArticleSectionViewModel();
                articleSectionViewModel = new ArticleSectionViewModel
                {
                    Header = "Seneste Nyheder",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Nyhed)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                };
                return View(articleSectionViewModel);
            }
        }
        public ActionResult Video()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var articleSectionViewModel = new ArticleSectionViewModel();
                articleSectionViewModel = new ArticleSectionViewModel
                {
                    Header = "Seneste Videoer",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Video)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                };
                return View(articleSectionViewModel);
            }
        }

        public ActionResult Test()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var articleSectionViewModel = new ArticleSectionViewModel();
                articleSectionViewModel = new ArticleSectionViewModel
                {
                    Header = "Seneste Test",
                    ShowHeader = true,
                    ArticleFrontpageViewModels =
                        _articleService.GetNewestPublished(4, ArticleSection.Test)
                            .Select(a => new ArticleFrontpageViewModel
                            {
                                Header = a.Header,
                                Image = a.Image,
                                Slug = a.Slug,
                                PublishDate = a.PublishDate,
                                UserName = a.User.UserName
                            })
                };
                return View(articleSectionViewModel);
            }
        }

        public ActionResult _SubCategories(ArticleSection section)
        {
            var vms = _articleCategoryService.GetAllBySection(section).Select(x => new ArticleSubCategoryViewModel
            {
                Name = x.Name, Slug = x.Slug, SortOrder = x.SortOrder
            });
            return PartialView(vms);
        }


        public ActionResult _Comments(Guid article)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var comments = _articleCommentService.GetByArticle(article);
                    return PartialView(comments);
                }
                catch (Exception ex)
                {
                    // Roll back database changes 
                    unitOfWork.Rollback();
                    // Log the error
                    LoggingService.Error(ex);

                    // Do what you want
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }

        [HttpGet]
        public ActionResult _Comment(Article model)
        {
            var viewmodel = new CommentViewModel();
            viewmodel.ArticleSlug = model.Slug;
            viewmodel.ArticleId = model.Id;
            return PartialView(viewmodel);
        }

        [HttpPost]
        public ActionResult _Comment(CommentViewModel vm)
        {
            ArticleComment newComment;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        newComment = _articleCommentService.Add(vm.CommentBody, vm.InReplyTo, vm.ArticleId, loggedOnUser);
                        unitOfWork.Commit();
                        return RedirectToAction("Show", new {slug = newComment.Article.Slug});
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

        // Mangler ikke permission da den tjekker logged on.
        public ActionResult _CommentsDelete(Guid commentId, Guid ArticleId)
        {
            Article article;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        if (permissions["Comment Moderation"].IsTicked || loggedOnUser == _articleCommentService.GetComment(commentId).User)
                        {
                            _articleCommentService.Delete(commentId);
                            article = _articleService.Get(ArticleId);
                            unitOfWork.Commit();
                            return RedirectToAction("Show", new { slug = article.Slug });
                        }
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

        public ActionResult CommentEdit(Guid commentId)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    if (commentId == null)
                        return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    var comment = _articleCommentService.GetComment(commentId);
                    if (comment == null)
                        return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    var permissions = RoleService.GetPermissions(null, UsersRole);
                    var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                    if (permissions["Comment Moderation"].IsTicked || loggedOnUser == _articleCommentService.GetComment(commentId).User)
                    {
                        var vm = new CommentViewModel
                        {
                            CommentBody = comment.CommentBody,
                            CommentId = comment.Id,
                            ArticleSlug = comment.Article.Slug
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

        [HttpPost]
        public ActionResult CommentEdit(CommentViewModel comment)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var permissions = RoleService.GetPermissions(null, UsersRole);
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        if (permissions["Comment Moderation"].IsTicked || loggedOnUser == _articleCommentService.GetComment(comment.CommentId).User)
                        {
                            // Henter comment fra Id fra viewmodel
                            var Comment = _articleCommentService.GetComment(comment.CommentId);

                            // Overfører data
                            Comment.CommentBody = comment.CommentBody;

                            _articleCommentService.Update(Comment);

                            // Commit
                            unitOfWork.Commit();
                            return RedirectToAction("Show", new { slug = comment.ArticleSlug });
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

        public ActionResult Search()
        {
            return View();
        }

        public PartialViewResult _SearchArticles(string keyword)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var articles = _articleService.Search(50, keyword);
                //var articleSearchVms = articles.Select(article => new ArticleSearchViewModel
                //{
                //    Slug = article.Slug,
                //    Header = article.Header,
                //    PublishDate = article.PublishDate,
                //    UserName = article.User.UserName
                //});
                var articleSectionViewModel = new ArticleSectionViewModel
                {
                    ShowHeader = false,
                    ArticleFrontpageViewModels = articles.Select(x => new ArticleFrontpageViewModel
                    {
                        Header = x.Header, Image = x.Image, PublishDate = x.PublishDate,
                        Slug = x.Slug, UserName = x.User.UserName
                    })
                };
                return PartialView("_ArticleSection", articleSectionViewModel);
            }
        }

        public ActionResult LatestRss()
        {
            var articles = _articleService.GetNewestPublished(50);
            var rssArticles = articles.Select(article => new RssItem
            {
                Description = article.Description,
                Link = "/nyhed/" + article.Slug,
                Title = article.Header,
                PublishedDate = article.PublishDate
            }).ToList();
            return new RssResult(rssArticles, "Overskrift", "Beskrivelse");
        }
    }
}