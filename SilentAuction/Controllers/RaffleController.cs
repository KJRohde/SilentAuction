using Microsoft.AspNet.Identity;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class RaffleController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Raffle
        public ActionResult Index(int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id);
            return View(raffle);
        }
        public ActionResult Create()
        {
            Raffle raffle = new Raffle();
            return View(raffle);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name,Description,StartTime,EndTime,Donors,RaffleId,ManagerId,Day,CostPerTicket,TotalRaised")] Raffle raffle)
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUserId);
            raffle.ManagerId = manager.ManagerId;
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Raffles.Add(raffle);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = raffle.RaffleId });
            }

            return View(raffle);
        }
        public ActionResult AddItem(int id)
        {
            RafflePrize rafflePrize = new RafflePrize();
            return View(rafflePrize);
        }

        [HttpPost]
        public ActionResult AddItem([Bind(Include = "Description,ActualValue,Picture,RaffleId,WinnerId")] RafflePrize rafflePrize, int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(a => a.RaffleId == id);
            rafflePrize.RaffleId = raffle.RaffleId;
            rafflePrize.WinnerId = null;


            context.RafflePrizes.Add(rafflePrize);
            context.SaveChanges();
            return RedirectToAction("Continue", new { id = rafflePrize.RaffleId });
        }
        public ActionResult GetRafflePrizes(int id)
        {
            var prizes = context.RafflePrizes.Where(p => p.RaffleId == id);
            return View(prizes);
        }
        public ActionResult Participate(int id)
        {
            var raffleItems = context.RafflePrizes.Where(a => a.RaffleId == id).ToList();
            return View(raffleItems);
        }
    }
}