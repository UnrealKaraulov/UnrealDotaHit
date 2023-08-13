using System;
using System.Collections.Generic;
using System.Text;

namespace Deerchao.War3Share.W3gParser
{
    public interface IReplay
    {
        string ReplayPath { get; }
        string MapPath { get; }
        bool IsDota { get; }
        string GameMode { get; }
        TimeSpan GameLength { get; }
        int PlayerCount { get; }
        IList<IPlayer> Players { get; }        
        TeamType Winner { get; }
    }
}
