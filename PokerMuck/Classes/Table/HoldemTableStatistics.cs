using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    class HoldemTableStatistics : TableStatistics
    {
        /* Information about the blinds */
        public float BigBlindAmount { get; set; }
        public float SmallBlindAmount { get; set; }

        /* Somebody limped preflop */
        private bool PlayerLimpedPreflop;

        /* Somebody raised preflop */
        private bool PlayerRaisedPreflop;

        /* Somebody checked preflop */
        private bool PlayerCheckedPreflop;

        /* Somebody bet the flop */
        private bool PlayerBetTheFlopThisRound;

        /* Somebody cbet the flop (which is different than simple betting) */
        private bool PlayerCBetThisRound;

        /* Somebody raised the flop */
        private bool PlayerRaisedTheFlopThisRound;

        /* Somebody steal raised */
        private bool PlayerStealRaisedPreflop;

        /* Small blind reraised a steal raise? */
        private bool SmallBlindReraisedAStealRaise;

        private int buttonSeat; 

        public HoldemTableStatistics(Table table)
            : base(table)
        {
            PrepareStatisticsForNewRound();
        }

        public override void PrepareStatisticsForNewRound()
        {
            base.PrepareStatisticsForNewRound();

            PlayerBetTheFlopThisRound = false;
            PlayerRaisedTheFlopThisRound = false;
            PlayerCBetThisRound = false;
            PlayerRaisedPreflop = false;
            PlayerLimpedPreflop = false;
            PlayerCheckedPreflop = false;
            PlayerStealRaisedPreflop = false;
            SmallBlindReraisedAStealRaise = false;
        }

        public override void RegisterParserHandlers(HHParser parser)
        {
            ((HoldemHHParser)parser).FoundBigBlind += new HoldemHHParser.FoundBigBlindHandler(handHistoryParser_FoundBigBlind);
            ((HoldemHHParser)parser).FoundSmallBlind += new HoldemHHParser.FoundSmallBlindHandler(handHistoryParser_FoundSmallBlind);
            ((HoldemHHParser)parser).PlayerBet += new HoldemHHParser.PlayerBetHandler(handHistoryParser_PlayerBet);
            ((HoldemHHParser)parser).PlayerCalled += new HoldemHHParser.PlayerCalledHandler(handHistoryParser_PlayerCalled);
            ((HoldemHHParser)parser).PlayerFolded += new HoldemHHParser.PlayerFoldedHandler(handHistoryParser_PlayerFolded);
            ((HoldemHHParser)parser).PlayerRaised += new HoldemHHParser.PlayerRaisedHandler(handHistoryParser_PlayerRaised);
            ((HoldemHHParser)parser).PlayerChecked += new HoldemHHParser.PlayerCheckedHandler(handHistoryParser_PlayerChecked);
            ((HoldemHHParser)parser).FoundButton += new HoldemHHParser.FoundButtonHandler(handHistoryParser_FoundButton);
            ((HoldemHHParser)parser).HoleCardsWillBeDealt += new HHParser.HoleCardsWillBeDealtHandler(HoldemTableStatistics_HoleCardsWillBeDealt);
        }

        void HoldemTableStatistics_HoleCardsWillBeDealt()
        {
            Debug.Print("Holw cards will be dealt");
            // The player that is seating at seatNumber gets the button
            foreach (HoldemPlayer p in table.PlayerList)
            {
                p.IsButton = (p.SeatNumber == buttonSeat);
                if (p.IsButton) Debug.Print("Button is now: " + p.Name);
            }
        }

        void handHistoryParser_FoundButton(int seatNumber)
        {
            buttonSeat = seatNumber;
        }

        void handHistoryParser_PlayerRaised(string playerName, float initialPot, float raiseAmount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = FindPlayer(playerName);

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                /* If this player is the button and he raises while nobody raised or limped before him
                 * this is a good candidate for a steal raise */
                if (p.IsButton && !PlayerRaisedPreflop && !PlayerLimpedPreflop && !PlayerCheckedPreflop)
                {
                    p.IncrementOpportunitiesToStealRaise();
                    p.IncrementStealRaises();

                    PlayerStealRaisedPreflop = true;
                }                
                    
                /* re-raise to a steal raise? */
                else if (p.IsSmallBlind && PlayerStealRaisedPreflop)
                {
                    p.IncrementRaisesToAStealRaise(BlindType.SmallBlind);
                    SmallBlindReraisedAStealRaise = true;
                }
                else if (p.IsBigBlind && PlayerStealRaisedPreflop && !SmallBlindReraisedAStealRaise)
                {
                    p.IncrementRaisesToAStealRaise(BlindType.BigBlind);
                }

                PlayerRaisedPreflop = true;
            }
            else if (gamePhase == HoldemGamePhase.Flop)
            {
                // Has somebody cbet?
                if (!PlayerRaisedTheFlopThisRound && PlayerCBetThisRound)
                {
                    p.IncrementRaiseToACBet();
                }

                PlayerRaisedTheFlopThisRound = true;
            }

            
            p.HasRaised(gamePhase);
        }

        void handHistoryParser_PlayerFolded(string playerName, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = FindPlayer(playerName);

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                /* Steal raise opportunity */
                if (p.IsButton && !PlayerRaisedPreflop && !PlayerLimpedPreflop && !PlayerCheckedPreflop)
                {
                    p.IncrementOpportunitiesToStealRaise();
                }

                /* Folded to a steal raise? */
                else if (p.IsSmallBlind && PlayerStealRaisedPreflop)
                {
                    p.IncrementFoldsToAStealRaise(BlindType.SmallBlind);
                }
                else if (p.IsBigBlind && PlayerStealRaisedPreflop && !SmallBlindReraisedAStealRaise)
                {
                    p.IncrementFoldsToAStealRaise(BlindType.BigBlind);
                }
            }
            // Has somebody cbet?
            else if(gamePhase == HoldemGamePhase.Flop && !PlayerRaisedTheFlopThisRound && PlayerCBetThisRound)
            {
                p.IncrementFoldToACBet();
            }

            p.HasFolded(gamePhase);
        }

        void handHistoryParser_PlayerChecked(string playerName, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = FindPlayer(playerName);

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                /* Steal raise opportunity */
                if (p.IsButton && !PlayerRaisedPreflop && !PlayerLimpedPreflop && !PlayerCheckedPreflop)
                {
                    p.IncrementOpportunitiesToStealRaise();
                }

                PlayerCheckedPreflop = true;
            }

            // Flop
            if (gamePhase == HoldemGamePhase.Flop && !PlayerBetTheFlopThisRound && p.HasPreflopRaisedThisRound())
            {
                p.IncrementOpportunitiesToCBet(false);
            }

            p.HasChecked(gamePhase);
        }

        void handHistoryParser_PlayerCalled(string playerName, float amount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = FindPlayer(playerName);

            // If we are preflop
            if (gamePhase == HoldemGamePhase.Preflop)
            {
                // If the call is the same amount as the big blind, this is also a limp
                if (amount == BigBlindAmount)
                {
                    PlayerLimpedPreflop = true;

                    // HasLimped() makes further checks to avoid duplicate counts and whether the player is the big blind or small blind
                    p.CheckForLimp();
                }

                /* Steal raise opportunity */
                if (p.IsButton && !PlayerRaisedPreflop && !PlayerLimpedPreflop && !PlayerCheckedPreflop)
                {
                    p.IncrementOpportunitiesToStealRaise();
                }

                /* Called a steal raise? */
                else if (p.IsSmallBlind && PlayerStealRaisedPreflop)
                {
                    p.IncrementCallsToAStealRaise(BlindType.SmallBlind);
                }
                else if (p.IsBigBlind && PlayerStealRaisedPreflop && !SmallBlindReraisedAStealRaise)
                {
                    p.IncrementCallsToAStealRaise(BlindType.BigBlind);
                }
            }
            else if (gamePhase == HoldemGamePhase.Flop)
            {
                // Has somebody cbet?
                if (!PlayerRaisedTheFlopThisRound && PlayerCBetThisRound)
                {
                    p.IncrementCallToACBet();
                }
            }

            p.HasCalled(gamePhase);
        }

        void handHistoryParser_PlayerBet(string playerName, float amount, HoldemGamePhase gamePhase)
        {
            HoldemPlayer p = FindPlayer(playerName);

            // Flop
            if (gamePhase == HoldemGamePhase.Flop && !PlayerBetTheFlopThisRound)
            {
                // Did he raised preflop?
                if (p.HasPreflopRaisedThisRound())
                {
                    p.IncrementOpportunitiesToCBet(true);
                    PlayerCBetThisRound = true;
                }

                PlayerBetTheFlopThisRound = true;
            }


            p.HasBet(gamePhase);
        }

        void handHistoryParser_FoundBigBlind(String playerName, float amount)
        {
            // Save current blind
            BigBlindAmount = amount;

            // Keep track of who is the big blind
            HoldemPlayer p = FindPlayer(playerName);
            p.IsBigBlind = true;
        }

        void handHistoryParser_FoundSmallBlind(String playerName, float amount)
        {
            // Save current blind
            SmallBlindAmount = amount;

            // Keep track of who is the small blind
            HoldemPlayer p = FindPlayer(playerName);
            p.IsSmallBlind = true;
        }


        /* Helper function to find a player */
        private HoldemPlayer FindPlayer(String playerName)
        {
            return (HoldemPlayer)table.FindPlayer(playerName);
        }
    }
}
