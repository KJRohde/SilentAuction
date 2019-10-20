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
        public ActionResult Index()
        {
            var currentUser = User.Identity.GetUserId();
            Manager manager = context.Managers.Where(m => m.ApplicationUserId == currentUser).Single();
            var myModel = new ViewModel
            {
                Auctions = context.Auctions.Where(s => s.ManagerId == manager.ManagerId && s.EndTime > DateTime.Now).ToList(),
                Raffles = context.Raffles.Where(r => r.ManagerId == manager.ManagerId && r.EndTime > DateTime.Now).ToList()
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

        public ActionResult CompletedAuctions()
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.Where(m => m.ApplicationUserId == currentUserId).Single();
            var completedAuctions = context.Auctions.Where(a => a.ManagerId == manager.ManagerId && a.EndTime < DateTime.Now).ToList();
            return View(completedAuctions);
        }
        public ActionResult CompletedRaffles()
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.Where(m => m.ApplicationUserId == currentUserId).Single();
            var completedRaffles = context.Raffles.Where(a => a.ManagerId == manager.ManagerId && a.EndTime < DateTime.Now).ToList();
            return View(completedRaffles);
        }
        public async System.Threading.Tasks.Task<ActionResult> EmailBlastAuctionAsync(int auctionId)
        {
            var auction = context.Auctions.FirstOrDefault(a => a.AuctionId == auctionId);
            var participants = context.Participants.ToList();

            foreach (Participant participant in participants)
            {
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction@gmail.com";
                    string FromAdressTitle = "Silent Auction App";
                    //To Address    
                    string ToAddress = participant.EmailAddress;
                    string ToAdressTitle = "Winner";
                    string Subject = "Live Silent Auction!";
                    string BodyContent = participant.FirstName + " " + participant.LastName + ",\nThere are events underway in the Silent Auction App! Check your account portal for the event, " + auction.Name + " to win some fabulous prizes!";

                    //Smtp Server    
                    string SmtpServer = "smtp.gmail.com";
                    //Smtp Port Number    
                    int SmtpPortNumber = 587;

                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress
                                            (FromAdressTitle,
                                             FromAddress
                                             ));
                    mimeMessage.To.Add(new MailboxAddress
                                             (ToAdressTitle,
                                             ToAddress
                                             ));
                    mimeMessage.Subject = Subject; //Subject  
                    mimeMessage.Body = new TextPart("plain")
                    {
                        Text = BodyContent
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, SmtpPortNumber, false);
                        client.Authenticate(
                            "DCCSilentAuction@gmail.com",
                            "!234Qwer"
                            );
                        await client.SendAsync(mimeMessage);
                        await client.DisconnectAsync(true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            return RedirectToAction("EmailSent", "Auction", new { id = auctionId });
        }
        public async System.Threading.Tasks.Task<ActionResult> EmailBlastRaffleAsync(int raffleId)
        {
            var raffle = context.Raffles.FirstOrDefault(a => a.RaffleId == raffleId);
            var participants = context.Participants.ToList();

            foreach (Participant participant in participants)
            {
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction@gmail.com";
                    string FromAdressTitle = "Silent Auction App";
                    //To Address    
                    string ToAddress = participant.EmailAddress;
                    string ToAdressTitle = "Winner";
                    string Subject = "Live Silent Auction!";
                    string BodyContent = participant.FirstName + " " + participant.LastName + ",\nThere are events underway in the Silent Auction App! Check your account portal for the event, " + raffle.Name + " to win some fabulous prizes!";

                    //Smtp Server    
                    string SmtpServer = "smtp.gmail.com";
                    //Smtp Port Number    
                    int SmtpPortNumber = 587;

                    var mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress
                                            (FromAdressTitle,
                                             FromAddress
                                             ));
                    mimeMessage.To.Add(new MailboxAddress
                                             (ToAdressTitle,
                                             ToAddress
                                             ));
                    mimeMessage.Subject = Subject; //Subject  
                    mimeMessage.Body = new TextPart("plain")
                    {
                        Text = BodyContent
                    };

                    using (var client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, SmtpPortNumber, false);
                        client.Authenticate(
                            "DCCSilentAuction@gmail.com",
                            "!234Qwer"
                            );
                        await client.SendAsync(mimeMessage);
                        await client.DisconnectAsync(true);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            return RedirectToAction("EmailSent", "Raffle", new { id = raffleId });
        }
        public ActionResult GetTransactions()
        {
            var currentUser = User.Identity.GetUserId();
            Manager manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUser);
            var myModel = new ViewModel
            {
                Participants = context.Participants.ToList(),
                Transactions = context.Transactions.Where(r => r.ManagerId == manager.ManagerId).ToList()
            };
            return View(myModel);
        }
    }
}