using System;

namespace Deerchao.War3Share.W3gParser
{
    public class KillInfo
    {
        public KillInfo(int time, Player killer, Player victim)
        {
            this.Time = new TimeSpan(0, 0, 0, 0, time);
            this.Killer = killer;
            this.Victim = victim;            
        }

        public KillInfo(int time, Player killer, UnitInfo victimInfo)
        {
            this.Time = new TimeSpan(0, 0, 0, 0, time);
            this.Killer = killer;            
            this.VictimInfo = victimInfo;
        }

        public TimeSpan Time
        {
            get;
            private set;
        }

        public Player Killer
        {
            get;
            private set;
        }

        public Player Victim
        {
            get;
            private set;
        }

        public UnitInfo VictimInfo
        {
            get;
            private set;
        }
    }
}