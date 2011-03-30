using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* Types of blinds */
    public enum BlindType { BigBlind, SmallBlind, NoBlind };

    /* A holdem player has certain statistics that a five card draw player might not have */
    class HoldemPlayer : Player
    {
        /* Limp */
        private ValueCounter limps;

        /* VPF */
        private ValueCounter voluntaryPutMoneyPreflop;

        /* Raises */
        private MultipleValueCounter raises;

        /* Bets */
        private MultipleValueCounter bets;

        /* Folds */
        private MultipleValueCounter folds;

        /* Calls */
        private MultipleValueCounter calls;
        
        /* C Bets */ 
        private int opportunitiesToCBet;

        private ValueCounter cbets;

        private int foldsToACBet;
        private int callsToACBet;
        private int raisesToACBet;

        /* Steal raise from the button */
        private ValueCounter stealRaises;
        private int opportunitiesToStealRaise;

        /* Fold/call/raise to a steal raise */
        private MultipleValueCounter foldsToAStealRaise;
        private MultipleValueCounter callsToAStealRaise;
        private MultipleValueCounter raisesToAStealRaise;

        /* Fold/call/raise blind to a raise preflop (steal or not) */
        private MultipleValueCounter foldsBlindToAPreflopRaise;
        private MultipleValueCounter callsBlindToAPreflopRaise;
        private MultipleValueCounter raisesBlindToAPreflopRaise;

        /* Checks */
        private MultipleValueCounter checks;

        /* Check-raise */
        private MultipleValueCounter checkRaises;

        /* Check-fold */
        private MultipleValueCounter checkFolds;

        /* Check-call */
        private MultipleValueCounter checkCalls;

        /* How many times has the player pushed all-in? */
        private ValueCounter totalAllIns;

        /* How many times have we seen a particular street? */
        private MultipleValueCounter sawStreet;

        /* Went and won at showdown */
        private ValueCounter wentToShowdown;
        private ValueCounter wonAtShowdown;

        /* Starting hands range on a limp or call */
        private List<HoldemHand> startingHandsWithPreflopCall;

        /* Starting hands range when raisor preflop */
        private List<HoldemHand> startingHandsWithPreflopRaise;

        /* Each table is set this way:
         key => value
         GamePhase => value
         Ex. calls[flop] == 4 --> player has flat called 4 times during the flop
         */
        private Hashtable totalCalls;
        private Hashtable totalBets;
        private Hashtable totalFolds;
        private Hashtable totalRaises;
        private Hashtable totalChecks;

        private BlindType blindType;
        public BlindType BlindType { get { return blindType; } }

        public bool IsBigBlind
        {
            get
            {
                return blindType == BlindType.BigBlind;
            }
            set
            {
                blindType = value ? BlindType.BigBlind : BlindType.NoBlind;
            }
        }
        public bool IsSmallBlind {
            get
            {
                return blindType == BlindType.SmallBlind;
            }
            set
            {
                blindType = value ? BlindType.SmallBlind : BlindType.NoBlind;
            }
        }
        public bool IsButton { get; set; }


        public override PokerGameType GameType
        {
            get
            {
                return PokerGameType.Holdem;
            }
        }


        public override Player Clone()
        {
            return new HoldemPlayer(this);
        }

        // Clone
        protected HoldemPlayer(HoldemPlayer other) :
            base(other)
        {
            this.blindType = other.blindType;
            this.limps = (ValueCounter)other.limps.Clone();
            this.voluntaryPutMoneyPreflop = (ValueCounter)other.limps.Clone();
            this.raises = (MultipleValueCounter)other.raises.Clone();
            this.bets = (MultipleValueCounter)other.bets.Clone();
            this.calls = (MultipleValueCounter)other.calls.Clone();
            this.folds = (MultipleValueCounter)other.calls.Clone();
            this.opportunitiesToCBet = other.opportunitiesToCBet;
            this.cbets = (ValueCounter)other.cbets.Clone();
            this.foldsToACBet = other.foldsToACBet;
            this.callsToACBet = other.callsToACBet;
            this.raisesToACBet = other.raisesToACBet;
            this.checks = (MultipleValueCounter)other.checks.Clone();
            this.checkRaises = (MultipleValueCounter)other.checkRaises.Clone();
            this.checkFolds = (MultipleValueCounter)other.checkFolds.Clone();
            this.checkCalls = (MultipleValueCounter)other.checkCalls.Clone();
            this.sawStreet = (MultipleValueCounter)other.sawStreet.Clone();
            this.totalCalls = (Hashtable)other.totalCalls.Clone();
            this.totalBets = (Hashtable)other.totalBets.Clone();
            this.totalFolds = (Hashtable)other.totalFolds.Clone();
            this.totalRaises = (Hashtable)other.totalRaises.Clone();
            this.totalChecks = (Hashtable)other.totalChecks.Clone();
            this.IsBigBlind = other.IsBigBlind;
            this.IsSmallBlind = other.IsSmallBlind;
            this.IsButton = other.IsButton;
            this.stealRaises = (ValueCounter)other.stealRaises.Clone();
            this.opportunitiesToStealRaise = other.opportunitiesToStealRaise;
            this.foldsToAStealRaise = (MultipleValueCounter)other.foldsToAStealRaise.Clone();
            this.callsToAStealRaise = (MultipleValueCounter)other.callsToAStealRaise.Clone();
            this.raisesToAStealRaise = (MultipleValueCounter)other.raisesToAStealRaise.Clone();
            this.wentToShowdown = (ValueCounter)other.wentToShowdown.Clone();
            this.wonAtShowdown = (ValueCounter)other.wonAtShowdown.Clone();
            this.totalAllIns = (ValueCounter)other.totalAllIns.Clone();
            this.foldsBlindToAPreflopRaise = (MultipleValueCounter)other.foldsBlindToAPreflopRaise.Clone();
            this.callsBlindToAPreflopRaise = (MultipleValueCounter)other.callsBlindToAPreflopRaise.Clone();
            this.raisesBlindToAPreflopRaise = (MultipleValueCounter)other.raisesBlindToAPreflopRaise.Clone();

            // TODO clone list!!!!
        
        }

        
        public HoldemPlayer(String playerName)
            : base(playerName)
        {
            blindType = BlindType.NoBlind;

            totalCalls = new Hashtable(5);
            totalBets = new Hashtable(5);
            totalFolds = new Hashtable(5);
            totalRaises = new Hashtable(5);
            totalChecks = new Hashtable(5);

            limps = new ValueCounter();
            voluntaryPutMoneyPreflop = new ValueCounter();

            raises = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checkRaises = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checkFolds = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checkCalls = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checks = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            sawStreet = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River, HoldemGamePhase.Showdown);
            bets = new MultipleValueCounter(HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            folds = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            calls = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);


            cbets = new ValueCounter();
            stealRaises = new ValueCounter();
            foldsToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            callsToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            raisesToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);

            wentToShowdown = new ValueCounter();
            wonAtShowdown = new ValueCounter();

            totalAllIns = new ValueCounter();

            foldsBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            callsBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            raisesBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);

            startingHandsWithPreflopCall = new List<HoldemHand>();
            startingHandsWithPreflopRaise = new List<HoldemHand>();


            ResetAllStatistics();
        }

        /* Resets all statistics counters */
        public override void ResetAllStatistics()
        {
            base.ResetAllStatistics();

            ResetStatistics(totalCalls);
            ResetStatistics(totalBets);
            ResetStatistics(totalFolds);
            ResetStatistics(totalRaises);
            ResetStatistics(totalChecks);

            limps.Reset();
            checks.Reset();
            voluntaryPutMoneyPreflop.Reset();
            raises.Reset();
            bets.Reset();
            cbets.Reset();
            stealRaises.Reset();
            foldsToAStealRaise.Reset();
            callsToAStealRaise.Reset();
            raisesToAStealRaise.Reset();

            folds.Reset();
            calls.Reset();
            sawStreet.Reset();

            callsToACBet = 0;
            foldsToACBet = 0;
            raisesToACBet = 0;

            opportunitiesToStealRaise = 0;
            opportunitiesToCBet = 0;

            checkRaises.Reset();
            checkFolds.Reset();
            checkCalls.Reset();

            wentToShowdown.Reset();
            wonAtShowdown.Reset();

            totalAllIns.Reset();

            foldsBlindToAPreflopRaise.Reset();
            callsBlindToAPreflopRaise.Reset();
            raisesBlindToAPreflopRaise.Reset();

            startingHandsWithPreflopCall.Clear();
            startingHandsWithPreflopRaise.Clear();

            PrepareStatisticsForNewRound();
        }


        /* Certain statistics are round specific (for example a person can only limp once per round)
         * This function should get called at the beginning of a new round */
        public override void PrepareStatisticsForNewRound()
        {
            base.PrepareStatisticsForNewRound();

            IsBigBlind = false;
            IsSmallBlind = false;
            IsButton = false;
            limps.AllowIncrement();
            voluntaryPutMoneyPreflop.AllowIncrement();
            raises.AllowIncrement();
            bets.AllowIncrement();
            cbets.AllowIncrement();
            stealRaises.AllowIncrement();
            foldsToAStealRaise.AllowIncrement();
            callsToAStealRaise.AllowIncrement();
            raisesToAStealRaise.AllowIncrement();

            folds.AllowIncrement();
            calls.AllowIncrement();

            checkRaises.AllowIncrement();
            checkFolds.AllowIncrement();
            checkCalls.AllowIncrement();

            checks.AllowIncrement();
            sawStreet.AllowIncrement();

            wentToShowdown.AllowIncrement();
            wonAtShowdown.AllowIncrement();

            totalAllIns.AllowIncrement();

            foldsBlindToAPreflopRaise.AllowIncrement();
            callsBlindToAPreflopRaise.AllowIncrement();
            raisesBlindToAPreflopRaise.AllowIncrement();
        }

        /* A mucked hand became available for this player */
        public override void MuckHandAvailable(Hand hand)
        {
            base.MuckHandAvailable(hand);

            HoldemHand holdemHand = (HoldemHand)hand;

            Debug.Print("Muck hand available called!");

            // Has this player raised preflop with this hand?
            if (raises[HoldemGamePhase.Preflop].WasIncremented)
            {
                Debug.Print("Added hand: " + hand.ToString());
                startingHandsWithPreflopRaise.Add(holdemHand);
            }

            // Has this player just limped or called a raise with this hand?
            else if (calls[HoldemGamePhase.Preflop].WasIncremented)
            {
                startingHandsWithPreflopCall.Add(holdemHand);
            }
        }

        /* Returns the limp statistics */
        public StatisticsData GetLimpStats()
        {
            float limpRatio = 0;

            if (totalHandsPlayed.Value == 0) limpRatio = 0;
            else limpRatio = (float)limps.Value / (float)totalHandsPlayed.Value;
            
            return new StatisticsPercentageData("Limp", limpRatio, "Preflop");
        }

        /* How many times has the player put money in preflop? */
        public StatisticsData GetVPFStats()
        {
            float vpfRatio = 0;

            if (totalHandsPlayed.Value == 0) vpfRatio = 0;
            else vpfRatio = (float)voluntaryPutMoneyPreflop.Value / (float)totalHandsPlayed.Value;
            return new StatisticsPercentageData("Voluntary Put $", vpfRatio, "Preflop");
        }

        /* How many times has the player made a continuation bet following a preflop raise? */
        public StatisticsData GetCBetStats()
        {
            if (opportunitiesToCBet == 0) return new StatisticsUnknownData("Continuation bets", "Flop");

            float cbetsRatio = (float)cbets.Value / (float)opportunitiesToCBet;
            return new StatisticsPercentageData("Continuation bets", cbetsRatio, "Flop");
        }

        /* How many times has a player folded to a continuation bet? */
        public StatisticsData GetFoldToACBetStats()
        {
            float actionsToACbet = foldsToACBet + raisesToACBet + callsToACBet;

            if (actionsToACbet == 0) return new StatisticsUnknownData("Folds to a continuation bet", "Flop");
            else
            {
                return new StatisticsPercentageData("Folds to a continuation bet",
                    (float)foldsToACBet / (float)(raisesToACBet + callsToACBet + foldsToACBet),
                    "Flop");
            }
        }

        /* How many times has a player won at showdown? */
        public StatisticsData GetWonAtShowdownStats()
        {
            if (wentToShowdown.Value == 0) return new StatisticsUnknownData("Won at Showdown", "Summary");
            else
            {
                float wonAtShowdownRatio = (float)wonAtShowdown.Value / (float)wentToShowdown.Value;
                return new StatisticsPercentageData("Won at Showdown", wonAtShowdownRatio, "Summary");
            }
        }

        /* How many times has the player pushed all-in? */
        public StatisticsData GetPushedAllInStats()
        {
            if (totalAllIns.Value == 0) return new StatisticsUnknownData("Pushed all-in", "Summary");
            else
            {
                float pushedAllInRatio = (float)totalAllIns.Value / (float)totalHandsPlayed.Value;
                return new StatisticsPercentageData("Pushed all-in", pushedAllInRatio, "Summary");
            }
        }

        /* How many times has the player raised? */
        public StatisticsData GetRaiseStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return new StatisticsUnknownData("Raises", category);
            else
            {
                float raiseRatio =  (float)raises[phase].Value / (float)sawStreet[phase].Value;
                return new StatisticsPercentageData("Raises", raiseRatio, category);
            }
        }

        /* How many times has the player bet? */
        public StatisticsData GetBetsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return new StatisticsUnknownData("Bets", category);
            else
            {
                float betsRatio = (float)bets[phase].Value / (float)sawStreet[phase].Value;
                return new StatisticsPercentageData("Bets", betsRatio, category);
            }
        }

        /* How many times has the player called? */
        public StatisticsData GetCallsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return new StatisticsUnknownData("Calls", category);
            else
            {
                float callsRatio = (float)calls[phase].Value / (float)sawStreet[phase].Value;
                return new StatisticsPercentageData("Calls", callsRatio, category);
            }
        }

        /* How many times has the player checked? */
        public StatisticsData GetChecksStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return new StatisticsUnknownData("Checks", category);
            else
            {
                float checksRatio = (float)checks[phase].Value / (float)sawStreet[phase].Value;
                return new StatisticsPercentageData("Checks", checksRatio, category);
            }
        }

        /* Calculate aggression frequency factor */
        public StatisticsData GetAggressionFrequencyStats()
        {
            int totRaises = SumStatistics(totalRaises);
            int totBets = SumStatistics(totalBets);
            int totChecks = SumStatistics(totalChecks);
            int totCalls = SumStatistics(totalCalls);
            //int totFolds = SumStatistics(totalFolds);
            int totalActions = totRaises + totBets + totChecks + totCalls;

            if (totalActions == 0) return new StatisticsUnknownData("Aggression Frequency", "Summary");
            else
            {
                float aggressionFrequency = ((float)totRaises + (float)totBets) / (float)totalActions;
                return new StatisticsNumberData("Aggression Frequency", aggressionFrequency, "Summary", 1);
            }
        }

        /* How many times has the player folded? */
        public StatisticsData GetFoldsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return new StatisticsUnknownData("Folds", category);
            else
            {
                float foldsRatio = (float)folds[phase].Value / (float)sawStreet[phase].Value;
                return new StatisticsPercentageData("Folds", foldsRatio, category);
            }
        }

        /* How many times has the player check raised? */
        public StatisticsData GetCheckRaiseStats(HoldemGamePhase phase, String category)
        {
            int checkActions = (int)checkRaises[phase].Value + (int)checkCalls[phase].Value + (int)checkFolds[phase].Value;

            if (checkActions == 0) return new StatisticsUnknownData("Check Raise", category);
            else
            {
                float checkRaiseRatio = (float)checkRaises[phase].Value / (float)checkActions;

                return new StatisticsPercentageData("Check Raise", checkRaiseRatio, category);
            }
        }

        /* How many times has the player check folded? */
        public StatisticsData GetCheckFoldStats(HoldemGamePhase phase, String category)
        {
            int checkActions = (int)checkRaises[phase].Value + (int)checkCalls[phase].Value + (int)checkFolds[phase].Value;

            if (checkActions == 0) return new StatisticsUnknownData("Check Fold", category);
            else
            {
                float checkFoldRatio = (float)checkFolds[phase].Value / (float)checkActions;

                return new StatisticsPercentageData("Check Fold", checkFoldRatio, category);
            }
        }


        /* How many times has the player steal raised? */
        public StatisticsData GetStealRaiseStats()
        {
            if (opportunitiesToStealRaise == 0) return new StatisticsUnknownData("Steal Raises", "Preflop");
            else
            {
                float stealRaiseRatio = (float)stealRaises.Value / (float)opportunitiesToStealRaise;

                return new StatisticsPercentageData("Steal Raises", stealRaiseRatio, "Preflop");
            }
        }

        /* How many times has the player folded the blind to a steal raise? */
        public StatisticsData GetFoldsToAStealRaiseStats(BlindType blindType)
        {
            int actionsToAStealRaise = (int)callsToAStealRaise[blindType].Value + (int)raisesToAStealRaise[blindType].Value + (int)foldsToAStealRaise[blindType].Value;
            String statDescription = String.Format("Fold {0} to a Steal Raise", 
                            (blindType == BlindType.BigBlind) ? "Big Blind" : "Small Blind");


            if (actionsToAStealRaise == 0) return new StatisticsUnknownData(statDescription, "Preflop");
            else
            {
                float foldToAStealRaiseRatio = (float)foldsToAStealRaise[blindType].Value / (float)actionsToAStealRaise;

                return new StatisticsPercentageData(statDescription, foldToAStealRaiseRatio, "Preflop");
            }
        }

        /* How many times has the player folded the blind to a raise (but not a reraise)? */
        public StatisticsData GetFoldsBlindToPreflopRaise(BlindType blindType)
        {
            int actionsToPreflopRaise = (int)callsBlindToAPreflopRaise[blindType].Value + (int)raisesBlindToAPreflopRaise[blindType].Value + (int)foldsBlindToAPreflopRaise[blindType].Value;
            String statDescription = String.Format("Fold {0} to a Raise",
                            (blindType == BlindType.BigBlind) ? "Big Blind" : "Small Blind");


            if (actionsToPreflopRaise == 0) return new StatisticsUnknownData(statDescription, "Preflop");
            else
            {
                float foldBlindToPreflopRaiseRatio = (float)foldsBlindToAPreflopRaise[blindType].Value / (float)actionsToPreflopRaise;

                return new StatisticsPercentageData(statDescription, foldBlindToPreflopRaiseRatio, "Preflop");
            }
        }

        public bool WentToShowdownThisRound()
        {
            return wentToShowdown.WasIncremented;
        }

        /* Helper function to find out whether a player never folded */
        public bool NeverFoldedThisRound()
        {
            bool foldedAStreet = folds[HoldemGamePhase.Preflop].WasIncremented || folds[HoldemGamePhase.Flop].WasIncremented
                                || folds[HoldemGamePhase.Turn].WasIncremented || folds[HoldemGamePhase.River].WasIncremented;

            return (totalHandsPlayed.WasIncremented && !foldedAStreet);
        }

        /* Get hands that the player called with preflop */
        private StatisticsData GetStartingHandsWithPreflopCallStats()
        {
            if (startingHandsWithPreflopCall.Count > 0)
            {
                String result = String.Empty;
                foreach (HoldemHand hand in startingHandsWithPreflopCall)
                {
                    result += hand.ToString() + ", ";
                }
                
                // Eliminate last comma
                result = result.Substring(0, result.Length - 2);
                return new StatisticsDescriptiveData("Call/limp starting hands", "Summary", result);
            }
            else return new StatisticsUnknownData("Call/limp starting hands", "Summary");
        }

        /* Get hands that the player raised with preflop */
        private StatisticsData GetStartingHandsWithPreflopRaiseStats()
        {
            if (startingHandsWithPreflopRaise.Count > 0)
            {
                String result = String.Empty;
                foreach (HoldemHand hand in startingHandsWithPreflopRaise)
                {
                    result += hand.ToString() + ", ";
                }

                // Eliminate last comma
                result = result.Substring(0, result.Length - 2);
                return new StatisticsDescriptiveData("Raise starting hands", "Summary", result);
            }
            else return new StatisticsUnknownData("Raise starting hands", "Summary");
        }

        /* Get style of play */
        public StatisticsData GetStyle()
        {
            /* Is this player tight, semi-loose or loose?
             * Depends on his VPF ratio */
            float vpf = GetVPFStats().GetFloat(2);

            /* Is this player aggressive or passive? 
             * Depends on the aggression frequency */
            float aggressionFrequency = GetAggressionFrequencyStats().GetFloat(2);

            String tightness = String.Empty;
            String aggressiveness = String.Empty;

            if (vpf < 0.15) tightness = "Tight";
            else if (vpf >= 0.15 && vpf <= 0.2) tightness = "Semi-loose";
            else if (vpf > 0.2) tightness = "Loose";

            if (aggressionFrequency > 0.2) aggressiveness = "Aggressive";
            else aggressiveness = "Passive";

            return new StatisticsDescriptiveData("Style","Summary",String.Format("{0} {1}",tightness, aggressiveness));
        }

        /* How many times has the player check called? */
        public StatisticsData GetCheckCallStats(HoldemGamePhase phase, String category)
        {
            int checkActions = (int)checkRaises[phase].Value + (int)checkCalls[phase].Value + (int)checkFolds[phase].Value;

            if (checkActions == 0) return new StatisticsUnknownData("Check Call", category);
            else
            {
                float checkCallRatio = (float)checkCalls[phase].Value / (float)checkActions;

                return new StatisticsPercentageData("Check Call", checkCallRatio, category);
            }
        }
        
        /* Has limped */
        public void CheckForLimp()
        {
            /* If he's not the small or big blind, this is also a limp */
            if (!IsSmallBlind && !IsBigBlind)
            {
                limps.Increment();
            }
        }

        /* Fold after a cbet */
        public void IncrementFoldToACBet()
        {
            foldsToACBet += 1;
        }

        /* Calls and raises after a cbet */
        public void IncrementCallToACBet()
        {
            callsToACBet += 1;
        }

        public void IncrementRaiseToACBet()
        {
            raisesToACBet += 1;
        }

        public void IncrementStealRaises()
        {
            stealRaises.Increment();
        }

        public void IncrementFoldsToAStealRaise()
        {
            foldsToAStealRaise[blindType].Increment();
        }

        public void IncrementCallsToAStealRaise()
        {
            callsToAStealRaise[blindType].Increment();
        }

        public void IncrementRaisesToAStealRaise()
        {
            raisesToAStealRaise[blindType].Increment();
        }

        public void IncrementOpportunitiesToStealRaise()
        {
            opportunitiesToStealRaise += 1;
        }

        /* Increment blind folds/calls/raises to a raise preflop */
        public void IncrementFoldsBlindToAPreflopRaise()
        {
            foldsBlindToAPreflopRaise[blindType].Increment();
        }

        public void IncrementCallsBlindToAPreflopRaise()
        {
            callsBlindToAPreflopRaise[blindType].Increment();
        }

        public void IncrementRaisesBlindToAPreflopRaise()
        {
            raisesBlindToAPreflopRaise[blindType].Increment();
        }

        /* This player has raised, increment the stats */
        public void HasRaised(HoldemGamePhase gamePhase)
        {
            /* We're only interested in the first action */
            if (!sawStreet[gamePhase].WasIncremented)
            {
                raises[gamePhase].Increment();
            }

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                voluntaryPutMoneyPreflop.Increment();
            }

            // Check raise?
            if (checks[gamePhase].WasIncremented)
            {
                checkRaises[gamePhase].Increment();
            }    

            IncrementStatistics(totalRaises, gamePhase);
            sawStreet[gamePhase].Increment();
        }

        /* Has bet */
        public void HasBet(HoldemGamePhase gamePhase)
        {
            /* We're only interested in the first action */
            if (!sawStreet[gamePhase].WasIncremented)
            {
                bets[gamePhase].Increment();
            }

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                voluntaryPutMoneyPreflop.Increment();
            }

            IncrementStatistics(totalBets, gamePhase);
            sawStreet[gamePhase].Increment();
        }

        /* Has checked */
        public void HasChecked(HoldemGamePhase gamePhase)
        {
            /* We're only interested in the first action */
            if (!sawStreet[gamePhase].WasIncremented)
            {
                checks[gamePhase].Increment();
            }

            IncrementStatistics(totalChecks, gamePhase);
            sawStreet[gamePhase].Increment();
        }

        /* Has called */
        public void HasCalled(HoldemGamePhase gamePhase)
        {
            /* We're only interested in the first action */
            if (!sawStreet[gamePhase].WasIncremented)
            {
                calls[gamePhase].Increment();
            }

            if (gamePhase == HoldemGamePhase.Preflop)
            {
                voluntaryPutMoneyPreflop.Increment();
            }
            

            // Check call?
            if (checks[gamePhase].WasIncremented)
            {
                checkCalls[gamePhase].Increment();
            }  

            IncrementStatistics(totalCalls, gamePhase);
            sawStreet[gamePhase].Increment();
        }

        /* Folded */
        public void HasFolded(HoldemGamePhase gamePhase)
        {
            /* We're only interested in the first action */
            if (!sawStreet[gamePhase].WasIncremented)
            {
                folds[gamePhase].Increment();
            }

            // Check fold?
            if (checks[gamePhase].WasIncremented)
            {
                checkFolds[gamePhase].Increment();
            }  
            
            IncrementStatistics(totalFolds, gamePhase);
            sawStreet[gamePhase].Increment();
        }

        /* Pushed all-in */
        public void HasPushedAllIn(HoldemGamePhase gamePhase)
        {
            totalAllIns.Increment();
        }


        /* Increments the opportunitiesToCBet counter. If realCBet is true, the 
         * cbets are also incremented */
        public void IncrementOpportunitiesToCBet(bool realCBet)
        {
            if (realCBet) cbets.Increment();
            opportunitiesToCBet++;
        }

        public void IncrementWentToShowdown()
        {
            wentToShowdown.Increment();
        }

        public void IncrementWonAtShowdown()
        {
            wonAtShowdown.Increment();
        }

        /* Helper function to increment the value in one of the hash tables (calls, raises, folds, etc.) */
        private void IncrementStatistics(Hashtable table, HoldemGamePhase gamePhase)
        {
            if (table != null) table[gamePhase] = (int)table[gamePhase] + 1;
        }


        public bool HasPreflopRaisedThisRound()
        {
            return raises[HoldemGamePhase.Preflop].WasIncremented;
        }

        /* Is he big or small blind? */
        public bool IsBlind()
        {
            return IsBigBlind || IsSmallBlind;
        }

        /* Returns the sum of all streets */
        private int SumStatistics(Hashtable table)
        {
            return ((int)table[HoldemGamePhase.Preflop] +
                (int)table[HoldemGamePhase.Flop] +
                (int)table[HoldemGamePhase.Turn] +
                (int)table[HoldemGamePhase.River]);
        }

        /* Reset the stats for a particular hash table set */
        private void ResetStatistics(Hashtable table)
        {
            table[HoldemGamePhase.Preflop] = 0;
            table[HoldemGamePhase.Flop] = 0;
            table[HoldemGamePhase.Turn] = 0;
            table[HoldemGamePhase.River] = 0;
        }

        protected override int GetCategoryOrder(string category)
        {
            switch (category)
            {
                case "Summary": return 0;
                case "Preflop": return 1;
                case "Flop": return 2;
                case "Turn": return 3;
                case "River": return 4;
                default: return 5;
            }
        }

        /* Returns the statistics of the player */
        public override PlayerStatistics GetStatistics()
        {
            PlayerStatistics result =  base.GetStatistics();

            result.Set(GetVPFStats());
            result.Set(GetLimpStats());

            result.Set(GetStealRaiseStats());
            result.Set(GetFoldsToAStealRaiseStats(BlindType.SmallBlind));
            result.Set(GetFoldsToAStealRaiseStats(BlindType.BigBlind));

            result.Set(GetFoldsBlindToPreflopRaise(BlindType.SmallBlind));
            result.Set(GetFoldsBlindToPreflopRaise(BlindType.BigBlind));            

            result.Set(GetWonAtShowdownStats());
            
            result.Set(GetCBetStats());
            result.Set(GetFoldToACBetStats());

            result.Set(GetStartingHandsWithPreflopRaiseStats());
            result.Set(GetStartingHandsWithPreflopCallStats());

            result.Set(GetRaiseStats(HoldemGamePhase.Preflop, "Preflop"));
            result.Set(GetRaiseStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetRaiseStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetRaiseStats(HoldemGamePhase.River, "River"));
            
            result.Set(GetBetsStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetBetsStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetBetsStats(HoldemGamePhase.River, "River"));

            result.Set(GetFoldsStats(HoldemGamePhase.Preflop, "Preflop"));
            result.Set(GetFoldsStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetFoldsStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetFoldsStats(HoldemGamePhase.River, "River"));

            result.Set(GetCallsStats(HoldemGamePhase.Preflop, "Preflop"));
            result.Set(GetCallsStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetCallsStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetCallsStats(HoldemGamePhase.River, "River"));

            result.Set(GetChecksStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetChecksStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetChecksStats(HoldemGamePhase.River, "River"));

            result.Set(GetCheckRaiseStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetCheckRaiseStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetCheckRaiseStats(HoldemGamePhase.River, "River"));

            result.Set(GetCheckFoldStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetCheckFoldStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetCheckFoldStats(HoldemGamePhase.River, "River"));

            result.Set(GetCheckCallStats(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetCheckCallStats(HoldemGamePhase.Turn, "Turn"));
            result.Set(GetCheckCallStats(HoldemGamePhase.River, "River"));

            result.Set(GetStyle());
            result.Set(GetAggressionFrequencyStats());

            result.Set(GetPushedAllInStats());

            // Calculate a few averages

            // Overall raises % average across all streets
            StatisticsData raisesPreflop = result.Get("Raises", "Preflop");
            StatisticsData raisesAverage = raisesPreflop.Average("Raises", "Summary", 0, result.Get("Raises", "Flop"),
                                                                                     result.Get("Raises", "Turn"),
                                                                                     result.Get("Raises", "River"));
            result.Set(raisesAverage);

            // Overall bets % average across all streets
            StatisticsData betsFlop = result.Get("Bets", "Flop");
            StatisticsData betsAverage = betsFlop.Average("Bets", "Summary", 0, result.Get("Bets", "Turn"),
                                                                                     result.Get("Bets", "River"));
            result.Set(betsAverage);

            // Overall folds % average across all streets
            StatisticsData foldsPreflop = result.Get("Folds", "Preflop");
            StatisticsData foldsAverage = foldsPreflop.Average("Folds", "Summary", 0, result.Get("Folds", "Flop"),
                                                                                     result.Get("Folds", "Turn"),
                                                                                     result.Get("Folds", "River"));
            result.Set(foldsAverage);

            // Overall calls % average across all streets
            StatisticsData callsPreflop = result.Get("Calls", "Preflop");
            StatisticsData callsAverage = callsPreflop.Average("Calls", "Summary", 0, result.Get("Calls", "Flop"),
                                                                                     result.Get("Calls", "Turn"),
                                                                                     result.Get("Calls", "River"));
            result.Set(callsAverage);

            // Overall check-calls % across all streets
            StatisticsData checkCallsFlop = result.Get("Check Call", "Flop");
            StatisticsData checkCallsAverage = checkCallsFlop.Average("Check Call", "Summary", 0, result.Get("Check Call", "Turn"),
                                                                                     result.Get("Check Call", "River"));
            result.Set(checkCallsAverage);

            // Overall check-raises % across all streets
            StatisticsData checkRaisesFlop = result.Get("Check Raise", "Flop");
            StatisticsData checkRaisesAverage = checkRaisesFlop.Average("Check Raise", "Summary", 0, result.Get("Check Raise", "Turn"),
                                                                                     result.Get("Check Raise", "River"));
            result.Set(checkRaisesAverage);

            // Overall check-fold % across all streets
            StatisticsData checkFoldsFlop = result.Get("Check Fold", "Flop");
            StatisticsData checkFoldsAverage = checkFoldsFlop.Average("Check Fold", "Summary", 0, result.Get("Check Fold", "Turn"),
                                                                                     result.Get("Check Fold", "River"));
            result.Set(checkFoldsAverage);



            return result;
        }
    }
}
