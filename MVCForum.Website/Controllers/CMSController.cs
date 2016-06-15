using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Constants;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Services;
using MVCForum.Services.Data.Context;
using MVCForum.Services.Data.UnitOfWork;

namespace MVCForum.Website.Controllers
{
    public class CMSController : BaseController
    {
        private readonly IArticleService _articleService;
        public CMSController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager,
            IMembershipService membershipService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, IArticleService articleService)
            : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
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

            return View(@article);
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

        // GET: CMS/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CMS/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CMS/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CMS/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CMS/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CMS/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CMS/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }



    }
}
