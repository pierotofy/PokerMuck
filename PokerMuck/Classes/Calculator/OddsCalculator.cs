using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck
{
    abstract public class OddsCalculator
    {
        public static OddsCalculator CreateFor(PokerGame game)
        {
            switch(game){
                case PokerGame.Holdem:
                    return new HoldemOddsCalculator();
                default:
                    return null;
            }
        }

        public abstract List<Statistic> Calculate(Hand hand);
    }
}
