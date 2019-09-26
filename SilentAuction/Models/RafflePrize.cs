using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class RafflePrize
    {
        [Key]
        public int RafflePrizeId { get; set; }
        [ForeignKey("Raffle")]
        public int RaffleId { get; set; }
        public Raffle Raffle { get; set; }
        public double Value { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public Participant Winner { get; set; }
        public int CurrentTickets { get; set; }
    }
}