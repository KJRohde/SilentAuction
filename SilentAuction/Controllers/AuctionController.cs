using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
using SilentAuction.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static SilentAuction.Models.LineChart;

namespace SilentAuction.Controllers
{
    public class AuctionController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        // GET: Auction
        public ActionResult Index(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            return View(auction);
        }
        public ActionResult CompletedIndex(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            return View(auction);
        }
        public ActionResult Create()
        {
            Auction auction = new Auction();
            return View(auction);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "Name,Description,StartTime,EndTime,Donors,AuctionId,ManagerId,Day,Message,ChartData,TotalRaised")] Auction auction)
        {
            var currentUserId = User.Identity.GetUserId();
            Manager manager = context.Managers.FirstOrDefault(m => m.ApplicationUserId == currentUserId);
            auction.ManagerId = manager.ManagerId;
            auction.TotalRaised = 0;
            if (manager.ApplicationUserId == currentUserId)
            {
                context.Auctions.Add(auction);
                context.SaveChanges();
                return RedirectToAction("Index", new { id = auction.AuctionId });
            }

            return View(auction);
        }

        public ActionResult Participate(int id, string category)
        {
            var auctionItems = context.AuctionPrizes.Where(a => a.AuctionId == id).ToList();
            if (category == null || category == "All")
            {
                return View(auctionItems);
            }
            else
            {
                var selectedItems = auctionItems.Where(s => s.Category.ToString() == category.ToString()).ToList();
                return View(selectedItems);
            }
        }
        public async System.Threading.Tasks.Task<ActionResult> CloseAuctionAsync(int auctionId)
        {
            var auction = context.Auctions.FirstOrDefault(u => u.AuctionId == auctionId);
            auction.EndTime = DateTime.Now;
            var prizes = context.AuctionPrizes.Where(p => p.AuctionId == auction.AuctionId).ToList();

            foreach (AuctionPrize prize in prizes)
            {
                Participant winner = context.Participants.FirstOrDefault(w => w.ParticipantId == prize.TopParticipant);
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction@gmail.com";
                    string FromAdressTitle = "Silent Auction App";
                    //To Address    
                    string ToAddress = winner.EmailAddress;
                    string ToAdressTitle = "Winner";
                    string Subject = "You're a Winner!";
                    string BodyContent = winner.FirstName + " " + winner.LastName + ",\nYou have won the following prize!\nName: " + prize.Name + "\nDescription: " + prize.Description + "\nBid: " + prize.CurrentBid + "\nPayments can be made in your 'Prizes Won' tab in the app.\n" + prize.Auction.Message;

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
            foreach (AuctionPrize prize in prizes)
            {
                prize.ParticipantId = prize.TopParticipant;
                AddTransaction(prize);
            }
            await context.SaveChangesAsync();
            return RedirectToAction("CompletedAuctions", "Manager");
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = context.Auctions.Where(c => c.AuctionId == id).Single();
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Description,StartTime,EndTime,Donors,AuctionId,ManagerId,Day,Message,ChartData,TotalRaised")]int id, Auction auction)
        {
            if (ModelState.IsValid)
            {
                Auction auctionToEdit = context.Auctions.Where(c => c.AuctionId == id).Single();
                auctionToEdit.Name = auction.Name;
                auctionToEdit.Description = auction.Description;
                auctionToEdit.StartTime = auction.StartTime;
                auctionToEdit.EndTime = auction.EndTime;
                auctionToEdit.Day = auction.Day;
                auctionToEdit.Message = auction.Message;
                auctionToEdit.Donors = auction.Donors;
                context.SaveChanges();
                return RedirectToAction("Index", new { id = auction.AuctionId });
            }
            return View(auction);
        }
        public ActionResult GetAuctionPrizes(int id)
        {
            var prizes = context.AuctionPrizes.Where(p => p.AuctionId == id);
            return View(prizes);
        }
        public async System.Threading.Tasks.Task<ActionResult> EmailBlastAsync(int auctionId)
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
                    string BodyContent = participant.FirstName + " " + participant.LastName + ",\nThere is currently a silent auction live on the Silent Auction App! Check your account portal for the auction, " + auction.Name + " to win some fabulous prizes!";

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
        public ActionResult EmailSent(int id)
        {
            Auction auction = context.Auctions.FirstOrDefault(a => a.AuctionId == id);
            return View(auction);
        }
        public Transaction AddTransaction(AuctionPrize prize)
        {
            Transaction transaction = new Transaction();
            transaction.Money = prize.CurrentBid;
            transaction.ManagerId = prize.Auction.ManagerId;
            transaction.ParticipantId = prize.ParticipantId;
            transaction.Paid = false;
            transaction.Description = "Winning " + prize.Name + "from " + prize.Auction.Name + ".";
            context.SaveChanges();
            return transaction;
        }
    }
}