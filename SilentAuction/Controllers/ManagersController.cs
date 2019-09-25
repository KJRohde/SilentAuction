using Microsoft.AspNet.Identity;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class ManagersController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Managers
        public ActionResult Index(int id)
        {
            var manager = context.Managers.FirstOrDefault(m => m.Id == id);
            return View(manager);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manager manager = context.Managers.Find(id);
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }
        public ActionResult Create()
        {
            Manager manager = new Manager();
            return View(manager);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId")] Manager manager)
        {
            var currentUserId = User.Identity.GetUserId();
            manager.ApplicationUserId = currentUserId;
            manager.EmailAddress = User.Identity.GetUserName();
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Managers.Add(manager);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = manager.Id });
            }

            return View(manager);
        }
    }
}