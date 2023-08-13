using System;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Collections.Specialized;

namespace DotaHIT.DatabaseModel
{
    using Data;
    using DataTypes;    

    namespace Environment
    {        
        using DotaHIT.Core.Resources;
        using DotaHIT.Jass;
        using DotaHIT.Jass.Native.Types;        
        using DotaHIT.Jass.Native.Constants;
        using System.Collections.Generic;
        using System.Collections.Specialized;
        using DotaHIT.DatabaseModel.Abilities;

        public class UnitAuraEnvironment : List<unit>
        {
            public UnitAuraEnvironment() { }
            public UnitAuraEnvironment(int capacity):base(capacity) { }
            public UnitAuraEnvironment(IEnumerable<unit> collection) : base(collection) { }

            public void BeginInteraction()
            {
                foreach (unit possibleAuraCarrier in this)
                {
                    DBABILITIES auras = possibleAuraCarrier.acquiredAbilities.GetSpecific(
                        AbilitySpecs.IsAura, 
                        AbilityMatchType.Intersects, 
                        TargetType.Ally | TargetType.Allies | TargetType.Friend | TargetType.Self);

                    foreach (DBABILITY aura in auras)
                        foreach (unit possibleAuraTarget in this)
                            if (aura.TargetsMatch(possibleAuraTarget))
                                possibleAuraTarget.Buffs.Add(aura);
                }
            }

            public void EndInteraction()
            {

            }
        }
    }
}


