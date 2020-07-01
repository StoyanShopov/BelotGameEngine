﻿namespace Belot.AI.SmartPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Belot.Engine;
    using Belot.Engine.Game;
    using Belot.Engine.GameMechanics;
    using Belot.Engine.Players;

    public class SmartPlayer : IPlayer
    {
        private readonly TrickWinnerService trickWinnerService;

        private readonly List<BidType> allBids = new List<BidType>
                                                     {
                                                         BidType.Pass,
                                                         BidType.Clubs,
                                                         BidType.Diamonds,
                                                         BidType.Hearts,
                                                         BidType.Spades,
                                                         BidType.NoTrumps,
                                                         BidType.AllTrumps,
                                                     };

        public SmartPlayer()
        {
            this.trickWinnerService = new TrickWinnerService();
        }

        public BidType GetBid(PlayerGetBidContext context)
        {
            return ThreadSafeRandom.Next(0, 2) == 0
                       ? BidType.Pass // In 50% of the cases announce Pass
                       : this.allBids.Where(x => context.AvailableBids.HasFlag(x)).RandomElement();
        }

        public IEnumerable<Announce> GetAnnounces(PlayerGetAnnouncesContext context)
        {
            return context.AvailableAnnounces;
        }

        public PlayCardAction PlayCard(PlayerPlayCardContext context)
        {
            return new PlayCardAction(
                context.AvailableCardsToPlay.OrderBy(x => x.GetValue(context.CurrentContract.Type))
                    .FirstOrDefault());
        }
    }
}
