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
    public class ParticipantController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Participants
        public ActionResult Index(int id)
        {
            var participant = context.Participants.FirstOrDefault(m => m.ParticipantId == id);
            return View(participant);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant participant = context.Participants.Find(id);
            if (participant == null)
            {
                return HttpNotFound();
            }
            return View(participant);
        }
        public ActionResult Create()
        {
            Participant participant = new Participant();
            return View(participant);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId")] Participant participant)
        {
            var currentUserId = User.Identity.GetUserId();
            participant.ApplicationUserId = currentUserId;
            participant.EmailAddress = User.Identity.GetUserName();
            if (participant.ApplicationUserId == currentUserId)
            {
                context.Participants.Add(participant);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = participant.ParticipantId });
            }

            return View(participant);
        }
    }
}