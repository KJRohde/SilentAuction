using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Name { get; set; }
        public string Description { get; set; }
        //[DisplayName("Upload Image File")]
        //public string ImagePath { get; set; }
        //[NotMapped]
        //public HttpPostedFileBase ImageFile { get; set; }
        public int? WinnerId { get; set; }
        public int CurrentTickets { get; set; }
        public string Category { get; set; }

    }
}