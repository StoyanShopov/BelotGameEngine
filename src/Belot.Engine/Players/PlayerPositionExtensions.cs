﻿namespace Belot.Engine.Players
{
    using System.Runtime.CompilerServices;

    public static class PlayerPositionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PlayerPosition Next(this PlayerPosition playerPosition)
        {
            return playerPosition == PlayerPosition.South ? PlayerPosition.East :
                   playerPosition == PlayerPosition.East ? PlayerPosition.North :
                   playerPosition == PlayerPosition.North ? PlayerPosition.West : PlayerPosition.South;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Index(this PlayerPosition playerPosition)
        {
            return playerPosition == PlayerPosition.East ? 1 :
                   playerPosition == PlayerPosition.North ? 2 :
                   playerPosition == PlayerPosition.West ? 3 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInSameTeamWith(this PlayerPosition position, PlayerPosition otherPlayerPosition)
        {
            return (position == PlayerPosition.South && otherPlayerPosition == PlayerPosition.North)
                   || (position == PlayerPosition.North && otherPlayerPosition == PlayerPosition.South)
                   || (position == PlayerPosition.East && otherPlayerPosition == PlayerPosition.West)
                   || (position == PlayerPosition.West && otherPlayerPosition == PlayerPosition.East);
        }
    }
}
