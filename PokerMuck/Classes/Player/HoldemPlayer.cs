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
        private MultipleValueCounter limpsDetails;

        /* VPF */
        private ValueCounter voluntaryPutMoneyPreflop;

        /* Raises */
        private MultipleValueCounter raises;
        private MultipleValueCounter raisesRatings;

        /* Bets */
        private MultipleValueCounter bets;
        private MultipleValueCounter betsRatings;

        /* Folds */
        private MultipleValueCounter folds;

        /* Calls */
        private MultipleValueCounter calls;
        private MultipleValueCounter callsRatings;

        /* C Bets */ 
        private int opportunitiesToCBet;

        /* Draws */
        private MultipleValueCounter draws;

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
        private MultipleValueCounter checksRatings;

        /* Check-raise */
        private MultipleValueCounter checkRaises;
        private MultipleValueCounter checksRaisesRatings;

        /* Check-fold */
        private MultipleValueCounter checkFolds;

        /* Check-call */
        private MultipleValueCounter checkCalls;
        private MultipleValueCounter checksCallsRatings;

        /* How many times has the player pushed all-in? */
        private MultipleValueCounter pushedAllIn; // Also keeps track of the street
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

        /* Starting hands when raise all-in preflop */
        private List<HoldemHand> startingHandsWithPreflopAllIn;

        /* Keep a reference for the final board */
        private HoldemBoard lastFinalBoard = null;

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


        public override PokerGame GameType
        {
            get
            {
                return PokerGame.Holdem;
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
            this.limpsDetails = (MultipleValueCounter)other.limpsDetails.Clone();
            this.voluntaryPutMoneyPreflop = (ValueCounter)other.limps.Clone();
            this.raises = (MultipleValueCounter)other.raises.Clone();
            this.raisesRatings = (MultipleValueCounter)other.raisesRatings.Clone();
            this.bets = (MultipleValueCounter)other.bets.Clone();
            this.betsRatings = (MultipleValueCounter)other.betsRatings.Clone();
            this.calls = (MultipleValueCounter)other.calls.Clone();
            this.callsRatings = (MultipleValueCounter)other.callsRatings.Clone();
            this.folds = (MultipleValueCounter)other.folds.Clone();
            this.opportunitiesToCBet = other.opportunitiesToCBet;
            this.cbets = (ValueCounter)other.cbets.Clone();
            this.foldsToACBet = other.foldsToACBet;
            this.callsToACBet = other.callsToACBet;
            this.raisesToACBet = other.raisesToACBet;
            this.checks = (MultipleValueCounter)other.checks.Clone();
            this.checksRatings = (MultipleValueCounter)other.checksRatings.Clone(); 
            this.checkRaises = (MultipleValueCounter)other.checkRaises.Clone();
            this.checksRaisesRatings = (MultipleValueCounter)other.checksRaisesRatings.Clone();
            this.checkFolds = (MultipleValueCounter)other.checkFolds.Clone();
            this.checkCalls = (MultipleValueCounter)other.checkCalls.Clone();
            this.checksCallsRatings = (MultipleValueCounter)other.checksCallsRatings.Clone();
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
            this.pushedAllIn = (MultipleValueCounter)other.pushedAllIn.Clone();
            this.totalAllIns = (ValueCounter)other.totalAllIns.Clone();
            this.foldsBlindToAPreflopRaise = (MultipleValueCounter)other.foldsBlindToAPreflopRaise.Clone();
            this.callsBlindToAPreflopRaise = (MultipleValueCounter)other.callsBlindToAPreflopRaise.Clone();
            this.raisesBlindToAPreflopRaise = (MultipleValueCounter)other.raisesBlindToAPreflopRaise.Clone();
            this.draws = (MultipleValueCounter)other.draws.Clone();

            // Deep copy lists
            startingHandsWithPreflopCall = new List<HoldemHand>();
            foreach (HoldemHand h in other.startingHandsWithPreflopCall) startingHandsWithPreflopCall.Add((HoldemHand)h.Clone());
            
            startingHandsWithPreflopRaise = new List<HoldemHand>();
            foreach (HoldemHand h in other.startingHandsWithPreflopRaise) startingHandsWithPreflopRaise.Add((HoldemHand)h.Clone());

            startingHandsWithPreflopAllIn = new List<HoldemHand>();
            foreach (HoldemHand h in other.startingHandsWithPreflopAllIn) startingHandsWithPreflopAllIn.Add((HoldemHand)h.Clone());
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
            limpsDetails = new MultipleValueCounter(HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster);
            
            voluntaryPutMoneyPreflop = new ValueCounter();

            raises = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            raisesRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });
            checkRaises = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checksRaisesRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });
            checkFolds = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checkCalls = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checksCallsRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });
            checks = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            checksRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });
            sawStreet = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River, HoldemGamePhase.Showdown);
            bets = new MultipleValueCounter(HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            betsRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });
            folds = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            calls = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            callsRatings = new MultipleValueCounter(new object[] { HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River },
                new object[] { HoldemHand.Rating.Nothing, HoldemHand.Rating.Weak, HoldemHand.Rating.Mediocre, HoldemHand.Rating.Strong, HoldemHand.Rating.Monster });

            draws = new MultipleValueCounter(new object[] { HoldemGamePhase.Flop, HoldemGamePhase.Turn },
                new object[] { HoldemPlayerAction.Bet, HoldemPlayerAction.Call, HoldemPlayerAction.Check, HoldemPlayerAction.CheckCall, HoldemPlayerAction.CheckFold, HoldemPlayerAction.CheckRaise, HoldemPlayerAction.Fold, HoldemPlayerAction.Raise });
            
            cbets = new ValueCounter();
            stealRaises = new ValueCounter();
            foldsToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            callsToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            raisesToAStealRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);

            wentToShowdown = new ValueCounter();
            wonAtShowdown = new ValueCounter();

            pushedAllIn = new MultipleValueCounter(HoldemGamePhase.Preflop, HoldemGamePhase.Flop, HoldemGamePhase.Turn, HoldemGamePhase.River);
            totalAllIns = new ValueCounter();

            foldsBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            callsBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);
            raisesBlindToAPreflopRaise = new MultipleValueCounter(BlindType.BigBlind, BlindType.SmallBlind);

            startingHandsWithPreflopCall = new List<HoldemHand>();
            startingHandsWithPreflopRaise = new List<HoldemHand>();
            startingHandsWithPreflopAllIn = new List<HoldemHand>();


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
            limpsDetails.Reset();
            checks.Reset();
            checksRatings.Reset();
            voluntaryPutMoneyPreflop.Reset();
            raises.Reset();
            raisesRatings.Reset();
            bets.Reset();
            betsRatings.Reset();
            cbets.Reset();
            stealRaises.Reset();
            foldsToAStealRaise.Reset();
            callsToAStealRaise.Reset();
            raisesToAStealRaise.Reset();

            folds.Reset();
            calls.Reset();
            callsRatings.Reset();
            sawStreet.Reset();

            callsToACBet = 0;
            foldsToACBet = 0;
            raisesToACBet = 0;

            opportunitiesToStealRaise = 0;
            opportunitiesToCBet = 0;

            draws.Reset();

            checkRaises.Reset();
            checksRaisesRatings.Reset();
            checkFolds.Reset();
            checkCalls.Reset();
            checksCallsRatings.Reset();

            wentToShowdown.Reset();
            wonAtShowdown.Reset();

            pushedAllIn.Reset();
            totalAllIns.Reset();

            foldsBlindToAPreflopRaise.Reset();
            callsBlindToAPreflopRaise.Reset();
            raisesBlindToAPreflopRaise.Reset();

            startingHandsWithPreflopCall.Clear();
            startingHandsWithPreflopRaise.Clear();
            startingHandsWithPreflopAllIn.Clear();

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
            limpsDetails.AllowIncrement();
            voluntaryPutMoneyPreflop.AllowIncrement();
            raises.AllowIncrement();
            raisesRatings.AllowIncrement();
            bets.AllowIncrement();
            betsRatings.AllowIncrement();
            cbets.AllowIncrement();
            stealRaises.AllowIncrement();
            foldsToAStealRaise.AllowIncrement();
            callsToAStealRaise.AllowIncrement();
            raisesToAStealRaise.AllowIncrement();

            folds.AllowIncrement();
            calls.AllowIncrement();
            callsRatings.AllowIncrement();

            checkRaises.AllowIncrement();
            checksRaisesRatings.AllowIncrement();
            checkFolds.AllowIncrement();
            checkCalls.AllowIncrement();
            checksCallsRatings.AllowIncrement();

            checks.AllowIncrement();
            checksRatings.AllowIncrement();

            sawStreet.AllowIncrement();

            wentToShowdown.AllowIncrement();
            wonAtShowdown.AllowIncrement();

            pushedAllIn.AllowIncrement();
            totalAllIns.AllowIncrement();

            draws.AllowIncrement();

            foldsBlindToAPreflopRaise.AllowIncrement();
            callsBlindToAPreflopRaise.AllowIncrement();
            raisesBlindToAPreflopRaise.AllowIncrement();

            lastFinalBoard = null;
        }

        public override void CalculateEndOfRoundStatistics()
        {
            base.CalculateEndOfRoundStatistics();

            if (lastFinalBoard != null && HasShowedThisRound)
            {
                // Sweet, we have a board and we have a hand
                Trace.WriteLine("Board and hand available, calculating hand strength for " + Name);

                for (HoldemGamePhase phase = HoldemGamePhase.Preflop; phase <= HoldemGamePhase.River; phase++)
                {
                    HoldemHand.Classification classification = ((HoldemHand)MuckedHand).GetHandClassification(phase, lastFinalBoard);
                    HoldemHand.Rating rating = classification.GetRating();

                    // Increment ratings 

                    if (raises[phase].WasIncremented)
                    {
                        raisesRatings[phase, rating].Increment();
                    }

                    // There are no bets preflop (only calls and raises)
                    if (phase != HoldemGamePhase.Preflop && bets[phase].WasIncremented)
                    {
                        betsRatings[phase, rating].Increment();
                    }

                    if (calls[phase].WasIncremented)
                    {
                        callsRatings[phase, rating].Increment();
                    }

                    if (checks[phase].WasIncremented)
                    {
                        checksRatings[phase, rating].Increment();
                    }

                    if (checkRaises[phase].WasIncremented)
                    {
                        checksRaisesRatings[phase, rating].Increment();
                    }

                    if (checkCalls[phase].WasIncremented)
                    {
                        checksCallsRatings[phase, rating].Increment();
                    }

                    // Increment Draws (only on flop and turn)
                    if (phase == HoldemGamePhase.Flop || phase == HoldemGamePhase.Turn){
                        HoldemHand.ClassificationPostflop classificationPostFlop = (HoldemHand.ClassificationPostflop)classification;

                        if (classificationPostFlop.HasADraw())
                        {
                            HoldemPlayerAction action = HoldemPlayerAction.None;

                            if (calls[phase].WasIncremented) action = HoldemPlayerAction.Call;
                            else if (raises[phase].WasIncremented) action = HoldemPlayerAction.Raise;
                            else if (bets[phase].WasIncremented) action = HoldemPlayerAction.Bet;
                            else if (folds[phase].WasIncremented) action = HoldemPlayerAction.Fold;
                            else if (checks[phase].WasIncremented) action = HoldemPlayerAction.Check;

                            if (checkRaises[phase].WasIncremented) action = HoldemPlayerAction.CheckRaise;
                            else if (checkCalls[phase].WasIncremented) action = HoldemPlayerAction.CheckCall;
                            else if (checkFolds[phase].WasIncremented) action = HoldemPlayerAction.CheckFold;

                            if (action != HoldemPlayerAction.None) draws[phase, action].Increment();
                        }
                    }
                }

                if (limps.WasIncremented)
                {
                    HoldemHand.Rating rating = ((HoldemHand)MuckedHand).GetHandClassification(HoldemGamePhase.Preflop, lastFinalBoard).GetRating();
                    limpsDetails[rating].Increment();
                }
            }
        }


        /* The final board became available */
        public void BoardAvailable(HoldemBoard board)
        {
            lastFinalBoard = board;
        }

        /* A mucked hand became available for this player */
        public override void MuckHandAvailable(Hand hand)
        {
            bool duplicate = hand.Equals(MuckedHand);
            base.MuckHandAvailable(hand);

            // Make sure we're not handling a duplicate... the parser might send multiple mucked hands
            if (!duplicate)
            {
                HoldemHand holdemHand = (HoldemHand)hand;

                Trace.WriteLine("Muck hand available called!");

                // Has this player raised preflop with this hand?
                if (raises[HoldemGamePhase.Preflop].WasIncremented)
                {
                    startingHandsWithPreflopRaise.Add(holdemHand);

                    // Has this player pushed all in preflop with this hand?
                    if (pushedAllIn[HoldemGamePhase.Preflop].WasIncremented)
                    {
                        startingHandsWithPreflopAllIn.Add(holdemHand);
                    }
                }

                // Has this player just limped or called a raise with this hand?
                else if (calls[HoldemGamePhase.Preflop].WasIncremented)
                {
                    startingHandsWithPreflopCall.Add(holdemHand);
                }
            }
        }

        /* Returns the limp statistics */
        public Statistic GetLimpStats()
        {
            float limpRatio = 0;

            if (totalHandsPlayed.Value == 0) limpRatio = 0;
            else limpRatio = (float)limps.Value / (float)totalHandsPlayed.Value;

            Statistic ret = new Statistic(new StatisticsPercentageData("Limp", limpRatio), "Preflop");

            // Advanced statistics
            float sum = (float)limpsDetails.GetSumOfAllValues();
            foreach (HoldemHand.Rating rating in Enum.GetValues(typeof(HoldemHand.Rating)))
            {
                if (sum == 0) ret.AddSubStatistic(new StatisticsUnknownData(rating.ToString()));
                else
                {
                    float ratio = (float)limpsDetails[rating].Value / sum;
                    ret.AddSubStatistic(new StatisticsPercentageData(rating.ToString(), ratio));
                }
            }

            return ret;
        }

        /* How many times has the player put money in preflop? */
        public Statistic GetVPFStats()
        {
            float vpfRatio = 0;

            if (totalHandsPlayed.Value == 0) vpfRatio = 0;
            else vpfRatio = (float)voluntaryPutMoneyPreflop.Value / (float)totalHandsPlayed.Value;

            return new Statistic(new StatisticsPercentageData("Voluntary Put $", vpfRatio), "Preflop");
        }

        /* How many times has the player made a continuation bet following a preflop raise? */
        public Statistic GetCBetStats()
        {
            if (opportunitiesToCBet == 0) return Statistic.CreateUnknown("Continuation bets", "Flop");

            float cbetsRatio = (float)cbets.Value / (float)opportunitiesToCBet;
            return new Statistic(new StatisticsPercentageData("Continuation bets", cbetsRatio), "Flop");
        }

        /* How many times has a player folded to a continuation bet? */
        public Statistic GetFoldToACBetStats()
        {
            float actionsToACbet = foldsToACBet + raisesToACBet + callsToACBet;

            if (actionsToACbet == 0) return Statistic.CreateUnknown("Folds to a continuation bet", "Flop");
            else
            {
                return new Statistic(new StatisticsPercentageData("Folds to a continuation bet",
                    (float)foldsToACBet / (float)(raisesToACBet + callsToACBet + foldsToACBet)),
                    "Flop");
            }
        }

        /* How many times has a player won at showdown? */
        public Statistic GetWonAtShowdownStats()
        {
            if (wentToShowdown.Value == 0) return Statistic.CreateUnknown("Won at Showdown", "Summary");
            else
            {
                float wonAtShowdownRatio = (float)wonAtShowdown.Value / (float)wentToShowdown.Value;
                return new Statistic(new StatisticsPercentageData("Won at Showdown", wonAtShowdownRatio), "Summary");
            }
        }

        /* How many times has the player pushed all-in? */
        public Statistic GetPushedAllInStats()
        {
            if (totalAllIns.Value == 0) return Statistic.CreateUnknown("Pushed all-in", "Summary");
            else
            {
                float pushedAllInRatio = (float)totalAllIns.Value / (float)totalHandsPlayed.Value;
                return new Statistic(new StatisticsPercentageData("Pushed all-in", pushedAllInRatio), "Summary");
            }
        }

        private Statistic GetDrawStatistics(HoldemGamePhase phase, String category)
        {
            List<StatisticsData> subData = new List<StatisticsData>(10);
            float sum = (float)draws.GetSumOfAllValuesIn(phase);
            float max = 0;
            HoldemPlayerAction mostCommonAction = (HoldemPlayerAction)(-1);

            for (HoldemPlayerAction action = HoldemPlayerAction.Call; action <= HoldemPlayerAction.CheckFold; action++)
            {
                if (sum == 0) subData.Add(new StatisticsUnknownData(action.ToString()));
                else
                {
                    float value = draws[phase, action].Value / sum;

                    // Keep track of the max value as to put a quick description on the statistic indicating
                    // which is the most common action taken by the player
                    // Note: in case of equality, the first element is used
                    if (value > max){
                        max = value;
                        mostCommonAction = action;
                    }
                    subData.Add(new StatisticsPercentageData(action.ToString(), value));
                }
            }

            String description = "?";
            if (mostCommonAction != (HoldemPlayerAction)(-1)) description = "Mostly " + mostCommonAction.ToString().ToLower();

            Statistic ret = new Statistic(new StatisticsDescriptiveData("On a straight/flush draw", description), category, subData);
            return ret;
        }

        private void AppendActionsSubstatistics(HoldemGamePhase phase, Statistic statistic, MultipleValueCounter ratings)
        {
            float sum = (float)ratings.GetSumOfAllValuesIn(phase);

            foreach (HoldemHand.Rating rating in Enum.GetValues(typeof(HoldemHand.Rating)))
            {
                if (sum == 0) statistic.AddSubStatistic(new StatisticsUnknownData(rating.ToString()));
                else
                {
                    float ratio = (float)ratings[phase, rating].Value / sum;
                    statistic.AddSubStatistic(new StatisticsPercentageData(rating.ToString(), ratio));
                }
            }
        }


        private Statistic CreateUnknownActionStatistic(String name, String category)
        {
            Statistic ret = Statistic.CreateUnknown(name, category);
            foreach (HoldemHand.Rating rating in Enum.GetValues(typeof(HoldemHand.Rating)))
            {
                ret.AddSubStatistic(new StatisticsUnknownData(rating.ToString()));
            }
            return ret;
        }

        /* How many times has the player raised? */
        public Statistic GetRaiseStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return CreateUnknownActionStatistic("Raises", category);
            else
            {
                float raiseRatio = (float)raises[phase].Value / (float)sawStreet[phase].Value;
                Statistic ret = new Statistic(new StatisticsPercentageData("Raises", raiseRatio), category);

                AppendActionsSubstatistics(phase, ret, raisesRatings);

                return ret;
            }
        }

        /* How many times has the player bet? */
        public Statistic GetBetsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return CreateUnknownActionStatistic("Bets", category);
            else
            {
                float betsRatio = (float)bets[phase].Value / (float)sawStreet[phase].Value;
                Statistic ret = new Statistic(new StatisticsPercentageData("Bets", betsRatio), category);

                AppendActionsSubstatistics(phase, ret, betsRatings);

                return ret;
            }
        }

        /* How many times has the player called? */
        public Statistic GetCallsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return CreateUnknownActionStatistic("Calls", category);
            else
            {
                float callsRatio = (float)calls[phase].Value / (float)sawStreet[phase].Value;
                Statistic ret = new Statistic(new StatisticsPercentageData("Calls", callsRatio), category);

                AppendActionsSubstatistics(phase, ret, callsRatings);

                return ret;
            }
        }

        /* How many times has the player checked? */
        public Statistic GetChecksStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return CreateUnknownActionStatistic("Checks", category);
            else
            {
                float checksRatio = (float)checks[phase].Value / (float)sawStreet[phase].Value;
                Statistic ret = new Statistic(new StatisticsPercentageData("Checks", checksRatio), category);

                AppendActionsSubstatistics(phase, ret, checksRatings);

                return ret;
            }
        }

        /* Calculate aggression frequency factor */
        public Statistic GetAggressionFrequencyStats()
        {
            int totRaises = SumStatistics(totalRaises);
            int totBets = SumStatistics(totalBets);
            int totChecks = SumStatistics(totalChecks);
            int totCalls = SumStatistics(totalCalls);
            //int totFolds = SumStatistics(totalFolds);
            int totalActions = totRaises + totBets + totChecks + totCalls;

            if (totalActions == 0) return Statistic.CreateUnknown("Aggression Frequency", "Summary");
            else
            {
                float aggressionFrequency = ((float)totRaises + (float)totBets) / (float)totalActions;
                return new Statistic(new StatisticsNumberData("Aggression Frequency", aggressionFrequency, 1), "Summary");
            }
        }

        /* How many times has the player folded? */
        public Statistic GetFoldsStats(HoldemGamePhase phase, String category)
        {
            if (sawStreet[phase].Value == 0) return Statistic.CreateUnknown("Folds", category);
            else
            {
                float foldsRatio = (float)folds[phase].Value / (float)sawStreet[phase].Value;
                return new Statistic(new StatisticsPercentageData("Folds", foldsRatio), category);
            }
        }

        /* @param ratings the multiple value counter object containing the ratings how a check action, can be null if no ratings are available */
        private Statistic GetCheckActionStats(HoldemGamePhase phase, String name, String category, MultipleValueCounter values, MultipleValueCounter ratings)
        {
            int checkActions = (int)checkRaises[phase].Value + (int)checkCalls[phase].Value + (int)checkFolds[phase].Value;

            if (checkActions == 0)
            {
                if (ratings != null) return CreateUnknownActionStatistic(name, category);
                else return Statistic.CreateUnknown(name, category);
            }
            else
            {
                float ratio = (float)values[phase].Value / (float)checkActions;
                Statistic ret = new Statistic(new StatisticsPercentageData(name, ratio), category);

                if (ratings != null) AppendActionsSubstatistics(phase, ret, ratings);

                return ret;
            }
        }

        /* How many times has the player check raised? */
        public Statistic GetCheckRaiseStats(HoldemGamePhase phase, String category)
        {
            return GetCheckActionStats(phase, "Check Raise", category, checkRaises, checksRaisesRatings);
        }

        /* How many times has the player check called? */
        public Statistic GetCheckCallStats(HoldemGamePhase phase, String category)
        {
            return GetCheckActionStats(phase, "Check Call", category, checkCalls, checksCallsRatings);
        }

        /* How many times has the player check folded? */
        public Statistic GetCheckFoldStats(HoldemGamePhase phase, String category)
        {
            return GetCheckActionStats(phase, "Check Fold", category, checkFolds, null);
        }


        /* How many times has the player steal raised? */
        public Statistic GetStealRaiseStats()
        {
            if (opportunitiesToStealRaise == 0) return Statistic.CreateUnknown("Steal Raises", "Preflop");
            else
            {
                float stealRaiseRatio = (float)stealRaises.Value / (float)opportunitiesToStealRaise;

                return new Statistic(new StatisticsPercentageData("Steal Raises", stealRaiseRatio), "Preflop");
            }
        }

        /* How many times has the player folded the blind to a steal raise? */
        public Statistic GetFoldsToAStealRaiseStats(BlindType blindType)
        {
            int actionsToAStealRaise = (int)callsToAStealRaise[blindType].Value + (int)raisesToAStealRaise[blindType].Value + (int)foldsToAStealRaise[blindType].Value;
            String statDescription = String.Format("Fold {0} to a Steal Raise", 
                            (blindType == BlindType.BigBlind) ? "Big Blind" : "Small Blind");


            if (actionsToAStealRaise == 0) return Statistic.CreateUnknown(statDescription, "Preflop");
            else
            {
                float foldToAStealRaiseRatio = (float)foldsToAStealRaise[blindType].Value / (float)actionsToAStealRaise;

                return new Statistic(new StatisticsPercentageData(statDescription, foldToAStealRaiseRatio), "Preflop");
            }
        }

        /* How many times has the player folded the blind to a raise (but not a reraise)? */
        public Statistic GetFoldsBlindToPreflopRaise(BlindType blindType)
        {
            int actionsToPreflopRaise = (int)callsBlindToAPreflopRaise[blindType].Value + (int)raisesBlindToAPreflopRaise[blindType].Value + (int)foldsBlindToAPreflopRaise[blindType].Value;
            String statDescription = String.Format("Fold {0} to a Raise",
                            (blindType == BlindType.BigBlind) ? "Big Blind" : "Small Blind");


            if (actionsToPreflopRaise == 0) return Statistic.CreateUnknown(statDescription, "Preflop");
            else
            {
                float foldBlindToPreflopRaiseRatio = (float)foldsBlindToAPreflopRaise[blindType].Value / (float)actionsToPreflopRaise;

                return new Statistic(new StatisticsPercentageData(statDescription, foldBlindToPreflopRaiseRatio), "Preflop");
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

        private Statistic GetStartingHandsStatistics(List<HoldemHand> handList, String statName, String category)
        {
            if (handList.Count > 0)
            {
                String result = String.Empty;
                foreach (HoldemHand hand in handList)
                {
                    result += hand.ToString() + ", ";
                }

                // Eliminate last comma
                result = result.Substring(0, result.Length - 2);
                return new Statistic(new StatisticsDescriptiveData(statName, result), category);
            }
            else return Statistic.CreateUnknown(statName, category);
        }

        /* Get hands that the player pushed all-in with preflop */
        private Statistic GetStartingHandsWithPreflopAllInStats()
        {
            return GetStartingHandsStatistics(startingHandsWithPreflopAllIn, "All-in hands", "Preflop");
        }

        /* Get hands that the player called with preflop */
        private Statistic GetStartingHandsWithPreflopCallStats()
        {
            return GetStartingHandsStatistics(startingHandsWithPreflopCall, "Call/limp starting hands", "Preflop");
        }

        /* Get hands that the player raised with preflop */
        private Statistic GetStartingHandsWithPreflopRaiseStats()
        {
            return GetStartingHandsStatistics(startingHandsWithPreflopRaise, "Raise starting hands", "Preflop");
        }

        /* Get style of play */
        public Statistic GetStyle()
        {
            /* Is this player tight, semi-loose or loose?
             * Depends on his VPF ratio */
            float vpf = GetVPFStats().MainData.GetFloat(2);

            /* Is this player aggressive or passive? 
             * Depends on the aggression frequency */
            float aggressionFrequency = GetAggressionFrequencyStats().MainData.GetFloat(2);

            String tightness = String.Empty;
            String aggressiveness = String.Empty;

            if (vpf < 0.15) tightness = "Tight";
            else if (vpf >= 0.15 && vpf <= 0.2) tightness = "Semi-loose";
            else if (vpf > 0.2) tightness = "Loose";

            if (aggressionFrequency > 0.2) aggressiveness = "Aggressive";
            else aggressiveness = "Passive";

            return new Statistic(new StatisticsDescriptiveData("Style",String.Format("{0} {1}",tightness, aggressiveness)), "Summary");
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
            pushedAllIn[gamePhase].Increment();
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

        private List<String> GetPreflopPushingRange()
        {
            List<String> result = new List<String>();
            if (startingHandsWithPreflopRaise.Count > 0)
            {
                // We calculate mean and standard deviation of the raising hands preflop
                double percentileSum = 0;
                double derivationSum = 0;
                foreach (HoldemHand hand in startingHandsWithPreflopRaise)
                {
                    double percentile = (double)hand.GetPrelopPercentile();
                    percentileSum += percentile;
                    derivationSum += (percentile) * (percentile);
                }

                double mean = percentileSum / startingHandsWithPreflopRaise.Count;
                double derivationSumAverage = derivationSum / startingHandsWithPreflopRaise.Count;
                double stdDeviation = Math.Sqrt(derivationSumAverage - (mean * mean));

                // Then the range is given by all the preflop hands that have a percentile
                // less than (mean + standard deviation)
                float range = (float)mean + (float)stdDeviation;

                result = HoldemHand.GetCardsWithinPercentile(range);                
            }

            return result;
        }

        public void DisplayPreflopPushingRangeWindow()
        {
            HoldemCardDisplayDialog d = new HoldemCardDisplayDialog();
            d.SelectCards(GetPreflopPushingRange());
            d.Text = "Estimated Preflop Pushing Range for " + this.Name;
            d.Show();
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
            result.Set(GetStartingHandsWithPreflopAllInStats());
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

            result.Set(GetChecksStats(HoldemGamePhase.Preflop, "Preflop"));
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

            result.Set(GetDrawStatistics(HoldemGamePhase.Flop, "Flop"));
            result.Set(GetDrawStatistics(HoldemGamePhase.Turn, "Turn"));

            result.Set(GetStyle());
            result.Set(GetAggressionFrequencyStats());

            result.Set(GetPushedAllInStats());

            // Calculate a few averages

            // Overall raises % average across all streets
            Statistic raisesPreflop = result.Get("Raises", "Preflop");
            Statistic raisesAverage = raisesPreflop.Average("Summary", 0, result.Get("Raises", "Flop"),
                                                                                     result.Get("Raises", "Turn"),
                                                                                     result.Get("Raises", "River"));
            result.Set(raisesAverage);
            
            // Overall bets % average across all streets
            Statistic betsFlop = result.Get("Bets", "Flop");
            Statistic betsAverage = betsFlop.Average("Summary", 0, result.Get("Bets", "Turn"),
                                                                                     result.Get("Bets", "River"));
            result.Set(betsAverage);

            // Overall folds % average across all streets
            Statistic foldsPreflop = result.Get("Folds", "Preflop");
            Statistic foldsAverage = foldsPreflop.Average("Summary", 0, result.Get("Folds", "Flop"),
                                                                                     result.Get("Folds", "Turn"),
                                                                                     result.Get("Folds", "River"));
            result.Set(foldsAverage);

            // Overall calls % average across all streets
            Statistic callsPreflop = result.Get("Calls", "Preflop");
            Statistic callsAverage = callsPreflop.Average("Summary", 0, result.Get("Calls", "Flop"),
                                                                                     result.Get("Calls", "Turn"),
                                                                                     result.Get("Calls", "River"));
            result.Set(callsAverage);

            // Overall checks % average across all streets
            Statistic checksPreflop = result.Get("Checks", "Preflop");
            Statistic checksAverage = checksPreflop.Average("Summary", 0, result.Get("Checks", "Flop"),
                                                                                     result.Get("Checks", "Turn"),
                                                                                     result.Get("Checks", "River"));
            result.Set(checksAverage);
            
            
            // Overall check-calls % across all streets
            Statistic checkCallsFlop = result.Get("Check Call", "Flop");
            Statistic checkCallsAverage = checkCallsFlop.Average("Summary", 0, result.Get("Check Call", "Turn"),
                                                                                     result.Get("Check Call", "River"));
            result.Set(checkCallsAverage);

            // Overall check-raises % across all streets
            Statistic checkRaisesFlop = result.Get("Check Raise", "Flop");
            Statistic checkRaisesAverage = checkRaisesFlop.Average("Summary", 0, result.Get("Check Raise", "Turn"),
                                                                                     result.Get("Check Raise", "River"));
            result.Set(checkRaisesAverage);

            // Overall check-fold % across all streets
            
            Statistic checkFoldsFlop = result.Get("Check Fold", "Flop");
            Statistic checkFoldsAverage = checkFoldsFlop.Average("Summary", 0, result.Get("Check Fold", "Turn"),
                                                                                     result.Get("Check Fold", "River"));
            result.Set(checkFoldsAverage);



            return result;
        }
    }
}
