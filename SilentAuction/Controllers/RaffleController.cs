using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using MimeKit;
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
        public ActionResult CompletedIndex(int id)
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
        public ActionResult Participate(int id, string category)
        {
            var currentUser = User.Identity.GetUserId();
            if (category == null)
            {
                var myModel = new ViewModel
                {
                    Participant = context.Participants.FirstOrDefault(a => a.ApplicationUserId == currentUser),
                    Raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id),
                    RafflePrizes = context.RafflePrizes.Where(p => p.RaffleId == id).ToList()
                };
                return View(myModel);
            }
            else
            {
                var myModel = new ViewModel
                {
                    Participant = context.Participants.FirstOrDefault(a => a.ApplicationUserId == currentUser),
                    Raffle = context.Raffles.FirstOrDefault(r => r.RaffleId == id),
                    RafflePrizes = context.RafflePrizes.Where(p => p.RafflePrizeId == id && p.Category == category).ToList()
                };
                return View(myModel);
            }
        }
        public ActionResult EmailSent(int id)
        {
            Raffle raffle = context.Raffles.FirstOrDefault(a => a.RaffleId == id);
            return View(raffle);
        }

        public async System.Threading.Tasks.Task<ActionResult> CloseRaffleAsync(int raffleId)
        {
            var raffle = context.Raffles.FirstOrDefault(u => u.RaffleId == raffleId);
            raffle.EndTime = DateTime.Now;
            var prizes = context.RafflePrizes.Where(p => p.RaffleId == raffle.RaffleId).ToList();

            foreach (RafflePrize prize in prizes)
            {
                Random random = new Random();
                var tickets = context.Tickets.Where(t => t.RafflePrizeId == prize.RafflePrizeId).ToList();
                Ticket winningTicket = tickets[random.Next(0, tickets.Count)];
                Participant winner = context.Participants.FirstOrDefault(w => w.ParticipantId == winningTicket.ParticipantId);
                prize.WinnerId = winner.ParticipantId;
                try
                {
                    //From Address    
                    string FromAddress = "DCCSilentAuction@gmail.com";
                    string FromAdressTitle = "Silent Auction App";
                    //To Address    
                    string ToAddress = winner.EmailAddress;
                    string ToAdressTitle = "Winner";
                    string Subject = "You're a Winner!";
                    string BodyContent = winner.FirstName + " " + winner.LastName + ",\nYou have won the following prize!\nName: " + prize.Name + "\nDescription: " + prize.Description + ". Please make sure you have paid for all tickets before claiming your prize.";

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
            await context.SaveChangesAsync();
            return RedirectToAction("CompletedRaffles", "Manager");
        }
    }
}