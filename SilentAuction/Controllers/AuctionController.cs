using Microsoft.AspNet.Identity;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class AuctionController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Auction
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            Auction auction = new Auction();
            return View(auction);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name,Description,StartTime,EndTime,Donors,AuctionId,ManagerId,Day")] Auction auction)
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUserId);
            auction.ManagerId = manager.ManagerId;
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Auctions.Add(auction);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = auction.AuctionId });
            }

            return View(auction);
        }
    }
}