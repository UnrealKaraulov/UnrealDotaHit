using System;
using System.Collections.Generic;
using System.Text;

namespace Deerchao.War3Share.W3gParser
{
    public interface IPlayer
    {
        string Name { get; }
        bool IsComputer { get; }
        bool IsObserver { get; }
        string HeroID { get; }
        TeamType TeamType { get; }
        LineUp LineUp { get; }
        ushort Apm { get; }
        short Kills { get; }
        short Deaths { get; }
        short Assists { get; }
        short CreepKills { get; }
        short CreepDenies { get; }
        short NeutralKills { get; }
    }
}
