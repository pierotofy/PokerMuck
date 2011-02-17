using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            /* 
                    PostSubmitter post = new PostSubmitter();
                    post.Url = "http://www.3dblackjacktrainer.pierotofy.it/bug/submit";

                    //Show status and disable button
                    btnSubmit.Enabled = false;
                    SetStatus("Looking for system information...");

                    //OS version
                    post.PostItems.Add("bug[os]", Environment.OSVersion.VersionString);

                    //RAM
                    VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
                    post.PostItems.Add("bug[ram]", string.Format("{0} mb", ci.TotalPhysicalMemory / 1048576));

                    //Videocard information
                    StringBuilder data = new StringBuilder();
                    int i = 1;
                    foreach (AdapterInformation ai in Manager.Adapters)
                    {

                        data.Append(string.Format("Adapter #{0}: {1}*", i, ai.Information.Description));

                        data.Append(string.Format
                            ("Driver information: {0} - {1}*",
                            ai.Information.DriverName,
                            ai.Information.DriverVersion));

                        // Get each display mode supported
                        data.Append(string.Format
                            ("Current Display Mode: {0}x{1}x{2}*",
                            ai.CurrentDisplayMode.Width,
                            ai.CurrentDisplayMode.Height,
                            ai.CurrentDisplayMode.Format));

                        foreach (DisplayMode dm in ai.SupportedDisplayModes)
                        {
                            data.Append(string.Format
                                ("Supported: {0}x{1}x{2}*",
                                dm.Width, dm.Height, dm.Format));
                        }

                        i++;
                    }
                    post.PostItems.Add("bug[videocardsinfo]", data.ToString());

                    //Game error
                    post.PostItems.Add("bug[gameerror]", gameError);

                    //Game version
                    post.PostItems.Add("bug[gameversion]", gameVersion);

                    //Stack trace
                    post.PostItems.Add("bug[stacktrace]", stackTrace);

                    //User description
                    post.PostItems.Add("bug[usererrordescription]", txtDescription.Text);

                    
                    SetStatus("Submitting the information...");
                    post.Type = PostSubmitter.PostTypeEnum.Get;

                    try
                    {
                        string result = post.Post();
                    }
                    catch (Exception)
                    {
                        //It might happen that the bug has been submitted already,
                        //So the server refuses the post
                    }
             * */
        }

        protected override float GetMaxRankValue()
        {
            return 65.0f; // 5 stars * num cards in a deck
        }

        protected override float GetMinRankValue()
        {
            return 1.0f; // 1 star, two
        }


    }
}
