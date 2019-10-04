using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public double Money { get; set; }
        [ForeignKey("Participant")]
        public int? ParticipantId { get; set; }
        public Participant Participant { get; set; }
        [ForeignKey("Manager")]
        public int? ManagerId { get; set; }
        public Manager Manager { get; set; }
        public bool Paid { get; set; }
        public string Description { get; set; }
    }
}