using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Constants;

namespace MVCForum.Website.Controllers
{
    public class CMSController : Controller
    {
        // GET: CMS
        public ActionResult Index()
        {
            return View();
        }
        // GET: Nyheder
        public ActionResult News()
        {
            return View();
        }
        // GET: Nyheder
        public ActionResult NewArticle()
        {
            //ViewBag.ImageUploadType = "forumimageinsert";
            return View();
        }
        // GET: Nyheder
        public ActionResult Comments()
        {
            return View();
        }
        // GET: Nyheder
        public ActionResult Statistics()
        {
            return View();
        }
        // GET: Nyheder
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
