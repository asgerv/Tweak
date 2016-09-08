using System;
using System.Linq;
using System.Web.Mvc;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Utilities;
using MVCForum.Website.Application;
using MVCForum.Website.ViewModels;
//using static MVCForum.Website.ViewModels.ArticleViewModels; wtf nicklas :D 
using RssItem = MVCForum.Domain.DomainModel.RssItem;
using System.Collections.Generic;

namespace MVCForum.Website.Controllers
{
    public class ArticleController : BaseController
    {
        private readonly IArticleCommentService _articleCommentService;
        private readonly IArticleService _articleService;
        private readonly IArticleTagService _articleTagService;
        private readonly ICMSSettingsService _CMSSettingsService;

        public ArticleController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            IArticleService articleService,
            IArticleCommentService articleCommentService, IArticleTagService articleTagService, ICMSSettingsService cmsSettingsService)
            : base(
                loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
            _articleService = articleService;
            _articleCommentService = articleCommentService;
            _articleTagService = articleTagService;
            _CMSSettingsService = cmsSettingsService;
        }

        public ActionResult _ArticleMain()
        {
            CMSSettings settings = _CMSSettingsService.GetOrCreate();
            List<Article> articles = new List<Article>();
            IList<Article> newest = _articleService.GetNewestPublished(4);
            var viewmodel = new ArticlesPreviewViewModel();
            if (newest.Count == 4)
            {
                if (settings.ArticleSticky1 != null)
                {
                    articles.Add(_articleService.Get(settings.ArticleSticky1));
                }
                else
                {
                    articles.Add(newest.ElementAt(1));
                }
                if (settings.ArticleSticky2 != null)
                {
                    articles.Add(_articleService.Get(settings.ArticleSticky2));
                }
                else
                {
                    articles.Add(newest.ElementAt(2));
                }
                if (settings.ArticleSticky3 != null)
                {
                    articles.Add(_articleService.Get(settings.ArticleSticky3));
                }
                else
                {
                    articles.Add(newest.ElementAt(3));
                }
                if (settings.ArticleSticky4 != null)
                {
                    articles.Add(_articleService.Get(settings.ArticleSticky4));
                }
                else
                {
                    articles.Add(newest.ElementAt(4));
                }
            }

            viewmodel.Articles = articles;
            

            if (viewmodel.Articles.Count() < 4)
                return new EmptyResult();
                return PartialView(viewmodel);
        }

        public ActionResult _Article_Grid4x2(int? number)
        {
            // 1. Skal få et articletag
            // 2. List<Article> x = _articleService.GetByTag(...
            var viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Tag = "Chosen Tag";
            viewmodel.Articles = _articleService.GetNewest(13);
            if (viewmodel.Articles.Any())
                return PartialView(viewmodel);
            else
                return new EmptyResult();
        }


        public ActionResult _Sticky_Tag(int? number)
        {
            // 1. Skal få et articletag
            // 2. List<Article> x = _articleService.GetByTag(...
            var viewmodel = new ArticlesPreviewViewModel();
            CMSSettings settings = _CMSSettingsService.GetOrCreate();
         
          
                if(settings.FrontPageCategory1 != null && number == 1)
                {
                    ArticleTag Tag = _articleTagService.Get(settings.FrontPageCategory1);
                    viewmodel.Tag = Tag.Name;
                    viewmodel.Articles = Tag.Articles.Take(8);
                }
            else
            {
                viewmodel.Tag = "Seneste nyheder";
                viewmodel.Articles = _articleService.GetNewest(4);
            }
                if (settings.FrontPageCategory2 != null && number == 2)
                {
                    ArticleTag Tag = _articleTagService.Get(settings.FrontPageCategory2);
                    viewmodel.Tag = Tag.Name;
                    viewmodel.Articles = Tag.Articles.Take(8);
                }
            else
            {
                viewmodel.Tag = "Seneste nyheder";
                viewmodel.Articles = _articleService.GetNewest(4);
            }
            if (settings.FrontPageCategory3 != null && number == 3)
                {
                    ArticleTag Tag = _articleTagService.Get(settings.FrontPageCategory3);
                    viewmodel.Tag = Tag.Name;
                    viewmodel.Articles = Tag.Articles.Take(8);
                }
            else
            {
                viewmodel.Tag = "Seneste nyheder";
                viewmodel.Articles = _articleService.GetNewest(4);
            }
            if (settings.FrontPageCategory4 != null &&number == 4)
                {
                    ArticleTag Tag = _articleTagService.Get(settings.FrontPageCategory4);
                    viewmodel.Tag = Tag.Name;
                    viewmodel.Articles = Tag.Articles.Take(8);
                }
            else
            {
                viewmodel.Tag = "Seneste nyheder";
                viewmodel.Articles = _articleService.GetNewest(4);
            }

            if (viewmodel.Articles.Any())
                return PartialView("_Article_Grid4x2", viewmodel);
            else
                return new EmptyResult();
        }

        public ActionResult Show(string slug)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (slug == null)
                    return RedirectToAction("Index", "Home"); // TODO: Lav fejlside ("Artikel blev ikke fundet")
                var article = _articleService.Get(slug);
                if (article == null)
                    return HttpNotFound();
                try
                {
                    // Do all logic here
                    if (!BotUtils.UserIsBot())
                        article.Views++;
                    // Commit the transaction
                    unitOfWork.Commit();
                    return View(article);
                }
                catch (Exception ex)
                {
                    // Roll back database changes 
                    unitOfWork.Rollback();
                    // Log the error
                    LoggingService.Error(ex);

                    // Do what you want
                    return RedirectToAction("Index", "Home");
                }
            }
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
                        unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                        return RedirectToAction("Show", new {id = newComment.Article.Slug});
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }

            return View("home");
        }


        public ActionResult _CommentsDelete(Guid commentId, Guid ArticleId)
        {
            Article article;
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        _articleCommentService.Delete(commentId);
                        article = _articleService.Get(ArticleId);
                        unitOfWork.SaveChanges();
                        unitOfWork.Commit();
                        return RedirectToAction("Show", new {id = article.Slug});
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    }
                }
            }

            return View("home");
        }

        public ActionResult CommentEdit(Guid commentId)
        {
            if (commentId == null)
                return RedirectToAction("Articles");
            var comment = _articleCommentService.GetComment(commentId);
            if (comment == null)
                return HttpNotFound();

            var vm = new CommentViewModel
            {
                CommentBody = comment.CommentBody,
                CommentId = comment.Id,
                ArticleSlug = comment.Article.Slug
            };
            return View(vm);
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
                        // Henter comment fra Id fra viewmodel
                        var Comment = _articleCommentService.GetComment(comment.CommentId);

                        // Overfører data
                        Comment.CommentBody = comment.CommentBody;

                        _articleCommentService.Update(Comment);

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
            return RedirectToAction("nyhed", new {id = comment.ArticleSlug});
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