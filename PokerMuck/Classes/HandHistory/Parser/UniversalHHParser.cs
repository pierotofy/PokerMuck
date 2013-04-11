using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace PokerMuck
{
    /* This basic hand history parser only recognizes what type of game an history file might be. */
    class UniversalHHParser : HHParser
    {
        private Regex reGameType;

        /* We found what kind of game this file uses! The delegate can then select a more specialized HHParser */
        public delegate void GameDiscoveredHandler(String gameType);
        public event GameDiscoveredHandler GameDiscovered;

        public UniversalHHParser(PokerClient pokerClient, String handhistoryFilename)
            : base(pokerClient, handhistoryFilename)
        {
            reGameType = pokerClient.GetRegex("hand_history_game_token");
        }

        public override void ParseLine(string line)
        {
            base.ParseLine(line);

            Match match = reGameType.Match(line);

            if (match.Success)
            {
                String gameType = match.Groups["gameType"].Value;

                if (GameDiscovered != null) GameDiscovered(gameType);
            }
            else
            {
                Trace.WriteLine("Failed to match a gameType from the universal parser: " + line);
            }
        }

        /* We don't need to check for the end of a round with the universal parser
         * (only exception) */
        protected override void CheckForEndOfRound(string line)
        {
        }
    }
}
