using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;

namespace PokerMuck
{
    public class Player : ICloneable
    {
        /* Every player has a name */
        private String name;
        public String Name { get { return name; } }

        /* Last mucked hand are stored here */
        public Hand MuckedHand { get; set; }

        /* Has this player showed his hands last round? */
        public bool HasShowedThisRound { get; set; }

        /* Number of hands played */
        protected ValueCounter totalHandsPlayed;

        /* Reference to the Hud Window (if any), can be null */
        public HudWindow HudWindow { get; set; }

        /* Has this player played last round? */
        public bool HasPlayedLastRound { get; set; }

        /* Is this player still playing or is he out? */
        public bool IsPlaying { get; set; }

        /* What Game ID is this player playing? */
        public String GameID { get; set; }
        
        /* What seat # is the player seated at? 
           -1 = Unknown or none */
        public int SeatNumber { get; set; }

        /* What game type are we playing? */
        public virtual PokerGame GameType { get { return PokerGame.Unknown; } }
        
        protected Player(String name)
        {
            this.name = name;
            this.HasShowedThisRound = false;
            this.HudWindow = null;
            this.IsPlaying = false;
            this.HasPlayedLastRound = true;
            this.SeatNumber = -1; // Don't know
            this.GameID = String.Empty; // Don't know
            this.totalHandsPlayed = new ValueCounter();
        }

        /* This player received the whole cards */
        public void IsDealtHoleCards()
        {
            totalHandsPlayed.Increment();
        }

        public void window_OnResetStatisticsButtonPressed(HudWindow sender)
        {
            ResetAllStatistics();
            sender.DisplayStatistics(GetStatistics());
        }

        public virtual void ResetAllStatistics()
        {
            /* Reset the stats for a particular set */
            totalHandsPlayed.Reset();
        }

        /* Certain statistics need to be calculated at the end of the round
         * because we might receive certain data in mixed order */
        public virtual void CalculateEndOfRoundStatistics()
        {

        }

        /* Reset the statistics variables that are valid for one round only 
         * ex. We count only one call preflop as a limp, if a player raises and the first player calls again
         * this has to be counted as a single limp, not as two */
        public virtual void PrepareStatisticsForNewRound()
        {
            totalHandsPlayed.AllowIncrement();
        }

        /* Given the name of a category, returns an integer indicating its ordering
         * 0 = first, inf = last */
        protected virtual int GetCategoryOrder(String category) { return 0; }

        /* How should the statistics categories be ordered? */
        protected int CompareCategories(String category1, String category2)
        {
            int order1 = GetCategoryOrder(category1);
            int order2 = GetCategoryOrder(category2);

            if (order1 == order2) return 0;
            else if (order1 < order2) return -1;
            else return 1;
        }

        /* Returns all the statistics available for the current player */
        public virtual PlayerStatistics GetStatistics()
        {
            PlayerStatistics result = new PlayerStatistics(CompareCategories);
            result.Set(new Statistic(new StatisticsNumberData("Total Hands Played", totalHandsPlayed.Value), "Summary"));
            return result;
        }

        /* A mucked hand became available for this player */
        public virtual void MuckHandAvailable(Hand hand)
        {
            MuckedHand = hand;
            HasShowedThisRound = true;
        }

        public void DisposeHud()
        {
            if (HudWindow != null)
            {
                HudWindow.DisposeFlag = true;
                HudWindow = null;
            }
        }

        ~Player(){
            Trace.WriteLine("Destroying... " + Name);

            DisposeHud();
        }

        // Callers should use the typesafe clone() instead
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        // Overriden by derived classes
        public virtual Player Clone()
        {
            return new Player(this);
        }

        protected Player(Player other)
        {
            Trace.WriteLine("Cloning " + other.Name);

            // Copy members here
            this.name = other.Name;
            this.HasShowedThisRound = other.HasShowedThisRound;
            this.HudWindow = null;
            this.IsPlaying = other.IsPlaying;
            this.HasPlayedLastRound = other.HasPlayedLastRound;
            this.SeatNumber = other.SeatNumber;
            this.totalHandsPlayed = (ValueCounter)other.totalHandsPlayed.Clone();
            if (other.MuckedHand != null) this.MuckedHand = (Hand)other.MuckedHand.Clone();
        }

    }
}
