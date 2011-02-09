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
        public delegate void GameTypeDiscoveredHandler(String gameType);
        public event GameTypeDiscoveredHandler GameTypeDiscovered;

        public UniversalHHParser(PokerClient pokerClient)
            : base(pokerClient)
        {
            reGameType = pokerClient.GetRegex("hand_history_game_type_token");
        }

        public override void ParseLine(string line)
        {
            Match match = reGameType.Match(line);

            if (match.Success)
            {
                String gameType = match.Groups["gameType"].Value;

                if (GameTypeDiscovered != null) GameTypeDiscovered(gameType);
            }
        }
    }
}
