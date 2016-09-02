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
            // 1. Skal få et articletag
            // 2. List<Article> x = _articleService.GetByTag(...
            var viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Tag = "Chosen Tag";
            viewmodel.Articles = _articleService.GetNewest(13);
            return PartialView(viewmodel);
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
                        return RedirectToAction("nyhed", new {id = newComment.Article.Id});
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
                        return RedirectToAction("nyhed", new {id = article.Id});
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
                ArticleId = comment.Article.Id
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
            return RedirectToAction("nyhed", new {id = comment.ArticleId});
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