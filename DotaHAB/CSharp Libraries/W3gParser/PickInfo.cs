using System;
using System.Collections.Generic;

namespace Deerchao.War3Share.W3gParser
{
    public struct PickInfo
    {
        private readonly int time;
        private readonly TeamType teamType;        
        private readonly Hero hero;

        public PickInfo(int time, TeamType teamType, Hero hero)
        {
            this.time = time;
            this.teamType = teamType;
            this.hero = hero;
        }

        public int Time
        {
            get
            {
                return time;
            }
        }

        public TeamType TeamType
        {
            get { return teamType; }
        }

        public Hero Hero
        {
            get { return hero; }
        }

        public override string ToString()
        {
            return hero != null ? hero.ToString() : null;
        }
    }
}