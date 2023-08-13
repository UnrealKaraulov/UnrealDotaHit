using System;

namespace Deerchao.War3Share.W3gParser
{
    public struct UnitInfo
    {
        public UnitInfo(TeamType teamType, string description) : this()
        {
            this.TeamType = teamType;
            this.Description = description;
        }

        public TeamType TeamType
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }
    }
}