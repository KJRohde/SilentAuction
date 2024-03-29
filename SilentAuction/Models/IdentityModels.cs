﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SilentAuction.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Raffle> Raffles { get; set; }
        public DbSet<RafflePrize> RafflePrizes { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionPrize> AuctionPrizes { get; set; }
        public DbSet<tblAuctionPrize> tblAuctionPrizes { get; set; }
        public DbSet<tblRafflePrize> tblRafflePrizes { get; set; }
        public DbSet<Data> Data { get; set; }
        public DbSet<ParticipantAction> ParticipantActions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}