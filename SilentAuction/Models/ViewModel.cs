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
        public IEnumerable<ParticipantAction> ParticipantActions { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
        public IEnumerable<RafflePrize> RafflePrizes { get; set; }
        public IEnumerable<AuctionPrize> AuctionPrizes { get; set; }
        public Participant Participant { get; set; }
        public Raffle Raffle { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}