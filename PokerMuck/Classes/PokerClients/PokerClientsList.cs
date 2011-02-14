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
        
        /* Adds a poker client to the list of supported clients */
        public static void Add(PokerClient client)
        {
            clientList.Add(client.Name, client);
        }

        public static PokerClient Find(String name)
        {
            Debug.Assert(clientList.ContainsKey(name), "No valid poker client was found: " + name);
           
            return (PokerClient)clientList[name];
        }

    }
}
