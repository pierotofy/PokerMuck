using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerMuck{
    /* This class encapsulates all the details that are passed when an event is raised from the
     * HHParser class. They include information such as the GameID, TableID that identify what game
     * we are currently parsing */
    public class HHParserEventArgs
    {
        
        private String gameId;
        private String tableId;

        public String GameId{ get { return gameId; } }
        public String TableId{ get { return tableId; } }

        public HHParserEventArgs(String gameId, String tableId){
            this.gameId = gameId;
            this.tableId = tableId;
        }
    }
}
