using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace PokerMuck
{
    /* Every valid PokerClient subclass should be added to this static list by the Director
     * in order to be considered for instantiation */
    static class PokerClientsList
    {
        private static List<PokerClient> pokerClientsList = new List<PokerClient>(10);
        
        /* Adds a poker client to the list of supported clients */
        public static void Add(PokerClient client)
        {
            pokerClientsList.Add(client);
        }

        public static PokerClient Find(String name)
        {
            PokerClient result = pokerClientsList.Find(
                delegate(PokerClient client)
                {
                    return client.Name == name;
                }
            );

            Debug.Assert(result != null, "No valid poker client was found {0}", name);

            return result;
        }

    }
}
