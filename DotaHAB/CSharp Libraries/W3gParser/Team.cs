using System.Collections.Generic;

namespace Deerchao.War3Share.W3gParser
{
    public class Team
    {
        private readonly int teamNo;
        private TeamType type;
        public Team(int no)
        {
            teamNo = no;
        }

        public int TeamNo
        {
            get { return teamNo; }
        }

        public TeamType Type
        {
            get { return type; }
        }

        public List<Player> Players
        {
            get { return players; }
        }

        public bool IsWinner = false;

        readonly List<Player> players = new List<Player>();

        public void Add(Player p)
        {
            players.Add(p);
            this.type = p.TeamType;
        }
        public void AddSortedBySlot(Player p)
        {
            for(int i=0; i<players.Count; i++)
                if (p.SlotNo < players[i].SlotNo)
                {
                    players.Insert(i, p);
                    goto SETTYPE;
                }

            players.Add(p);

        SETTYPE: ;
            this.type = p.TeamType;
        }
    }
}