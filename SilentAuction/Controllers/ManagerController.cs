using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SilentAuction.Controllers
{
    public class ManagerController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Managers
        public ActionResult Index(int id)
        {

            Manager manager = context.Managers.Where(m => m.ManagerId == id).Single();
            var myModel = new ViewModel
            {
                Auctions = context.Auctions.Where(s => s.ManagerId == manager.ManagerId).ToList(),
                Raffles = context.Raffles.Where(r => r.ManagerId == manager.ManagerId).ToList()
            };
            return View(myModel);
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
        public ActionResult Create([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId,ManagerId")] Manager manager)
        {
            var currentUserId = User.Identity.GetUserId();
            manager.ApplicationUserId = currentUserId;
            manager.EmailAddress = User.Identity.GetUserName();
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Managers.Add(manager);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = manager.ManagerId });
            }

            return View(manager);
        }
        public ActionResult Edit()
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.Where(m => m.ApplicationUserId == currentUserId).Single();
            if (manager == null)
            {
                return HttpNotFound();
            }
            return View(manager);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FirstName,LastName,EmailAddress,ApplicationUserId,ManagerId")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                Manager managerToEdit = context.Managers.Where(c => c.ManagerId == manager.ManagerId).Single();
                managerToEdit.FirstName = manager.FirstName;
                managerToEdit.LastName = manager.LastName;
                managerToEdit.EmailAddress = manager.EmailAddress;
                context.SaveChanges();
                return RedirectToAction("Index", new { id = manager.ManagerId });
            }
            return View(manager);
        }
        //public async System.Threading.Tasks.Task<ActionResult> CloseAuctionAsync(int auctionId)
        //{
        //        var auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionId);
        //        auction.EndTime = DateTime.Now;
        //        context.SaveChanges();
        //        var prizes = context.AuctionPrizes.Where(p => p.AuctionId == auction.AuctionId).ToList();
        //        foreach (AuctionPrize prize in prizes)
        //        {
        //            Participant winner = context.Participants.FirstOrDefault(w => w.ParticipantId == prize.WinnerId);
        //            try
        //            {
        //                //From Address    
        //                string FromAddress = "DCCSilentAuction@gmail.com";
        //                string FromAdressTitle = "Silent Auction App";
        //                //To Address    
        //                string ToAddress = winner.EmailAddress;
        //                string ToAdressTitle = "Winner";
        //                string Subject = "You're a Winner!";
        //                string BodyContent = prize.Auction.Message;

        //                //Smtp Server    
        //                string SmtpServer = "smtp.gmail.com";
        //                //Smtp Port Number    
        //                int SmtpPortNumber = 587;

        //                var mimeMessage = new MimeMessage();
        //                mimeMessage.From.Add(new MailboxAddress
        //                                        (FromAdressTitle,
        //                                         FromAddress
        //                                         ));
        //                mimeMessage.To.Add(new MailboxAddress
        //                                         (ToAdressTitle,
        //                                         ToAddress
        //                                         ));
        //                mimeMessage.Subject = Subject; //Subject  
        //                mimeMessage.Body = new TextPart("plain")
        //                {
        //                    Text = BodyContent
        //                };

        //                using (var client = new SmtpClient())
        //                {
        //                    client.Connect(SmtpServer, SmtpPortNumber, false);
        //                    client.Authenticate(
        //                        "DCCSilentAuction@gmail.com",
        //                        "!234Qwer"
        //                        );
        //                    await client.SendAsync(mimeMessage);
        //                    Console.WriteLine("The mail has been sent successfully !!");
        //                    Console.ReadLine();
        //                    await client.DisconnectAsync(true);
        //                    return RedirectToAction("SentEmail", "Manager");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;

        //            }
        //        }
        //    return View();
        //}
        //public ActionResult SentEmail()
        //{
        //    return View();
        //}
    }
}