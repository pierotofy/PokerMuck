using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    /* The TS Ranking system uses a playing card with 5 stars as a level to identify the rank of the player.
     * The cards are Ace, King, Queen, Jack, 10,9,8,7,6,5,4,3,2 and the stars are 1,2,3,4,5 . 
     * Ace with 5 stars is the highest rank and 2 with 1 star is the lowest rank. The rank is calculated 
     * based on the number of tournament entries, where you finish and the buy-in amount.
     * http://www.pokerprolabs.com */
 
    class PokerProLabsRankScanner : RankScanner
    {
        private static string SEARCH_PLAYER_URL = "http://www.pokerprolabs.com/topsharkpro/controls/search";

        public PokerProLabsRankScanner()
        {
        }

        public override Rank FindPlayerRank(string playerName)
        {
            // TODO in another thread

            /* We'll use this to make post requests to the website */
            PostSubmitter post = new PostSubmitter();
            post.Type = PostSubmitter.PostTypeEnum.Post;

            /* Do a preliminary search to see if the player is in the database */
            post.PostItems.Add("pn", playerName);
            
            try
            {
                string result = post.Post(SEARCH_PLAYER_URL);
                Debug.Print(result);
            }
            catch (Exception)
            {
                // Something went wrong?

                return null; //TODO error handling?
            }

            return null;

          
        }


    }
}
