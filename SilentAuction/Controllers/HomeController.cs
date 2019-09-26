using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        public ActionResult Index(string email)
        {
            if (User.IsInRole("Manager"))
            {
                Manager manager = context.Managers.FirstOrDefault(m => m.EmailAddress == email);
                return RedirectToAction("Index", "Manager", new { id = manager.ManagerId });
            }
            if (User.IsInRole("Participant"))
            {
                Participant participant = context.Participants.FirstOrDefault(p => p.EmailAddress == email);
                return RedirectToAction("Index", "Participant", new { id = participant.ParticipantId });
            }
            else
            {
                return View();
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}