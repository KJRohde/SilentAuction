using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SilentAuction.Models
{
    public class ViewModel
    {
        public IEnumerable<Auction> Auctions { get; set; }
        public IEnumerable<Raffle> Raffles { get; set; }

    }
}