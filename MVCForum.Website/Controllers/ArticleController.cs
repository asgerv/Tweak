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
            var viewmodel = new ArticlesPreviewViewModel();
            viewmodel.Articles = _articleService.GetNewest(4);

            if (viewmodel.Articles.Count() < 4)
                return new EmptyResult();
            return PartialView(viewmodel);
        }
        public ActionResult _StickiesTag()
        {
            // 1. Skal få et articletag
            // 2. List<Article> x = _articleService.GetByTag(...
            CMSSettings settings = _CMSSettingsService.GetOrCreate();
            IList<ArticlesPreviewViewModel> viewmodel = new List<ArticlesPreviewViewModel>();
            ArticlesPreviewViewModel model = new ArticlesPreviewViewModel();
            IList<ArticleTag> Tags = settings.StickyTags.Take(8).ToList();
           
            if(Tags != null)
            foreach(var item in Tags)
            {

                model.Articles = item.Articles;
                model.Tag = item.Name;
                viewmodel.Add(model);
                model = new ArticlesPreviewViewModel();


            }

            //using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            //{
            //    try
            //    {
                 
                   
            //     }
                
            //    catch (Exception ex)
            //    {
            //        // Roll back database changes 
            //        unitOfWork.Rollback();
            //        // Log the error
            //        LoggingService.Error(ex);

            //        // Do what you want
            //        return RedirectToAction("Index", "CMS");
            //    }

            //}

            if(viewmodel.Any())
                return PartialView("_StickiesTag", viewmodel);
            else
                return new EmptyResult();
        }

        public ActionResult _LatestArticles()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    // 1. Skal få et articletag
                    // 2. List<Article> x = _articleService.GetByTag(...
                   
                    IList<ArticlesPreviewViewModel> viewmodel = new List<ArticlesPreviewViewModel>();
                    ArticlesPreviewViewModel model = new ArticlesPreviewViewModel();
                
                
                    

                            model.Articles = _articleService.GetNewest(8);
                            model.Tag = "Seneste Artikler";
                            viewmodel.Add(model);
                            model = new ArticlesPreviewViewModel();
                            return PartialView("_StickiesTag", viewmodel);

                        
                }

                catch (Exception ex)
                {
                    // Roll back database changes 
                    unitOfWork.Rollback();
                    // Log the error
                    LoggingService.Error(ex);

                    // Do what you want
                    return new EmptyResult();
                }
            }
            return new EmptyResult();
        }
        public ActionResult _Stickies()
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var viewmodel = new ArticlesPreviewViewModel();
                    CMSSettings settings = _CMSSettingsService.GetOrCreate();
                    IList<Article> articles = new List<Article>();
                    if(settings.StickyArticle1 != null && settings.StickyArticle2 != null && settings.StickyArticle3 != null && settings.StickyArticle4 != null)
                        articles.Add(settings.StickyArticle1);
                        articles.Add(settings.StickyArticle2);
                        articles.Add(settings.StickyArticle3);
                        articles.Add(settings.StickyArticle4);
                        viewmodel.Articles = articles;
                    if(viewmodel.Articles.Count() > 3)
                        return PartialView("_ArticleMain", viewmodel);
                    }
                
                catch (Exception ex)
                {
                    // Roll back database changes 
                    unitOfWork.Rollback();
                    // Log the error
                    LoggingService.Error(ex);

                    // Do what you want
                    return new EmptyResult();
                }
            }
            return new EmptyResult();
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

        public ActionResult _Comments(Guid article)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    IList<ArticleComment> comments = _articleCommentService.GetByArticle(article);
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

        public ActionResult Category(string tag)
        {

            return new EmptyResult();
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
                        return RedirectToAction("Show", new { slug = newComment.Article.Slug });
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
                try
                {
                    if (ModelState.IsValid)
                {
                    //try
                    //{
                        var loggedOnUser = MembershipService.GetUser(LoggedOnReadOnlyUser.Id);
                        _articleCommentService.Delete(commentId);
                        article = _articleService.Get(ArticleId);
                    unitOfWork.Commit();
                        
                        return RedirectToAction("Show", new { slug = article.Slug });
                    //}
                    //catch (Exception ex)
                    //{
                    //    unitOfWork.Rollback();
                    //    LoggingService.Error(ex);
                    //    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                    //}
                }
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
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
            return RedirectToAction("Show", new { id = comment.ArticleSlug });
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
                var articleSearchVms = articles.Select(article => new ArticleSearchViewModel
                {
                    Slug = article.Slug, Header = article.Header, PublishDate = article.PublishDate, UserName = article.User.UserName
                });
                return PartialView(articleSearchVms);
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