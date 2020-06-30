﻿namespace Belot.Engine.GameMechanics
{
    using System.Collections.Generic;

    using Belot.Engine.Cards;
    using Belot.Engine.Game;
    using Belot.Engine.Players;

    public class ContractManager
    {
        private readonly IList<IPlayer> players;

        public ContractManager(IPlayer southPlayer, IPlayer eastPlayer, IPlayer northPlayer, IPlayer westPlayer)
        {
            this.players = new List<IPlayer>(4) { southPlayer, eastPlayer, northPlayer, westPlayer };
        }

        public BidType GetContract(
            int roundNumber,
            PlayerPosition firstToPlay,
            int southNorthTeamPoints,
            int eastWestTeamPoints,
            IReadOnlyList<CardCollection> playerCards,
            out IList<Bid> bids)
        {
            bids = new List<Bid>();
            var consecutivePasses = 0;
            var currentPlayerPosition = firstToPlay;
            var contract = BidType.Pass;
            var contractByPlayer = currentPlayerPosition;
            var bidContext = new PlayerGetBidContext
            {
                RoundNumber = roundNumber,
                FirstToPlayInTheRound = firstToPlay,
                EastWestTeamPoints = eastWestTeamPoints,
                SouthNorthTeamPoints = southNorthTeamPoints,
                Bids = bids,
            };
            while (true)
            {
                var availableBids = this.GetAvailableBids(contract, contractByPlayer, currentPlayerPosition);

                BidType bid;
                if (availableBids == BidType.Pass)
                {
                    // Only pass is available so we don't ask the player
                    bid = BidType.Pass;
                }
                else
                {
                    bidContext.AvailableBids = availableBids;
                    bidContext.MyCards = playerCards[currentPlayerPosition.Index()];
                    bidContext.MyPosition = currentPlayerPosition;
                    bidContext.CurrentContract = contract;
                    bid = this.players[currentPlayerPosition.Index()].GetBid(bidContext);
                    if (bid != BidType.Pass && (bid & (bid - 1)) != 0)
                    {
                        throw new BelotGameException($"Invalid bid from {currentPlayerPosition} player. More than 1 flags returned.");
                    }

                    if (!availableBids.HasFlag(bid))
                    {
                        throw new BelotGameException($"Invalid bid from {currentPlayerPosition} player. This bid is not permitted.");
                    }

                    if (bid == BidType.Double || bid == BidType.ReDouble)
                    {
                        contract &= ~BidType.Double;
                        contract |= bid;
                        contractByPlayer = currentPlayerPosition;
                    }
                    else if (bid != BidType.Pass)
                    {
                        contract = bid;
                        contractByPlayer = currentPlayerPosition;
                    }
                }

                bids.Add(new Bid(currentPlayerPosition, bid));

                consecutivePasses = (bid == BidType.Pass) ? consecutivePasses + 1 : 0;
                if (contract == BidType.Pass && consecutivePasses == 4)
                {
                    break;
                }

                if (contract != BidType.Pass && consecutivePasses == 3)
                {
                    break;
                }

                currentPlayerPosition = currentPlayerPosition.Next();
            }

            return contract;
        }

        private BidType GetAvailableBids(
            BidType currentContract,
            PlayerPosition contractByPlayer,
            PlayerPosition currentPlayer)
        {
            var cleanContract = currentContract;
            cleanContract &= ~BidType.Double;
            cleanContract &= ~BidType.ReDouble;

            var availableBids = BidType.Pass;
            if (cleanContract < BidType.Clubs)
            {
                availableBids |= BidType.Clubs;
            }

            if (cleanContract < BidType.Diamonds)
            {
                availableBids |= BidType.Diamonds;
            }

            if (cleanContract < BidType.Hearts)
            {
                availableBids |= BidType.Hearts;
            }

            if (cleanContract < BidType.Spades)
            {
                availableBids |= BidType.Spades;
            }

            if (cleanContract < BidType.NoTrumps)
            {
                availableBids |= BidType.NoTrumps;
            }

            if (cleanContract < BidType.AllTrumps)
            {
                availableBids |= BidType.AllTrumps;
            }

            if (!currentPlayer.IsInSameTeamWith(contractByPlayer) && currentContract != BidType.Pass)
            {
                if (currentContract.HasFlag(BidType.Double))
                {
                    availableBids |= BidType.ReDouble;
                }
                else if (currentContract.HasFlag(BidType.ReDouble))
                {
                }
                else
                {
                    availableBids |= BidType.Double;
                }
            }

            return availableBids;
        }
    }
}
