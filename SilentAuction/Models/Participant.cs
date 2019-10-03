using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string EmailAddress { get; set; }
        public double TotalSpent { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RaffleTickets { get; set; }
        public string RecentActionOne { get; set; }
        public string RecentActionTwo { get; set; }
        public string RecentActionThree { get; set; }

    }
}