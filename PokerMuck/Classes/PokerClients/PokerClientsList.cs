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
        private static Hashtable clientList = new Hashtable();
        public static Hashtable ClientList { get { return clientList; } }
        private static PokerClient defaultClient;
        
        /* Adds a poker client to the list of supported clients */
        public static void Add(PokerClient client)
        {
            clientList.Add(client.Name, client);
        }

        public static void SetDefault(PokerClient client)
        {
            defaultClient = client;
        }

        public static PokerClient Find(String name)
        {
            Trace.Assert(defaultClient != null, "Trying to find a poker client before the default client has been set");

            if (clientList.ContainsKey(name))
            {
                return (PokerClient)clientList[name];
            }
            else
            {
                Trace.WriteLine("Could not find client: " + name + ", defaulting to " + defaultClient.Name);
                return defaultClient;
            }           
        }

    }
}
