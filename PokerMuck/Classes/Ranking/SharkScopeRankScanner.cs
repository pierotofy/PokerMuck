using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PokerMuck
{

    class SharkScopeRankScanner : RankScanner
    {
        /* {0} = Username
         * {1} = Network name (trimmed to alphanumeric only, no spaces, no special chars)
         * ex. PokerStars.it = pokerstarsit
        */
        private static string SEARCH_PLAYER_URL = "http://www.sharkscope.com/SharkScope/FindPlayer?searchstring={0}&Username=sswebsite&Password=&Network={1}&GeneralSearchType=ALL&Skin=&Filter=&searchTag=12.26662229789376&nocache=0&version=355&DecimalSeparator=.";

        public SharkScopeRankScanner()
        {
        }

        //?pnid=2&pn=POKERFEDE87&tt=sng
        public override Rank FindPlayerRank(string playerName)
        {
            // TODO in another thread

            /* We'll use this to make requests to the website */
            PostSubmitter post = new PostSubmitter();
            post.Type = PostSubmitter.PostTypeEnum.Get;

            /* See if the player is in the database for our client */
            String requestURL = String.Format(SEARCH_PLAYER_URL, playerName, "pokerstarsit");
            
            try
            {
                string searchResult = post.Post(requestURL);
                Trace.WriteLine(searchResult);

                /* Match <div val="19" class="s2" title="J 2 Star(s)">J</div> */
                Regex r = new Regex("<div val=\"[0-9]+\" class=\"s[0-9]\" title=\"([.]{3}) Star(s)\">J</div>");

                Match m = r.Match(searchResult);
                if (m.Success){
                    Trace.WriteLine(m.Groups[0].Value);
                }
            
            }
            catch (Exception e)
            {
                // Something went wrong?
                Trace.WriteLine(e.ToString());

                return null; //TODO error handling?
            }

            return null;

          
        }


    }
}
