using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.CodeDom;
using System.Reflection;
using DotaHIT.Jass.Operations;
using DotaHIT.Core;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using DotaHIT.Jass.Commands;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass.Native.Events;
using DotaHIT.Jass.Native.Constants;
using System.Collections.Specialized;
using System.Collections;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.Abilities;
using DotaHIT.DatabaseModel.Upgrades;
using DotaHIT.Core.Resources.Media;
using System.Windows.Forms;
using System.Threading;

namespace DotaHIT.Jass.Native.Functions
{
    public abstract class DHJassNativeFunction : DHJassFunction
    {
        protected void CreateArguments(params DHJassValue[] args)
        {
            argsArray = args;

            for (int i = 0; i < argsArray.Length; i++)
                this.args.Add(argsArray[i].Name, argsArray[i]);
        }
    }

    public class ConsolePrint : DHJassNativeFunction
    {
        public ConsolePrint()
        {
            this.CreateArguments(new DHJassString("str", null));
        }

        protected override DHJassValue Run()
        {
            Console.WriteLine(argsArray[0].Value + "");
            return null;
        }
    }

    #region convertion functions
    public abstract class ConvertFunc : DHJassNativeFunction
    {
        public ConvertFunc()
        {
            this.CreateArguments(new DHJassInt("i", 0));
        }

        protected override DHJassValue Run()
        {
            return args["i"];
        }
    }
    public class ConvertPlayerState : ConvertFunc { }
    public class ConvertPlayerColor : ConvertFunc { }
    public class ConvertRace : ConvertFunc { }
    public class ConvertPlayerGameResult : ConvertFunc { }
    public class ConvertVersion : ConvertFunc { }
    public class ConvertAllianceType : ConvertFunc { }
    public class ConvertAttackType : ConvertFunc { }
    public class ConvertDamageType : ConvertFunc { }
    public class ConvertWeaponType : ConvertFunc { }
    public class ConvertPathingType : ConvertFunc { }
    public class ConvertRacePref : ConvertFunc { }
    public class ConvertMapControl : ConvertFunc { }
    public class ConvertGameType : ConvertFunc { }
    public class ConvertMapFlag : ConvertFunc { }
    public class ConvertPlacement : ConvertFunc { }
    public class ConvertStartLocPrio : ConvertFunc { }
    public class ConvertMapDensity : ConvertFunc { }
    public class ConvertGameDifficulty : ConvertFunc { }
    public class ConvertGameSpeed : ConvertFunc { }
    public class ConvertPlayerSlotState : ConvertFunc { }
    public class ConvertVolumeGroup : ConvertFunc { }
    public class ConvertIGameState : ConvertFunc { }
    public class ConvertFGameState : ConvertFunc { }
    public class ConvertUnitState : ConvertFunc { }
    public class ConvertAIDifficulty : ConvertFunc { }
    public class ConvertPlayerScore : ConvertFunc { }
    public class ConvertGameEvent : ConvertFunc { }
    public class ConvertPlayerEvent : ConvertFunc { }
    public class ConvertPlayerUnitEvent : ConvertFunc { }
    public class ConvertUnitEvent : ConvertFunc { }
    public class ConvertWidgetEvent : ConvertFunc { }
    public class ConvertDialogEvent : ConvertFunc { }
    public class ConvertLimitOp : ConvertFunc { }
    public class ConvertUnitType : ConvertFunc { }
    public class ConvertItemType : ConvertFunc { }
    public class ConvertCameraField : ConvertFunc { }
    public class ConvertBlendMode : ConvertFunc { }
    public class ConvertRarityControl : ConvertFunc { }
    public class ConvertTexMapFlags : ConvertFunc { }
    public class ConvertFogState : ConvertFunc { }
    public class ConvertEffectType : ConvertFunc { }
    public class ConvertSoundType : ConvertFunc { }
    public class I2S : ConvertFunc
    {
        protected override DHJassValue Run()
        {
            return new DHJassString(null, args["i"].StringValue);
        }
    }
    public class S2I : DHJassNativeFunction
    {
        public S2I()
        {
            this.CreateArguments(new DHJassString("i", null));
        }

        protected override DHJassValue Run()
        {
            int result;
            int.TryParse(args["i"].StringValue, out result);
            return new DHJassInt(null, result);
        }
    }
    public class R2I : ConvertFunc
    {
        protected override DHJassValue Run()
        {
            return new DHJassInt(null, args["i"].IntValue);
        }
    }
    public class R2S : ConvertFunc
    {
        protected override DHJassValue Run()
        {
            return new DHJassString(null, args["i"].StringValue);
        }
    }
    public class GetHandleId : DHJassNativeFunction
    {
        public GetHandleId()
        {
            this.CreateArguments(new DHJassHandle("i", 0));
        }

        protected override DHJassValue Run()
        {            
            return new DHJassInt(null, args["i"].IntValue);
        }
    }
    #endregion

    #region empty functions
    public abstract class EmptyFunc : DHJassNativeFunction
    {
        protected override DHJassValue Run()
        {
            return null;
        }
    }
    public abstract class EmptyValueFunc : DHJassNativeFunction
    {
        protected override DHJassValue Run()
        {
            return new DHJassUnusedType(); ;
        }
    }

    public class SetDayNightModels : EmptyFunc { }

    public class CreateSound : EmptyValueFunc { }
    public class CreateSoundFromLabel : EmptyValueFunc { }
    public class StartSound : EmptyFunc { }
    public class StopSound : EmptyFunc { }
    public class SetSoundVolume : EmptyFunc { }
    public class SetSoundDuration : EmptyFunc { }
    public class SetSoundChannel : EmptyFunc { }

    public class CreateMIDISound : EmptyValueFunc { }
    
    public class SetPlayerHandicapXP : EmptyFunc { }

    public class CreateCameraSetup : EmptyValueFunc { }
    public class SetCameraBounds : EmptyFunc { }
    public class SetWaterBaseColor : EmptyFunc { }
    public class NewSoundEnvironment : EmptyFunc { }
    public class SetMapMusic : EmptyFunc { }
    public class SetSoundPitch : EmptyFunc { }
    public class SetSoundParamsFromLabel : EmptyFunc { }
    public class CameraSetupSetField : EmptyFunc { }
    public class CameraSetupSetDestPosition : EmptyFunc { }
    public class SetUnitColor : EmptyFunc { }
    public class SetFloatGameState : EmptyFunc { }
    public class SuspendTimeOfDay : EmptyFunc { }
    public class SetTimeOfDayScale : EmptyFunc { }
    public class SetSkyModel : EmptyFunc { }
    public class SetCreepCampFilterState : EmptyFunc { }
    public class GetCameraMargin : EmptyValueFunc { }
    public class AddWeatherEffect : EmptyValueFunc { }
    public class SetAllyColorFilterState : EmptyFunc { }
    public class SetPlayerColor : EmptyFunc { }
    public class IsFogEnabled : EmptyValueFunc { }
    public class IsFogMaskEnabled : EmptyValueFunc { }
    public class SetAllItemTypeSlots : EmptyFunc { }
    public class SetAllUnitTypeSlots : EmptyFunc { }    

    public class QuestSetTitle : EmptyFunc { }
    public class QuestSetDescription : EmptyFunc { }
    public class QuestSetIconPath : EmptyFunc { }
    public class QuestSetRequired : EmptyFunc { }
    public class QuestSetDiscovered : EmptyFunc { }
    public class QuestSetCompleted : EmptyFunc { }
    public class CreateQuest : EmptyValueFunc { }
    public class MultiboardDisplay : EmptyFunc { }
    public class MultiboardSetItemValue : EmptyFunc { }
    public class MultiboardReleaseItem : EmptyFunc { }
    public class MultiboardGetRowCount : EmptyValueFunc { }
    public class MultiboardGetColumnCount : EmptyValueFunc { }
    public class MultiboardSetItemValueColor : EmptyFunc { }
    public class MultiboardSetItemWidth : EmptyFunc { }
    public class MultiboardSetItemStyle : EmptyFunc { }

    public class PlayerSetLeaderboard : EmptyFunc { }
    public class LeaderboardDisplay : EmptyFunc { }
    public class LeaderboardSetLabel : EmptyFunc { }
    public class CreateLeaderboard : EmptyValueFunc { }
    public class MultiboardSetItemIcon : EmptyFunc { }
    #endregion

    public class SetMapName : DHJassNativeFunction
    {
        public SetMapName()
        {
            this.CreateArguments(new DHJassString("name", null));
        }

        protected override DHJassValue Run()
        {
            DHJassExecutor.Globals["DHMapName"] = new DHJassString(null, args["name"].StringValue);
            return null;
        }
    }    
    public class SetMapDescription : DHJassNativeFunction
    {
        public SetMapDescription()
        {
            this.CreateArguments(new DHJassString("description", null));
        }

        protected override DHJassValue Run()
        {
            DHJassExecutor.Globals["DHMapDescription"] = new DHJassString(null, args["description"].StringValue);
            return null;
        }
    }
    public class VersionGet : DHJassNativeFunction
    {
        public VersionGet()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, 1); // VERSION_FROZEN_THRONE (common.j)
        }
    }

    public class FlushGameCache : DHJassNativeFunction
    {
        public FlushGameCache()
        {
            this.CreateArguments(new DHJassHandle("cache", 0));
        }
        protected override DHJassValue Run()
        {
            gamecache gc = args["cache"].HandleValue as gamecache;
            if (gc != null) gc.flush();
            return null;
        }
    }
    public class InitGameCache : DHJassNativeFunction
    {
        public InitGameCache()
        {
            this.CreateArguments(new DHJassString("campaignFile", null));
        }
        protected override DHJassValue Run()
        {
            string name = args["campaignFile"].StringValue;
            gamecache gc = new gamecache(name);
            return new DHJassHandle(null, gc.handle);
        }
    }
    public class FlushStoredMission : DHJassNativeFunction
    {
        public FlushStoredMission()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                new DHJassString("missionKey", null));
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache != null) cache.FlushStoredMission(args["missionKey"].StringValue);
            return null;
        }
    }
    public class StoreString : DHJassNativeFunction
    {
        public StoreString()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null),
                                new DHJassString("value", null)
                                );
        }

        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassBoolean(null, false);

            cache.StoreValue(args["missionKey"].StringValue,
                            args["key"].StringValue,
                            args["value"].StringValue);

            return new DHJassBoolean(null, true);
        }
    }
    public class GetStoredString : DHJassNativeFunction
    {
        public GetStoredString()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassString(null, null);

            object value = cache.GetStoredValue(args["missionKey"].StringValue,
                                args["key"].StringValue);
            if (value is string)
                return new DHJassString(null, (string)value);
            else
                return new DHJassString(null, null);
        }
    }
    public class StoreInteger : DHJassNativeFunction
    {
        public StoreInteger()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null),
                                new DHJassInt("value", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return null;            

            cache.StoreValue(args["missionKey"].StringValue,
                            args["key"].StringValue,
                            args["value"].IntValue);

            return null;
        }
    }
    public class GetStoredInteger : DHJassNativeFunction
    {
        public GetStoredInteger()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassInt(null, 0);

            object value = cache.GetStoredValue(args["missionKey"].StringValue,
                                args["key"].StringValue);
            if (value is int)
                return new DHJassInt(null, (int)value);
            else
                return new DHJassInt(null, 0);
        }
    }
    public class StoreBoolean : DHJassNativeFunction
    {
        public StoreBoolean()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null),
                                new DHJassBoolean("value", false)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return null;

            cache.StoreValue(args["missionKey"].StringValue,
                            args["key"].StringValue,
                            args["value"].BoolValue);

            return null;
        }
    }
    public class GetStoredBoolean : DHJassNativeFunction
    {
        public GetStoredBoolean()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassBoolean(null, false);

            object value = cache.GetStoredValue(args["missionKey"].StringValue,
                                args["key"].StringValue);
            if (value is bool)
                return new DHJassBoolean(null, (bool)value);
            else
                return new DHJassBoolean(null, false);
        }
    }
    public class HaveStoredBoolean : DHJassNativeFunction
    {
        public HaveStoredBoolean()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null));
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassBoolean(null, false);

            bool result = cache.HaveStoredValue(
                args["missionKey"].StringValue,
                args["key"].StringValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class StoreReal : DHJassNativeFunction
    {
        public StoreReal()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null),
                                new DHJassReal("value", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return null;

            cache.StoreValue(args["missionKey"].StringValue,
                            args["key"].StringValue,
                            args["value"].RealValue);

            return null;
        }
    }
    public class GetStoredReal : DHJassNativeFunction
    {
        public GetStoredReal()
        {
            this.CreateArguments(new DHJassHandle("cache", 0),
                                new DHJassString("missionKey", null),
                                new DHJassString("key", null)
                                );
        }
        protected override DHJassValue Run()
        {
            gamecache cache = args["cache"].HandleValue as gamecache;
            if (cache == null) return new DHJassReal(null, 0.0);

            object value = cache.GetStoredValue(args["missionKey"].StringValue,
                                args["key"].StringValue);
            if (value is double)
                return new DHJassReal(null, (double)value);
            else
                return new DHJassReal(null, 0.0);
        }
    }

    public class FlushParentHashtable : DHJassNativeFunction
    {
        public FlushParentHashtable()
        {
            this.CreateArguments(new DHJassHandle("table", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null) table.flush();
            return null;
        }
    }
    public class InitHashtable : DHJassNativeFunction
    {
        public InitHashtable()
        {            
        }
        protected override DHJassValue Run()
        {
            hashtable table = new hashtable();
            return new DHJassHandle(null, table.handle);
        }
    }
    public class FlushChildHashtable : DHJassNativeFunction
    {
        public FlushChildHashtable()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                new DHJassInt("parentKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null) table.FlushChildHashtable(args["parentKey"].IntValue);
            return null;
        }
    }
    public class SaveInteger : DHJassNativeFunction
    {
        public SaveInteger()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0),
                                new DHJassInt("value", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return null;

            table.SaveInteger(args["parentKey"].IntValue,
                            args["childKey"].IntValue,
                            args["value"].IntValue);

            return null;
        }
    }
    public class SaveReal : DHJassNativeFunction
    {
        public SaveReal()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0),
                                new DHJassReal("value", 0.0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return null;

            table.SaveReal(args["parentKey"].IntValue,
                            args["childKey"].IntValue,
                            args["value"].RealValue);

            return null;
        }
    }
    public class SaveBoolean : DHJassNativeFunction
    {
        public SaveBoolean()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0),
                                new DHJassBoolean("value", false)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return null;

            table.SaveBoolean(args["parentKey"].IntValue,
                            args["childKey"].IntValue,
                            args["value"].BoolValue);

            return null;
        }
    }
    public class SaveStr : DHJassNativeFunction
    {
        public SaveStr()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0),
                                new DHJassString("value", null)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return null;

            table.SaveString(args["parentKey"].IntValue,
                            args["childKey"].IntValue,
                            args["value"].StringValue);

            return new DHJassBoolean(null, true);
        }
    }
    public abstract class SaveHandle : DHJassNativeFunction
    {
        public SaveHandle()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0),
                                new DHJassHandle("value", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return null;

            table.SaveHandle(args["parentKey"].IntValue,
                            args["childKey"].IntValue,
                            args["value"].HandleValue);

            return new DHJassBoolean(null, true);
        }
    }
    #region savehandle functions    
    public class SavePlayerHandle : SaveHandle { }
    public class SaveWidgetHandle : SaveHandle { }
    public class SaveDestructableHandle : SaveHandle { }
    public class SaveItemHandle : SaveHandle { }
    public class SaveUnitHandle : SaveHandle { }
    public class SaveAbilityHandle : SaveHandle { }
    public class SaveTimerHandle : SaveHandle { }
    public class SaveTriggerHandle : SaveHandle { }
    public class SaveTriggerConditionHandle : SaveHandle { }
    public class SaveTriggerActionHandle : SaveHandle { }
    public class SaveTriggerEventHandle : SaveHandle { }
    public class SaveForceHandle : SaveHandle { }
    public class SaveGroupHandle : SaveHandle { }
    public class SaveLocationHandle : SaveHandle { }
    public class SaveRectHandle : SaveHandle { }
    public class SaveBooleanExprHandle : SaveHandle { }
    public class SaveSoundHandle : SaveHandle { }
    public class SaveEffectHandle : SaveHandle { }
    public class SaveUnitPoolHandle : SaveHandle { }
    public class SaveItemPoolHandle : SaveHandle { }
    public class SaveQuestHandle : SaveHandle { }
    public class SaveQuestItemHandle : SaveHandle { }
    public class SaveDefeatConditionHandle : SaveHandle { }
    public class SaveTimerDialogHandle : SaveHandle { }
    public class SaveLeaderboardHandle : SaveHandle { }
    public class SaveMultiboardHandle : SaveHandle { }
    public class SaveMultiboardItemHandle : SaveHandle { }
    public class SaveTrackableHandle : SaveHandle { }
    public class SaveDialogHandle : SaveHandle { }
    public class SaveButtonHandle : SaveHandle { }
    public class SaveTextTagHandle : SaveHandle { }
    public class SaveLightningHandle : SaveHandle { }
    public class SaveImageHandle : SaveHandle { }
    public class SaveUbersplatHandle : SaveHandle { }
    public class SaveRegionHandle : SaveHandle { }
    public class SaveFogStateHandle : SaveHandle { }
    public class SaveFogModifierHandle : SaveHandle { }
    public class SaveAgentHandle : SaveHandle { }
    public class SaveHashtableHandle : SaveHandle { }
    #endregion
    public class LoadInteger : DHJassNativeFunction
    {
        public LoadInteger()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassInt(null, 0);

            int value = table.LoadInteger(args["parentKey"].IntValue,
                                args["childKey"].IntValue);
            
            return new DHJassInt(null, value);
        }
    }
    public class LoadReal : DHJassNativeFunction
    {
        public LoadReal()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassReal(null, 0.0);

            double value = table.LoadReal(args["parentKey"].IntValue,
                                args["childKey"].IntValue);

            return new DHJassReal(null, value);
        }
    }
    public class LoadBoolean : DHJassNativeFunction
    {
        public LoadBoolean()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool value = table.LoadBoolean(args["parentKey"].IntValue,
                                args["childKey"].IntValue);

            return new DHJassBoolean(null, value);
        }
    }
    public class LoadStr : DHJassNativeFunction
    {
        public LoadStr()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassString(null, null);

            string value = table.LoadString(args["parentKey"].IntValue,
                                args["childKey"].IntValue);

            return new DHJassString(null, value);
        }
    }
    public abstract class LoadHandle : DHJassNativeFunction
    {
        public LoadHandle()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0)
                                );
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassHandle(null, 0);

            handlevalue value = table.LoadHandle(args["parentKey"].IntValue,
                                args["childKey"].IntValue);

            return new DHJassHandle(null, value != null ? value.handle : 0);
        }
    }
    #region loadhandle functions
    public class LoadPlayerHandle : LoadHandle { }
    public class LoadWidgetHandle : LoadHandle { }
    public class LoadDestructableHandle : LoadHandle { }
    public class LoadItemHandle : LoadHandle { }
    public class LoadUnitHandle : LoadHandle { }
    public class LoadAbilityHandle : LoadHandle { }
    public class LoadTimerHandle : LoadHandle { }
    public class LoadTriggerHandle : LoadHandle { }
    public class LoadTriggerConditionHandle : LoadHandle { }
    public class LoadTriggerActionHandle : LoadHandle { }
    public class LoadTriggerEventHandle : LoadHandle { }
    public class LoadForceHandle : LoadHandle { }
    public class LoadGroupHandle : LoadHandle { }
    public class LoadLocationHandle : LoadHandle { }
    public class LoadRectHandle : LoadHandle { }
    public class LoadBooleanExprHandle : LoadHandle { }
    public class LoadSoundHandle : LoadHandle { }
    public class LoadEffectHandle : LoadHandle { }
    public class LoadUnitPoolHandle : LoadHandle { }
    public class LoadItemPoolHandle : LoadHandle { }
    public class LoadQuestHandle : LoadHandle { }
    public class LoadQuestItemHandle : LoadHandle { }
    public class LoadDefeatConditionHandle : LoadHandle { }
    public class LoadTimerDialogHandle : LoadHandle { }
    public class LoadLeaderboardHandle : LoadHandle { }
    public class LoadMultiboardHandle : LoadHandle { }
    public class LoadMultiboardItemHandle : LoadHandle { }
    public class LoadTrackableHandle : LoadHandle { }
    public class LoadDialogHandle : LoadHandle { }
    public class LoadButtonHandle : LoadHandle { }
    public class LoadTextTagHandle : LoadHandle { }
    public class LoadLightningHandle : LoadHandle { }
    public class LoadImageHandle : LoadHandle { }
    public class LoadUbersplatHandle : LoadHandle { }
    public class LoadRegionHandle : LoadHandle { }
    public class LoadFogStateHandle : LoadHandle { }
    public class LoadFogModifierHandle : LoadHandle { }
    public class LoadHashtableHandle : LoadHandle { }
    #endregion
    public class HaveSavedInteger : DHJassNativeFunction
    {
        public HaveSavedInteger()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool result = table.HaveSavedInteger(
                args["parentKey"].IntValue,
                args["childKey"].IntValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class HaveSavedReal : DHJassNativeFunction
    {
        public HaveSavedReal()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool result = table.HaveSavedReal(
                args["parentKey"].IntValue,
                args["childKey"].IntValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class HaveSavedBoolean : DHJassNativeFunction
    {
        public HaveSavedBoolean()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool result = table.HaveSavedBoolean(
                args["parentKey"].IntValue,
                args["childKey"].IntValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class HaveSavedString : DHJassNativeFunction
    {
        public HaveSavedString()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool result = table.HaveSavedString(
                args["parentKey"].IntValue,
                args["childKey"].IntValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class HaveSavedHandle : DHJassNativeFunction
    {
        public HaveSavedHandle()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table == null) return new DHJassBoolean(null, false);

            bool result = table.HaveSavedHandle(
                args["parentKey"].IntValue,
                args["childKey"].IntValue);

            return new DHJassBoolean(null, result);
        }
    }
    public class RemoveSavedInteger : DHJassNativeFunction
    {
        public RemoveSavedInteger()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null) 
                table.RemoveSavedInteger(
                    args["parentKey"].IntValue,
                    args["childKey"].IntValue);

            return null;
        }
    }
    public class RemoveSavedReal : DHJassNativeFunction
    {
        public RemoveSavedReal()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null)
                table.RemoveSavedReal(
                    args["parentKey"].IntValue,
                    args["childKey"].IntValue);

            return null;
        }
    }
    public class RemoveSavedBoolean : DHJassNativeFunction
    {
        public RemoveSavedBoolean()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null)
                table.RemoveSavedBoolean(
                    args["parentKey"].IntValue,
                    args["childKey"].IntValue);

            return null;
        }
    }
    public class RemoveSavedString : DHJassNativeFunction
    {
        public RemoveSavedString()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null)
                table.RemoveSavedString(
                    args["parentKey"].IntValue,
                    args["childKey"].IntValue);

            return null;
        }
    }
    public class RemoveSavedHandle : DHJassNativeFunction
    {
        public RemoveSavedHandle()
        {
            this.CreateArguments(new DHJassHandle("table", 0),
                                new DHJassInt("parentKey", 0),
                                new DHJassInt("childKey", 0));
        }
        protected override DHJassValue Run()
        {
            hashtable table = args["table"].HandleValue as hashtable;
            if (table != null)
                table.RemoveSavedHandle(
                    args["parentKey"].IntValue,
                    args["childKey"].IntValue);

            return null;
        }
    }

    public class Player : DHJassNativeFunction
    {
        public Player()
        {
            this.CreateArguments(new DHJassInt("number", 0));
        }

        protected override DHJassValue Run()
        {
            int handle = player.players[argsArray[0].IntValue].handle;
            return new DHJassHandle(null, handle);
        }
    }
    public class GetPlayerId : DHJassNativeFunction
    {
        public GetPlayerId()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, p.get_id());
        }
    }

    public class Location : DHJassNativeFunction
    {
        public Location()
        {
            this.CreateArguments(new DHJassReal("x", 0.0),
                                new DHJassReal("y", 0.0));
        }

        protected override DHJassValue Run()
        {
            location l = new location(args["x"].RealValue, args["y"].RealValue);
            return new DHJassHandle(null, l.handle);
        }
    }
    public class GetLocationX : DHJassNativeFunction
    {
        public GetLocationX()
        {
            this.CreateArguments(new DHJassHandle("whichLocation", 0));
        }

        protected override DHJassValue Run()
        {
            location l = args["whichLocation"].HandleValue as location;
            if (l == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, l.x);
        }
    }
    public class GetLocationY : DHJassNativeFunction
    {
        public GetLocationY()
        {
            this.CreateArguments(new DHJassHandle("whichLocation", 0));
        }

        protected override DHJassValue Run()
        {
            location l = args["whichLocation"].HandleValue as location;
            if (l == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, l.y);
        }
    }

    public class CreateUnit : DHJassNativeFunction
    {
        public CreateUnit()
        {
            this.CreateArguments(new DHJassHandle("player", 0),
                                   new DHJassInt("unitid", 0),
                                   new DHJassReal("x", 0.0),
                                   new DHJassReal("y", 0.0),
                                   new DHJassReal("face", 0.0)
                                   );
        }

        protected override DHJassValue Run()
        {
            player p = args["player"].HandleValue as player;
            if (p == null) return new DHJassAnyHandle(null, 0);//DHJassUnit(null, 0);
            else
            {
                unit u = p.add_unit(args["unitid"].IntValue);
                u.x = args["x"].RealValue;
                u.y = args["y"].RealValue;
                u.face = args["face"].RealValue;
                return new DHJassAnyHandle(null, u.handle);//DHJassUnit(null, u.handle);
            }
        }
    }
    public class CreateUnitAtLoc : DHJassNativeFunction
    {
        public CreateUnitAtLoc()
        {
            this.CreateArguments(new DHJassHandle("id", 0),
                                   new DHJassInt("unitid", 0),
                                   new DHJassHandle("whichLocation", 0),
                                   new DHJassReal("face", 0.0)
                                   );
        }

        protected override DHJassValue Run()
        {
            player p = args["id"].HandleValue as player;
            if (p == null) return new DHJassHandle(null, 0);

            location l = args["whichLocation"].HandleValue as location;
            if (l == null) return new DHJassHandle(null, 0);

            unit u = p.add_unit(args["unitid"].IntValue);

            u.face = args["face"].RealValue;
            u.set_location(l);

            return new DHJassHandle(null, u.handle);
        }
    }
    public class SetUnitX : DHJassNativeFunction
    {
        public SetUnitX()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassReal("newX", 0.0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            u.set_location(args["newX"].RealValue, u.y);

            return null;
        }
    }
    public class SetUnitY : DHJassNativeFunction
    {
        public SetUnitY()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassReal("newY", 0.0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            u.set_location(u.x, args["newY"].RealValue);

            return null;
        }
    }
    public class GetUnitX : DHJassNativeFunction
    {
        public GetUnitX()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, u.x);
        }
    }
    public class GetUnitY : DHJassNativeFunction
    {
        public GetUnitY()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, u.y);
        }
    }
    public class SetUnitPosition : DHJassNativeFunction
    {
        public SetUnitPosition()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassReal("newX", 0.0),
                                   new DHJassReal("newY", 0.0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            u.set_location(args["newX"].RealValue, args["newY"].RealValue);

            return null;
        }
    }
    public class GetUnitUserData : DHJassNativeFunction
    {
        public GetUnitUserData()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, u.userData);
        }
    }
    public class SetUnitUserData : DHJassNativeFunction
    {
        public SetUnitUserData()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                new DHJassInt("data", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null)
                u.userData = args["data"].IntValue;

            return null;
        }
    }
    public class KillUnit : DHJassNativeFunction
    {
        public KillUnit()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null) u.destroy();
            //DHJassHandleEngine.RemoveHandle(args["whichUnit"].IntValue);
            return null;
        }
    }
    public class RemoveUnit : DHJassNativeFunction
    {
        public RemoveUnit()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null) u.destroy();
            return null;
        }
    }

    public class AddUnitToStock : DHJassNativeFunction
    {
        public AddUnitToStock()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("unitId", 0),
                                   new DHJassInt("currentStock", 0),
                                   new DHJassInt("stockMax", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null)
            {
                string unitID = DHJassInt.int2id(args["unitId"].IntValue);
                u.sellunits.Add(unitID);
            }

            return null;
        }
    }
    public class RemoveUnitFromStock : DHJassNativeFunction
    {
        public RemoveUnitFromStock()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("unitId", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null)
            {
                string unitID = DHJassInt.int2id(args["unitId"].IntValue);
                u.sellunits.Remove(unitID);
            }

            return null;
        }
    }
    public class GetUnitTypeId : DHJassNativeFunction
    {
        public GetUnitTypeId()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, DHJassInt.id2int(u.codeID));
        }
    }
    public class UnitId : DHJassNativeFunction
    {
        public UnitId()
        {
            this.CreateArguments(new DHJassHandle("unitIdString", 0));
        }

        protected override DHJassValue Run()
        {
            return new DHJassInt(null, DHJassInt.id2int(args["unitIdString"].StringValue));
        }
    }
    public class IsUnitType : DHJassNativeFunction
    {
        public IsUnitType()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassHandle("whichUnitType", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            string unitType = unittype.getName(args["whichUnitType"].IntValue);

            bool result;
            switch (unitType)
            {
                // assume that only heroes have primary attributes
                case "UNIT_TYPE_HERO":
                    result = (PrimAttrType)u.primary != PrimAttrType.None;
                    break;

                case "UNIT_TYPE_MELEE_ATTACKER":
                    result = (AttackMethod)u.attackMethod == AttackMethod.Melee;
                    break;

                case "UNIT_TYPE_RANGED_ATTACKER":
                    result = (AttackMethod)u.attackMethod == AttackMethod.Range;
                    break;

                default:
                    result = false;
                    break;
            }

            return new DHJassBoolean(null, result);
        }
    }
    public class IsUnitIllusion : DHJassNativeFunction
    {
        public IsUnitIllusion()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            return new DHJassBoolean(null, u.isIllusion);
        }
    }
    public class IsUnitAlly : DHJassNativeFunction
    {
        public IsUnitAlly()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassBoolean(null, false);

            // use IsFriend instead of IsAlly, 
            // because it checks for (allies + this_player)
            return new DHJassBoolean(null, u.IsFriend(p));
        }
    }
    public class IsUnitEnemy : DHJassNativeFunction
    {
        public IsUnitEnemy()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassBoolean(null, false);

            return new DHJassBoolean(null, u.IsEnemy(p));
        }
    }

    public class CreateItem : DHJassNativeFunction
    {
        public CreateItem()
        {
            this.CreateArguments(new DHJassInt("itemid", 0),
                                   new DHJassReal("x", 0.0),
                                   new DHJassReal("y", 0.0)
                                   );
        }

        protected override DHJassValue Run()
        {
            item i = new item(args["itemid"].IntValue);
            i.set_location(args["x"].RealValue, args["y"].RealValue);
            return new DHJassHandle(null, i.handle);
        }
    }
    public class UnitAddItem : DHJassNativeFunction
    {
        public UnitAddItem()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassHandle("whichItem", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            item i = args["whichItem"].HandleValue as item;
            if (i == null) return new DHJassBoolean(null, false);

            return new DHJassBoolean(null, u.Inventory.put_item(i));
        }
    }
    public class UnitAddItemById : DHJassNativeFunction
    {
        public UnitAddItemById()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                    new DHJassInt("itemId", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassHandle(null, 0);
            
            if (args["itemId"].IntValue == 0)                            
                return new DHJassHandle(null, 0);            

            item i = new item(args["itemId"].IntValue);
            u.Inventory.put_item(i);

            return new DHJassHandle(null, i.handle);
        }
    }
    public class UnitAddItemToSlotById : DHJassNativeFunction
    {
        public UnitAddItemToSlotById()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("itemId", 0),
                                   new DHJassInt("itemSlot", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            item i = new item(args["itemId"].IntValue);

            return new DHJassBoolean(null,
                u.Inventory.put_item(i, args["itemSlot"].IntValue));
        }
    }
    public class UnitRemoveItem : DHJassNativeFunction
    {
        public UnitRemoveItem()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassHandle("whichItem", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            item i = args["whichItem"].HandleValue as item;
            if (i == null) return new DHJassBoolean(null, false);

            return new DHJassBoolean(null, u.Inventory.remove_item(i));
        }
    }
    public class GetItemTypeId : DHJassNativeFunction
    {
        public GetItemTypeId()
        {
            this.CreateArguments(new DHJassHandle("i", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["i"].HandleValue as item;
            if (i == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, DHJassInt.id2int(i.codeID));
        }
    }
    public class RemoveItem : DHJassNativeFunction
    {
        public static bool ForceNoDestroy = false;

        public RemoveItem()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0));
        }

        protected override DHJassValue Run()
        {
            handlevalue handle = args["whichItem"].HandleValue;
            if (handle != null)
            {
                if (ForceNoDestroy)
                    (handle as item).set_owner_ex(null);
                else 
                    handle.destroy();                
            }
            return null;
        }
    }
    public class GetItemType : DHJassNativeFunction
    {
        public GetItemType()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, itemtype.getValue(i.type));
        }
    }
    public class GetItemX : DHJassNativeFunction
    {
        public GetItemX()
        {
            this.CreateArguments(new DHJassHandle("i", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["i"].HandleValue as item;
            if (i == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, i.x);
        }
    }
    public class GetItemY : DHJassNativeFunction
    {
        public GetItemY()
        {
            this.CreateArguments(new DHJassHandle("i", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["i"].HandleValue as item;
            if (i == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, i.y);
        }
    }
    public class UnitItemInSlot : DHJassNativeFunction
    {
        public UnitItemInSlot()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("itemSlot", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassHandle(null, 0);

            item i = u.Inventory.itemAt(args["itemSlot"].IntValue);
            if (i == null) return new DHJassHandle(null, 0);

            return new DHJassHandle(null, i.handle);
        }
    }
    public class UnitInventorySize : DHJassNativeFunction
    {
        public static int ForceReturnValue = -1;

        public UnitInventorySize()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            if (ForceReturnValue != -1)
                return new DHJassInt(null, ForceReturnValue);

            return new DHJassInt(null, u.getInventorySize());
        }
    }
    public class SetItemDroppable : DHJassNativeFunction
    {
        public SetItemDroppable()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0),
                                 new DHJassBoolean("flag", false));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i == null) return null;

            i.droppable.Value = args["flag"].BoolValue;

            return null;
        }
    }
    public class GetItemCharges : DHJassNativeFunction
    {
        public GetItemCharges()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, i.uses);
        }
    }
    public class SetItemCharges : DHJassNativeFunction
    {
        public SetItemCharges()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0),
                new DHJassInt("charges", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i != null)
                i.set_charges(args["charges"].IntValue);

            return null;
        }
    }
    public class GetItemUserData : DHJassNativeFunction
    {
        public GetItemUserData()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, i.userData);
        }
    }
    public class SetItemUserData : DHJassNativeFunction
    {
        public SetItemUserData()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0),
                new DHJassInt("data", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i != null)
                i.userData = args["data"].IntValue;

            return null;
        }
    }
    public class GetItemPlayer : DHJassNativeFunction
    {
        public GetItemPlayer()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;            
            if (i == null) return new DHJassAnyHandle();

            return new DHJassHandle(null, i.get_owningPlayer().handle);
        }
    }
    public class SetItemPlayer : DHJassNativeFunction
    {
        public SetItemPlayer()
        {
            this.CreateArguments(new DHJassHandle("whichItem", 0),
                new DHJassHandle("whichPlayer", 0),
                new DHJassBoolean("changeColor", false));
        }

        protected override DHJassValue Run()
        {
            item i = args["whichItem"].HandleValue as item;
            if (i != null)
                i.set_owningPlayer(args["whichPlayer"].HandleValue as player);

            return null;
        }
    }
    public class GetEnumItem : DHJassNativeFunction
    {
        public GetEnumItem()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, DHJassExecutor.ItemStack.Peek().handle);
        }
    }
    public class GetFilterItem : DHJassNativeFunction
    {
        public GetFilterItem()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, DHJassExecutor.ItemStack.Peek().handle);
        }
    }
    public class EnumItemsInRect : DHJassNativeFunction
    {
        public EnumItemsInRect()
        {
            this.CreateArguments(new DHJassHandle("r", 0),                                
                                new DHJassHandle("filter", 0),
                                new DHJassCode("actionFunc", null));
        }

        protected override DHJassValue Run()
        {
            rect r = args["r"].HandleValue as rect;
            if (r == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            DHJassFunction action = args["actionFunc"].CodeValue;            

            item item;
            lock((DHJassHandleEngine.Values as ICollection).SyncRoot)
                foreach (handlevalue hv in DHJassHandleEngine.Values)
                    if (hv is item)
                    {
                        item = hv as item;

                        if (!r.ContainsXY(item.x, item.y))
                            continue;

                        DHJassExecutor.ItemStack.Push(item);
                        if ((be == null || be.GetResult()) && action != null) action.Execute();
                        DHJassExecutor.ItemStack.Pop();
                    }

            return null;
        }
    }

    public class GetUnitLoc : DHJassNativeFunction
    {
        public GetUnitLoc()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null)
                return new DHJassHandle(null, u.get_location().handle);
            else
                return new DHJassUnusedType();
        }
    }
    public class RemoveLocation : DHJassNativeFunction
    {
        public RemoveLocation()
        {
            this.CreateArguments(new DHJassHandle("whichLocation", 0));
        }

        protected override DHJassValue Run()
        {
            location l = args["whichLocation"].HandleValue as location;
            if (l != null) l.destroy();
            return null;
        }
    }

    public class CreateGroup : DHJassNativeFunction
    {
        public CreateGroup()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            group g = new group();
            return new DHJassHandle(null, g.handle);
        }
    }
    public class ForGroup : DHJassNativeFunction
    {
        public ForGroup()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassCode("callback", null));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            DHJassFunction callback = args["callback"].CodeValue;
            if (callback != null) group.runForeach(callback);

            return null;
        }
    }
    public class FirstOfGroup : DHJassNativeFunction
    {
        public FirstOfGroup()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return new DHJassHandle(null, 0);

            unit u = group.getFirst();
            if (u != null) return new DHJassHandle(null, u.handle);
            else return new DHJassHandle(null, 0);
        }
    }
    public class GroupAddUnit : DHJassNativeFunction
    {
        public GroupAddUnit()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            unit unit = args["whichUnit"].HandleValue as unit;
            if (unit == null) return null;

            group.addUnit(unit);

            return null;
        }
    }
    public class GroupRemoveUnit : DHJassNativeFunction
    {
        public GroupRemoveUnit()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            unit unit = args["whichUnit"].HandleValue as unit;
            if (unit == null) return null;

            group.removeUnit(unit);

            return null;
        }
    }
    public class GetEnumUnit : DHJassNativeFunction
    {
        public GetEnumUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, DHJassExecutor.GroupStack.Peek().handle);
        }
    }
    public class GetFilterUnit : DHJassNativeFunction
    {
        public GetFilterUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, DHJassExecutor.GroupStack.Peek().handle);
        }
    }
    public class GroupEnumUnitsOfPlayer : DHJassNativeFunction
    {
        public GroupEnumUnitsOfPlayer()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("filter", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            player player = args["whichPlayer"].HandleValue as player;
            if (player == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            group.enumUnitsOfPlayer(player, be);

            return null;
        }
    }
    public class GroupEnumUnitsInRange : DHJassNativeFunction
    {
        public GroupEnumUnitsInRange()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassReal("x", 0.0),
                                new DHJassReal("y", 0.0),
                                new DHJassReal("radius", 0.0),
                                new DHJassHandle("filter", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            group.enumUnitsInRange(
                args["x"].RealValue,
                args["y"].RealValue,
                args["radius"].RealValue,
                be);

            return null;
        }
    }
    public class GroupEnumUnitsInRect : DHJassNativeFunction
    {
        public GroupEnumUnitsInRect()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0),
                                new DHJassHandle("r", 0),
                                new DHJassHandle("filter", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group == null) return null;

            rect r = args["r"].HandleValue as rect;
            if (r == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            group.enumUnitsInRect(r, be);

            return null;
        }
    }
    public class GroupClear : DHJassNativeFunction
    {
        public GroupClear()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group != null) group.clear();
            return null;
        }
    }
    public class DestroyGroup : DHJassNativeFunction
    {
        public DestroyGroup()
        {
            this.CreateArguments(new DHJassHandle("whichGroup", 0));
        }

        protected override DHJassValue Run()
        {
            group group = args["whichGroup"].HandleValue as group;
            if (group != null) group.destroy();
            return null;
        }
    }

    public class CreateForce : DHJassNativeFunction
    {
        public CreateForce()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            force f = new force();
            return new DHJassHandle(null, f.handle);
        }
    }
    public class ForceAddPlayer : DHJassNativeFunction
    {
        public ForceAddPlayer()
        {
            this.CreateArguments(new DHJassHandle("whichForce", 0),
                                new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            force f = args["whichForce"].HandleValue as force;
            if (f == null) return null;

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            f.addPlayer(p);
            return null;
        }
    }
    public class ForForce : DHJassNativeFunction
    {
        public ForForce()
        {
            this.CreateArguments(new DHJassHandle("whichForce", 0),
                                new DHJassCode("callback", null));
        }

        protected override DHJassValue Run()
        {
            force force = args["whichForce"].HandleValue as force;
            if (force == null) return null;

            DHJassFunction callback = args["callback"].CodeValue;
            if (callback != null) force.runForeach(callback);

            return null;
        }
    }
    public class GetEnumPlayer : DHJassNativeFunction
    {
        public GetEnumPlayer()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, DHJassExecutor.ForceStack.Peek().handle);
        }
    }
    public class ForceEnumPlayers : DHJassNativeFunction
    {
        public ForceEnumPlayers()
        {
            this.CreateArguments(new DHJassHandle("whichForce", 0),
                                new DHJassHandle("filter", 0));
        }

        protected override DHJassValue Run()
        {
            force force = args["whichForce"].HandleValue as force;
            if (force == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            force.enumPlayers(be);

            return null;
        }
    }
    public class IsPlayerInForce : DHJassNativeFunction
    {
        public IsPlayerInForce()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("whichForce", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            force f = args["whichForce"].HandleValue as force;
            if (f == null) return null;

            return new DHJassBoolean(null, f.players.Contains(p));
        }
    }
    public class ForceEnumEnemies : DHJassNativeFunction
    {
        public ForceEnumEnemies()
        {
            this.CreateArguments(new DHJassHandle("whichForce", 0),
                                new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("filter", 0));
        }

        protected override DHJassValue Run()
        {
            force force = args["whichForce"].HandleValue as force;
            if (force == null) return null;

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            boolexpr be = args["filter"].HandleValue as boolexpr;

            force.enumEnemies(p, be);

            return null;
        }
    }

    public class GetLocalPlayer : DHJassNativeFunction
    {
        public GetLocalPlayer()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, Current.player.handle);
        }
    }
    public class GetPlayerName : DHJassNativeFunction
    {
        public GetPlayerName()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            string result;

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) result = "NoName";
            else result = String.IsNullOrEmpty(p.name) ? "Player" + p.get_id() : p.name;

            return new DHJassString(null, result);
        }
    }

    public class SetPlayerTeam : DHJassNativeFunction
    {
        public SetPlayerTeam()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("whichTeam", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p != null) p.team = args["whichTeam"].IntValue;

            return null;
        }
    }
    public class SetPlayerName : DHJassNativeFunction
    {
        public SetPlayerName()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassString("name", null));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p != null) p.name = args["name"].StringValue;

            return null;
        }
    }

    public class GetPlayerAlliance : DHJassNativeFunction
    {
        public GetPlayerAlliance()
        {
            this.CreateArguments(new DHJassHandle("sourcePlayer", 0),
                                new DHJassHandle("otherPlayer", 0),
                                new DHJassHandle("whichAllianceSetting", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["sourcePlayer"].HandleValue as player;
            if (p != null) 
                return new DHJassBoolean(null,
                    p.getAlliance(args["otherPlayer"].HandleValue as player, 
                                args["whichAllianceSetting"].IntValue));

            return new DHJassBoolean(null, false);
        }
    }
    public class SetPlayerAlliance : DHJassNativeFunction
    {
        public SetPlayerAlliance()
        {
            this.CreateArguments(new DHJassHandle("sourcePlayer", 0),
                                new DHJassHandle("otherPlayer", 0),
                                new DHJassHandle("whichAllianceSetting", 0),
                                new DHJassBoolean("value", false));
        }

        protected override DHJassValue Run()
        {
            player p = args["sourcePlayer"].HandleValue as player;
            if (p != null) p.setAlliance(
                args["otherPlayer"].HandleValue as player,
                args["whichAllianceSetting"].IntValue,
                args["value"].BoolValue);

            return null;
        }
    }    

    public class GetPlayerState : DHJassNativeFunction
    {
        public GetPlayerState()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("whichPlayerState", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassInt(null, 0);

            string stateName = playerstate.getName(args["whichPlayerState"].IntValue);

            switch (stateName)
            {
                case "PLAYER_STATE_RESOURCE_GOLD":
                    return new DHJassInt(null, p.Gold);

                case "PLAYER_STATE_RESOURCE_LUMBER":
                    return new DHJassInt(null, p.lumber);

                case "PLAYER_STATE_RESOURCE_FOOD_USED":
                    return new DHJassInt(null, p.foodUsed);

                case "PLAYER_STATE_OBSERVER":
                    return new DHJassInt(null, p.observer);

                case "PLAYER_STATE_GIVES_BOUNTY":
                    return new DHJassInt(null, p.givesBounty);

                case "PLAYER_STATE_GOLD_GATHERED":
                    return new DHJassInt(null, p.goldGathered);

                default:
                    Console.WriteLine("Unknown playerstate: '" + stateName + "'");
                    break;
            }

            return new DHJassInt(null, 0);
        }
    }
    public class SetPlayerState : DHJassNativeFunction
    {
        public SetPlayerState()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("whichPlayerState", 0),
                                new DHJassInt("value", 0)
                                );
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            string stateName = playerstate.getName(args["whichPlayerState"].IntValue);

            switch (stateName)
            {
                case "PLAYER_STATE_RESOURCE_GOLD":
                    p.Gold = args["value"].IntValue;
                    break;

                case "PLAYER_STATE_RESOURCE_LUMBER":
                    p.lumber = args["value"].IntValue;
                    break;

                case "PLAYER_STATE_RESOURCE_FOOD_USED":
                    p.foodUsed = args["value"].IntValue;
                    break;

                case "PLAYER_STATE_OBSERVER":
                    p.observer = args["value"].IntValue;
                    break;

                case "PLAYER_STATE_GIVES_BOUNTY":
                    p.givesBounty = args["value"].IntValue;
                    break;

                case "PLAYER_STATE_GOLD_GATHERED":
                    p.goldGathered = args["value"].IntValue;
                    break;

                default:
                    Console.WriteLine("Unknown playerstate: '" + stateName + "'");
                    break;
            }

            return null;
        }
    }

    public class SetHeroXP : DHJassNativeFunction
    {
        public SetHeroXP()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassInt("newXpVal", 0),
                                new DHJassBoolean("showEyeCandy", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return null;

            hero.xp = args["newXpVal"].IntValue;

            return null;
        }
    }
    public class GetHeroStr : DHJassNativeFunction
    {
        public GetHeroStr()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassBoolean("includeBonuses", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return new DHJassInt(null, 0);

            return new DHJassInt(null,
                (args["includeBonuses"].BoolValue) ?
                (int)hero.strength : (int)hero.get_naked_attr(PrimAttrType.Str));
        }
    }
    public class SetHeroStr : DHJassNativeFunction
    {
        public SetHeroStr()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassInt("newStr", 0),
                                new DHJassBoolean("permanent", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return null;

            hero.set_naked_attr(PrimAttrType.Str, args["newStr"].IntValue);
            hero.refresh();
            return null;
        }
    }
    public class GetHeroAgi : DHJassNativeFunction
    {
        public GetHeroAgi()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassBoolean("includeBonuses", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return new DHJassInt(null, 0);

            return new DHJassInt(null,
                (args["includeBonuses"].BoolValue) ?
                (int)hero.agility : (int)hero.get_naked_attr(PrimAttrType.Agi));
        }
    }
    public class SetHeroAgi : DHJassNativeFunction
    {
        public SetHeroAgi()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassInt("newAgi", 0),
                                new DHJassBoolean("permanent", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return null;

            hero.set_naked_attr(PrimAttrType.Agi, args["newAgi"].IntValue);
            hero.refresh();
            return null;
        }
    }
    public class GetHeroInt : DHJassNativeFunction
    {
        public GetHeroInt()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassBoolean("includeBonuses", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return new DHJassInt(null, 0);

            return new DHJassInt(null,
                (args["includeBonuses"].BoolValue) ?
                (int)hero.intelligence : (int)hero.get_naked_attr(PrimAttrType.Int));
        }
    }
    public class SetHeroInt : DHJassNativeFunction
    {
        public SetHeroInt()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),
                                new DHJassInt("newInt", 0),
                                new DHJassBoolean("permanent", false));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return null;

            hero.set_naked_attr(PrimAttrType.Int, args["newInt"].IntValue);
            hero.refresh();
            return null;
        }
    }

    public class GetHeroLevel : DHJassNativeFunction
    {
        public GetHeroLevel()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0));
        }

        protected override DHJassValue Run()
        {
            unit hero = args["whichHero"].HandleValue as unit;
            if (hero == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, hero.Level);
        }
    }

    public class ReviveHero : DHJassNativeFunction
    {
        public ReviveHero()
        {
            this.CreateArguments(new DHJassHandle("whichHero", 0),                                   
                                   new DHJassReal("x", 0.0),
                                   new DHJassReal("y", 0.0),
                                   new DHJassBoolean("doEyecandy", false)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichHero"].HandleValue as unit;
            Console.WriteLine("[DUMMY] Revive hero" + u.name);           

            return new DHJassBoolean(null, true);
        }
    }

    public class GetUnitState : DHJassNativeFunction
    {
        public GetUnitState()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassHandle("whichUnitState", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassReal(null, 0.0);

            string stateName = unitstate.getName(args["whichUnitState"].IntValue);

            switch (stateName)
            {
                case "UNIT_STATE_LIFE":
                    return new DHJassReal(null, u.hp);

                case "UNIT_STATE_MANA":
                    return new DHJassReal(null, u.max_mana); // 'max_mana' is temporary, should be 'mana'

                case "UNIT_STATE_MAX_LIFE":
                    return new DHJassReal(null, u.max_hp);

                default:
                    Console.WriteLine("Unknown unitstate: '" + stateName + "'");
                    break;
            }

            return new DHJassReal(null, 0.0);
        }
    }
    public class SetUnitState : DHJassNativeFunction
    {
        public SetUnitState()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassHandle("whichUnitState", 0),
                                new DHJassReal("newVal", 0.0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            string stateName = unitstate.getName(args["whichUnitState"].IntValue);

            double value = args["newVal"].RealValue;

            switch (stateName)
            {
                case "UNIT_STATE_LIFE":
                    u[u.hp] = value;
                    break;

                case "UNIT_STATE_MANA":
                    // temporary disable mana change
                    break;

                case "UNIT_STATE_MAX_LIFE":
                    u[u.max_hp] = value;
                    break;

                default:
                    Console.WriteLine("Unknown unitstate: '" + stateName + "'");
                    break;
            }

            return null;
        }
    }
    public class GetUnitName : DHJassNativeFunction
    {
        public GetUnitName()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            string result;

            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) result = "NoName";
            else result = u.ID;

            return new DHJassString(null, result);
        }
    }
    public class GetUnitPointValue : DHJassNativeFunction
    {
        public GetUnitPointValue()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, u.points);
        }
    }
    public class GetUnitPointValueByType : DHJassNativeFunction
    {
        public GetUnitPointValueByType()
        {
            this.CreateArguments(new DHJassInt("unitType", 0));
        }

        protected override DHJassValue Run()
        {
            string unitID = (args["unitType"] as DHJassInt).IDValue;
            HabProperties hpcUnit = DHMpqDatabase.UnitSlkDatabase["UnitData"].GetValue(unitID);
            if (hpcUnit == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, hpcUnit.GetIntValue("points"));
        }
    }
    public class GetUnitAbilityLevel : DHJassNativeFunction
    {
        public GetUnitAbilityLevel()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassInt("abilcode", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            int result = u.getAbilityLevel((args["abilcode"] as DHJassInt).IDValue);
            return new DHJassInt(null, result);
        }
    }
    public class SetUnitAbilityLevel : DHJassNativeFunction
    {
        public SetUnitAbilityLevel()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassInt("abilcode", 0),
                                new DHJassInt("level", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassInt(null, 0);

            int result = u.setAbilityLevel(
                (args["abilcode"] as DHJassInt).IDValue,
                args["level"].IntValue);

            return new DHJassInt(null, result);
        }
    }
    public class UnitAddAbility : DHJassNativeFunction
    {
        public UnitAddAbility()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("abilityId", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);            

            bool result = u.heroAbilities.AddEx((args["abilityId"] as DHJassInt).IDValue, 1);
            if (result) u.refresh();

            return new DHJassBoolean(null, result);
        }
    }
    public class UnitRemoveAbility : DHJassNativeFunction
    {
        public UnitRemoveAbility()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("abilityId", 0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);

            bool result = u.heroAbilities.Remove((args["abilityId"] as DHJassInt).IDValue);
            if (result) u.refresh();

            return new DHJassBoolean(null, result);
        }
    }
    public class UnitApplyTimedLife : DHJassNativeFunction
    {
        public UnitApplyTimedLife()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                   new DHJassInt("buffId", 0),
                                   new DHJassReal("duration", 0.0)
                                   );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;            

            u.ApplyTimedLife((args["buffId"] as DHJassInt).IDValue, args["duration"].RealValue);

            return null;
        }
    }
    public class SetUnitPathing : DHJassNativeFunction
    {
        public SetUnitPathing()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassBoolean("flag", false));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            u.pathing = args["flag"].BoolValue;

            return null;
        }
    }
    public class SetUnitInvulnerable : DHJassNativeFunction
    {
        public SetUnitInvulnerable()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassBoolean("flag", false));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            if (args["flag"].BoolValue)
            {
                if (!u.abilities.Contains("Avul"))
                    u.abilities.Add("Avul");
            }
            else
                u.abilities.Remove("Avul");

            return null;
        }
    }
    public class ShowUnit : DHJassNativeFunction
    {
        public ShowUnit()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassBoolean("show", false));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            u.isVisible = args["show"].BoolValue;

            return null;
        }
    }

    public class GetObjectName : DHJassNativeFunction
    {
        public GetObjectName()
        {
            this.CreateArguments(new DHJassInt("objectId", 0));
        }

        protected override DHJassValue Run()
        {
            string objectId = (args["objectId"] as DHJassInt).IDValue;
            string result = DHLOOKUP.hpcUnitProfiles.GetStringValue(objectId, "Name").Trim('"');

            return new DHJassString(null, result);
        }
    }

    public class GetPlayerController : DHJassNativeFunction
    {
        public GetPlayerController()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p != null) return new DHJassHandle(null, p.pController);

            return new DHJassHandle(null, 5); // MAP_CONTROL_NONE
        }
    }
    public class SetPlayerController : DHJassNativeFunction
    {
        public SetPlayerController()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassHandle("controlType", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p != null) p.pController = args["controlType"].IntValue;

            return null;
        }
    }

    public class GetPlayerTechCount : DHJassNativeFunction
    {
        public GetPlayerTechCount()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("techid", 0),
                                new DHJassBoolean("specificonly", false));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassInt(null, 0);

            return new DHJassInt(null, p.getTechCount(args["techid"].IntValue));
        }
    }
    public class GetPlayerTechMaxAllowed : DHJassNativeFunction
    {
        public GetPlayerTechMaxAllowed()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("techid", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            return new DHJassInt(null,
                p.getTechMaxAllowed(args["techid"].IntValue));
        }
    }
    public class SetPlayerTechMaxAllowed : DHJassNativeFunction
    {
        public SetPlayerTechMaxAllowed()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("techid", 0),
                                new DHJassInt("maximum", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            p.setTechMaxAllowed(
                args["techid"].IntValue,
                args["maximum"].IntValue);

            return null;
        }
    }
    public class GetPlayerTechResearched : DHJassNativeFunction
    {
        public GetPlayerTechResearched()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("techid", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            return new DHJassInt(null,
                p.getTechCount(args["techid"].IntValue));
        }
    }
    public class SetPlayerTechResearched : DHJassNativeFunction
    {
        public SetPlayerTechResearched()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("techid", 0),
                                new DHJassInt("setToLevel", 0));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            p.setTechResearched(
                args["techid"].IntValue,
                args["setToLevel"].IntValue);

            return null;
        }
    }
    public class SetPlayerAbilityAvailable : DHJassNativeFunction
    {
        public SetPlayerAbilityAvailable()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0),
                                new DHJassInt("abilid", 0),
                                new DHJassBoolean("avail", false));
        }

        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            p.setAbilityAvaiable(args["abilid"].IntValue, args["avail"].BoolValue);

            return null;
        }
    }

    public class GetFloatGameState : DHJassNativeFunction
    {
        public GetFloatGameState()
        {
            this.CreateArguments(new DHJassHandle("whichFloatGameState", 0));
        }

        protected override DHJassValue Run()
        {
            string stateName = gamestate.getName(args["whichFloatGameState"].IntValue);

            switch (stateName)
            {
                case "GAME_STATE_TIME_OF_DAY":
                    return new DHJassReal(null, 0);

                default:
                    Console.WriteLine("Unknown gamestate: '" + stateName + "'");
                    break;
            }

            return new DHJassReal(null, 0);
        }
    }

    public class SetUnitOwner : DHJassNativeFunction
    {
        public SetUnitOwner()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassHandle("whichPlayer", 0),
                                new DHJassBoolean("changeColor", false)
                                );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return null;

            u.set_owningPlayer(p);

            return null;
        }
    }

    public class GetPlayerSlotState : DHJassNativeFunction
    {
        public GetPlayerSlotState()
        {
            this.CreateArguments(new DHJassHandle("whichPlayer", 0));
        }
        protected override DHJassValue Run()
        {
            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassHandle(null, 0);

            if (p.playing)
                return DHJassExecutor.Globals["PLAYER_SLOT_STATE_PLAYING"];
            else
                return DHJassExecutor.Globals["PLAYER_SLOT_STATE_EMPTY"];
        }
    }

    public class Condition : DHJassNativeFunction
    {
        public Condition()
        {
            this.CreateArguments(new DHJassCode("func", null));
        }

        protected override DHJassValue Run()
        {
            boolexpr be = new boolexpr(args["func"].CodeValue);
            return new DHJassHandle(null, be.handle);
        }
    }
    public class Filter : DHJassNativeFunction
    {
        public Filter()
        {
            this.CreateArguments(new DHJassCode("func", null));
        }

        protected override DHJassValue Run()
        {
            boolexpr be = new boolexpr(args["func"].CodeValue);
            return new DHJassHandle(null, be.handle);
        }
    }

    public class CreateTrigger : DHJassNativeFunction
    {
        public CreateTrigger()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            trigger t = new trigger();
            return new DHJassHandle(null, t.handle);
        }
    }
    public class DestroyTrigger : DHJassNativeFunction
    {
        public DestroyTrigger()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null) t.destroy();
            return null;
        }
    }
    public class TriggerAddAction : DHJassNativeFunction
    {
        public TriggerAddAction()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassCode("actionFunc", null));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            triggeraction ta = new triggeraction(args["actionFunc"].CodeValue);
            t.addAction(ta);

            return new DHJassHandle(null, ta.handle);
        }
    }
    public class TriggerAddCondition : DHJassNativeFunction
    {
        public TriggerAddCondition()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("condition", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            triggercondition tc = new triggercondition(args["condition"].HandleValue as boolexpr);
            t.addCondition(tc);
            return new DHJassHandle(null, tc.handle);
        }
    }
    public class TriggerRegisterPlayerUnitEvent : DHJassNativeFunction
    {
        public TriggerRegisterPlayerUnitEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichPlayer", 0),
                                 new DHJassHandle("whichPlayerUnitEvent", 0),
                                 new DHJassHandle("filter", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassHandle(null, 0);

            int eventId = args["whichPlayerUnitEvent"].IntValue;
            string eventName = playerunitevent.getName(eventId);

            triggerevent te = new triggerevent(t, eventId);
            
            if (t.debugInfo == null || !t.debugInfo.Contains(eventName))
                t.debugInfo += (!string.IsNullOrEmpty(t.debugInfo)? ",":string.Empty) + eventName;

            switch (eventName)
            {
                case "EVENT_PLAYER_UNIT_DEATH":
                    p.unit_death += te.OnEvent;
                    break;

                case "EVENT_PLAYER_HERO_REVIVE_FINISH":
                    break;

                case "EVENT_PLAYER_UNIT_SELL":
                    p.unit_sell += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_SELL_ITEM":
                    p.unit_sell_item += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_PICKUP_ITEM":
                    p.unit_pickup_item += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_DROP_ITEM":
                    p.unit_drop_item += te.OnEvent;
                    break;
       
                case "EVENT_PLAYER_UNIT_USE_ITEM":
                    p.unit_use_item += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_SUMMON":
                    p.unit_summon += te.OnEvent;
                    break;

                case "EVENT_PLAYER_HERO_SKILL":
                    p.hero_skill += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_SPELL_CAST":
                    p.unit_spell_cast += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_SPELL_EFFECT":
                    p.unit_spell_effect += te.OnEvent;
                    break;

                case "EVENT_PLAYER_UNIT_ISSUED_ORDER":
                    p.unit_issued_order += te.OnEvent;
                    break;

                default:
                    if (DHJassExecutor.ShowUnknownEvents)
                        Console.WriteLine("Unknown playerunitevent: '" + eventName + "'");
                    break;
            }

            return new DHJassHandle(null, te.handle);
        }
    }
    public class TriggerRegisterUnitEvent : DHJassNativeFunction
    {
        public TriggerRegisterUnitEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichUnit", 0),
                                 new DHJassHandle("whichEvent", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassHandle(null, 0);

            int eventId = args["whichEvent"].IntValue;
            string eventName = unitevent.getName(eventId);

            triggerevent te = new triggerevent(t, eventId);

            switch (eventName)
            {
                case "EVENT_UNIT_DEATH":
                    u.death += te.OnEvent;
                    break;

                case "EVENT_UNIT_SELL_ITEM":
                    u.sell_item += te.OnEvent;
                    break;

                default:
                    if (DHJassExecutor.ShowUnknownEvents)
                        Console.WriteLine("Unknown unitevent: '" + eventName + "'");
                    break;
            }

            return new DHJassHandle(null, te.handle);
        }
    }
    public class TriggerRegisterGameStateEvent : DHJassNativeFunction
    {
        public TriggerRegisterGameStateEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichState", 0),
                                 new DHJassHandle("opcode", 0),
                                 new DHJassReal("limitval", 0.0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            // create game state command
            string stateName = gamestate.getName(args["whichState"].IntValue);
            DHJassGetGameStateCommand gs_cmd = new DHJassGetGameStateCommand(stateName);

            // create limitval command
            DHJassPassValueCommand lv_cmd = new DHJassPassValueCommand(args["limitval"]);

            // get opcode
            AnyOperation opcode = (AnyOperation)args["opcode"].IntValue;

            // create command for this event
            DHJassOperationCommand opcmd = new DHJassOperationCommand(gs_cmd, lv_cmd, opcode);

            triggercommandevent tce = new triggercommandevent(t, opcmd);
            gamestate.statechanged += tce.OnEvent;

            return new DHJassHandle(null, tce.handle);
        }
    }
    public class TriggerRegisterPlayerEvent : DHJassNativeFunction
    {
        public TriggerRegisterPlayerEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichPlayer", 0),
                                 new DHJassHandle("whichPlayerEvent", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassHandle(null, 0);

            int eventId = args["whichPlayerEvent"].IntValue;
            string eventName = playerevent.getName(eventId);

            triggerevent te = new triggerevent(t, eventId);

            switch (eventName)
            {
                case "EVENT_PLAYER_LEAVE":
                    break;

                default:
                    if (DHJassExecutor.ShowUnknownEvents)
                        Console.WriteLine("Unknown playerevent: '" + eventName + "'");
                    break;
            }

            return new DHJassHandle(null, te.handle);
        }
    }
    public class TriggerRegisterEnterRegion : DHJassNativeFunction
    {
        public TriggerRegisterEnterRegion()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichRegion", 0),
                                 new DHJassHandle("filter", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            region r = args["whichRegion"].HandleValue as region;
            if (r == null) return new DHJassHandle(null, 0);

            triggerregionevent tre = new triggerregionevent(t, r);
            unit.move += tre.OnEnterEvent;

            return new DHJassHandle(null, tre.handle);
        }
    }
    public class TriggerRegisterUnitInRange : DHJassNativeFunction
    {
        public TriggerRegisterUnitInRange()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichUnit", 0),
                                 new DHJassReal("range", 0),
                                 new DHJassHandle("filter", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassHandle(null, 0);

            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassHandle(null, 0);

            double range = args["range"].RealValue;
            range r = new range(u, range);

            triggerunitinrangeevent ture = new triggerunitinrangeevent(t, r);
            unit.move += ture.OnUnitInRangeEvent;

            return new DHJassHandle(null, ture.handle);
        }
    }
    public class TriggerExecute : DHJassNativeFunction
    {
        public TriggerExecute()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null) t.ExecuteAction();

            return null;
        }
    }
    public class TriggerEvaluate : DHJassNativeFunction
    {
        public TriggerEvaluate()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null) t.Execute();

            return new DHJassBoolean(null, false);
        }
    }
    public class GetTriggerExecCount : DHJassNativeFunction
    {
        public GetTriggerExecCount()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null)
                return new DHJassInt(null, t.ExecCount);
            else
            {
                Console.WriteLine("GetTriggerExecCount failed!");
                return new DHJassInt(null, 0);
            }
        }
    }
    public class GetTriggerEventId : DHJassNativeFunction
    {
        public GetTriggerEventId()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("eventId", out obj))
                return new DHJassHandle(null, (int)obj);
            else
            {
                Console.WriteLine("GetTriggerEventId failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetTriggerUnit : DHJassNativeFunction
    {
        public GetTriggerUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassHandle(null, (obj as unit).handle);
            else
            {
                Console.WriteLine("GetTriggerUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetTriggerPlayer : DHJassNativeFunction
    {
        public GetTriggerPlayer()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("player", out obj))
                return new DHJassHandle(null, (obj as player).handle);
            else
            {
                Console.WriteLine("GetTriggerPlayer failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetTriggeringTrigger : DHJassNativeFunction
    {
        public GetTriggeringTrigger()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("trigger", out obj))
                return new DHJassHandle(null, (obj as trigger).handle);
            else
            {
                Console.WriteLine("GetTriggeringTrigger failed!");
                return new DHJassHandle();
            }
        }
    }

    public class GetBuyingUnit : DHJassNativeFunction
    {
        public GetBuyingUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("buying", out obj))
                return new DHJassHandle(null, (obj as unit).handle);
            else
            {
                Console.WriteLine("GetBuyingUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetEnteringUnit : DHJassNativeFunction
    {
        public GetEnteringUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassHandle(null, (obj as unit).handle);
            else
            {
                Console.WriteLine("GetEnteringUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetDyingUnit : DHJassNativeFunction
    {
        public GetDyingUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassHandle(null, (obj as unit).handle);
            else
            {
                Console.WriteLine("GetDyingUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetKillingUnit : DHJassNativeFunction
    {
        public GetKillingUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("killing", out obj))
                return new DHJassHandle(null, (obj != null) ? (obj as unit).handle : 0);
            else
            {
                Console.WriteLine("GetKillingUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetSummonedUnit : DHJassNativeFunction
    {
        public GetSummonedUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassHandle(null, (obj != null) ? (obj as unit).handle : 0);
            else
            {
                Console.WriteLine("GetSummonedUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetSellingUnit : DHJassNativeFunction
    {
        public GetSellingUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("selling", out obj))
                return new DHJassHandle(null, (obj != null) ? (obj as unit).handle : 0);
            else
            {
                Console.WriteLine("GetSellingUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetSoldUnit : DHJassNativeFunction
    {
        public GetSoldUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassHandle(null, (obj as unit).handle);
            else
            {
                Console.WriteLine("GetSoldUnit failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetSoldItem : DHJassNativeFunction
    {
        public GetSoldItem()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("item", out obj))
                return new DHJassHandle(null, (obj as item).handle);
            else
            {
                Console.WriteLine("GetSoldItem failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetManipulatedItem : DHJassNativeFunction
    {
        public GetManipulatedItem()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("item", out obj))
                return new DHJassHandle(null, (obj as item).handle);
            else
            {
                Console.WriteLine("GetManipulatedItem failed!");
                return new DHJassHandle();
            }
        }
    }
    public class GetLearnedSkill : DHJassNativeFunction
    {
        public GetLearnedSkill()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("skill", out obj))
                return new DHJassInt(null, DHJassInt.id2int((obj as DBABILITY).Alias));
            else
            {
                Console.WriteLine("GetLearnedSkill failed!");
                return new DHJassInt();
            }
        }
    }
    public class GetLearningUnit : DHJassNativeFunction
    {
        public GetLearningUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassInt(null, (obj != null) ? (obj as unit).handle : 0);
            else
            {
                Console.WriteLine("GetLearningUnit failed!");
                return new DHJassHandle();
            }
        }
    }    

    public class GetOwningPlayer : DHJassNativeFunction
    {
        public GetOwningPlayer()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0));
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u != null)
                return new DHJassHandle(null, u.get_owningPlayer().handle);

            return new DHJassHandle(null, 0);
        }
    }

    public class GetSpellAbilityId : DHJassNativeFunction
    {
        public GetSpellAbilityId()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("spell", out obj))
                return new DHJassInt(null, DHJassInt.id2int(obj as string));
            else
            {
                Console.WriteLine("GetSpellAbilityId failed!");
                return new DHJassInt(null, 0);
            }
        }
    }
    public class GetSpellTargetUnit : DHJassNativeFunction
    {
        public GetSpellTargetUnit()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("unit", out obj))
                return new DHJassInt(null, (obj != null) ? (obj as unit).handle : 0);
            else
            {
                Console.WriteLine("GetSpellTargetUnit failed!");
                return new DHJassHandle();
            }
        }
    }

    public class GetIssuedOrderId : DHJassNativeFunction
    {
        public GetIssuedOrderId()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("order", out obj))
                return new DHJassInt(null, (obj is OrderID) ? (int)(OrderID)obj : 0);
            else
            {
                Console.WriteLine("GetIssuedOrderId failed!");
                return new DHJassInt(null, 0);
            }
        }
    }
    public class OrderId : DHJassNativeFunction
    {
        public OrderId()
        {
            this.CreateArguments(new DHJassString("orderIdString", null));
        }

        protected override DHJassValue Run()
        {
            return new DHJassInt(null, (int)orderid.Parse(args["orderIdString"].StringValue));
        }
    }    

    public class CreateTimer : DHJassNativeFunction
    {
        public CreateTimer()
        {
        }

        protected override DHJassValue Run()
        {
            timer t = new timer();
            return new DHJassHandle(null, t.handle);
        }
    }
    public class TimerStart : DHJassNativeFunction
    {
        public TimerStart()
        {
            this.CreateArguments(new DHJassHandle("whichTimer", 0),
                                 new DHJassReal("timeout", 0.0),
                                 new DHJassBoolean("periodic", false),
                                 new DHJassCode("handlerFunc", null)
                                 );
        }

        protected override DHJassValue Run()
        {
            timer timer = args["whichTimer"].HandleValue as timer;
            if (timer == null) return null;            

            DHJassFunction function = args["handlerFunc"].CodeValue;

            if (function != null) 
                timer.SetCallback(function.Callback);

            timer.start(args["timeout"].RealValue, args["periodic"].BoolValue);
            return null;
        }
    }
    public class TimerGetElapsed : DHJassNativeFunction
    {
        public TimerGetElapsed()
        {
            this.CreateArguments(new DHJassHandle("whichTimer", 0));
        }

        protected override DHJassValue Run()
        {
            timer timer = args["whichTimer"].HandleValue as timer;
            if (timer == null) return new DHJassReal(null, 0);

            return new DHJassReal(null, timer.Elapsed);
        }
    }
    public class TimerGetRemaining : DHJassNativeFunction
    {
        public TimerGetRemaining()
        {
            this.CreateArguments(new DHJassHandle("whichTimer", 0));
        }

        protected override DHJassValue Run()
        {
            timer timer = args["whichTimer"].HandleValue as timer;
            if (timer == null) return new DHJassReal(null, 0);

            return new DHJassReal(null, timer.Remaining);
        }
    }
    public class DestroyTimer : DHJassNativeFunction
    {
        public DestroyTimer()
        {
            this.CreateArguments(new DHJassHandle("whichTimer", 0));
        }

        protected override DHJassValue Run()
        {
            timer t = args["whichTimer"].HandleValue as timer;
            if (t != null) t.destroy();
            return null;
        }
    }

    public class TriggerRegisterTimerEvent : DHJassNativeFunction
    {
        public TriggerRegisterTimerEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassReal("timeout", 0.0),
                                 new DHJassBoolean("periodic", false)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger trigger = args["whichTrigger"].HandleValue as trigger;
            if (trigger == null) return new DHJassHandle(null, 0);

            timer timer = new timer();

            triggerevent tevent = new triggerevent(trigger, timer);
            timer.SetCallback(tevent.OnEvent);

            timer.start(args["timeout"].RealValue, args["periodic"].BoolValue);

            return new DHJassHandle(null, tevent.handle);
        }
    }
    public class TriggerRegisterTimerExpireEvent : DHJassNativeFunction
    {
        public TriggerRegisterTimerExpireEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("timer", 0)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger trigger = args["whichTrigger"].HandleValue as trigger;
            if (trigger == null) return new DHJassHandle(null, 0);

            timer timer = args["timer"].HandleValue as timer;
            if (timer == null) return new DHJassHandle(null, 0);

            triggerevent tevent = new triggerevent(trigger);
            timer.SetCallback(tevent.OnEvent);

            return new DHJassHandle(null, tevent.handle);
        }
    }
    public class GetExpiredTimer : DHJassNativeFunction
    {
        public GetExpiredTimer()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("timer", out obj))
                return new DHJassHandle(null, (obj as timer).handle);
            else
            {
                Console.WriteLine("GetExpiredTimer failed!");
                return new DHJassHandle();
            }
        }
    }

    public class ChatEventHandler
    {
        string chatString;
        bool exactMatchOnly;
        public DHJassEventHandler match;

        public ChatEventHandler(string chatString, bool exactMatchOnly)
        {
            this.chatString = chatString;
            this.exactMatchOnly = exactMatchOnly;
        }
        public void OnChat(object sender, DHJassEventArgs e)
        {
            string s = e.args["chatString"] as string;
            if (exactMatchOnly)
            {
                if (chatString == s)
                    OnMatch(e);
            }
            else
                if (s.Contains(chatString))
                    OnMatch(e);
        }
        protected void OnMatch(DHJassEventArgs e)
        {
            if (match != null)
                match(null, e);
        }
    }
    public class TriggerRegisterPlayerChatEvent : DHJassNativeFunction
    {
        public TriggerRegisterPlayerChatEvent()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0),
                                 new DHJassHandle("whichPlayer", 0),
                                 new DHJassString("chatMessageToDetect", null),
                                 new DHJassBoolean("exactMatchOnly", false)
                                 );
        }

        protected override DHJassValue Run()
        {
            trigger trigger = args["whichTrigger"].HandleValue as trigger;
            if (trigger == null) return new DHJassHandle(null, 0);

            player p = args["whichPlayer"].HandleValue as player;
            if (p == null) return new DHJassHandle(null, 0);

            ChatEventHandler ceh = new ChatEventHandler(args["chatMessageToDetect"].StringValue,
                                                        args["exactMatchOnly"].BoolValue);

            triggerevent tevent = new triggerevent(trigger, ceh);
            p.chat += ceh.OnChat;
            ceh.match += tevent.OnEvent;

            return new DHJassHandle(null, tevent.handle);
        }
    }

    public class DisplayTimedTextToPlayer : DHJassNativeFunction
    {
        public DisplayTimedTextToPlayer()
        {
            this.CreateArguments(new DHJassHandle("player", 0),
                                new DHJassInt("x", 0),
                                new DHJassInt("y", 0),
                                new DHJassInt("duration", 0),
                                new DHJassString("message", null));
        }

        protected override DHJassValue Run()
        {
            player p = args["player"].HandleValue as player;
            if (p == null) return null;

            p.OnMessage(args["message"].StringValue);

            return null;
        }
    }
    public class GetEventPlayerChatString : DHJassNativeFunction
    {
        public GetEventPlayerChatString()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Dictionary<string, object> dc = DHJassExecutor.TriggerStack.Peek();
            object obj;
            if (dc.TryGetValue("chatString", out obj))
                return new DHJassString(null, obj.ToString());
            else
            {
                Console.WriteLine("GetEventPlayerChatString failed!");
                return new DHJassString();
            }
        }
    }

    public class ExecuteFunc : DHJassNativeFunction
    {
        public ExecuteFunc()
        {
            this.CreateArguments(new DHJassString("funcName", null));
        }

        protected override DHJassValue Run()
        {
            DHJassValue returned;
            DHJassExecutor.TryCallFunction(args["funcName"].StringValue, out returned);
            return null;
        }
    }

    public class StringLength : DHJassNativeFunction
    {
        public StringLength()
        {
            this.CreateArguments(new DHJassString("s", null));
        }

        protected override DHJassValue Run()
        {
            return new DHJassInt(null, args["s"].StringValue.Length);
        }
    }
    public class SubString : DHJassNativeFunction
    {
        public SubString()
        {
            this.CreateArguments(new DHJassString("source", null),
                                new DHJassInt("start", 0),
                                new DHJassInt("end", 0));
        }

        protected override DHJassValue Run()
        {
            int src_length = args["source"].StringValue.Length;

            int start = args["start"].IntValue;
            int length = Math.Min(args["end"].IntValue, src_length) - start;            

            return new DHJassString(null,
                (length >= 0) ? args["source"].StringValue.Substring(start, length) : "");
        }
    }
    public class StringCase : DHJassNativeFunction
    {
        public StringCase()
        {
            this.CreateArguments(new DHJassString("source", null),
                                new DHJassBoolean("upper", false));
        }

        protected override DHJassValue Run()
        {
            string result = (args["upper"].BoolValue) ? args["source"].StringValue.ToUpper() : args["source"].StringValue.ToLower();

            return new DHJassString(null, result);
        }
    }
    public class StringHash : DHJassNativeFunction
    {
        public StringHash()
        {
            this.CreateArguments(new DHJassString("s", null));
        }

        protected override DHJassValue Run()
        {
            int result = args["s"].StringValue.GetHashCode();

            return new DHJassInt(null, result);
        }
    }

    public class EnableTrigger : DHJassNativeFunction
    {
        public EnableTrigger()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null) t.enable();
            return null;
        }
    }
    public class DisableTrigger : DHJassNativeFunction
    {
        public DisableTrigger()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t != null) t.disable();
            return null;
        }
    }
    public class IsTriggerEnabled : DHJassNativeFunction
    {
        public IsTriggerEnabled()
        {
            this.CreateArguments(new DHJassHandle("whichTrigger", 0));
        }

        protected override DHJassValue Run()
        {
            trigger t = args["whichTrigger"].HandleValue as trigger;
            if (t == null) return new DHJassBoolean(null, false);

            return new DHJassBoolean(null, t.Enabled);
        }
    }

    public class GetGameSpeed : DHJassNativeFunction
    {
        public GetGameSpeed()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            return new DHJassHandle(null, 1);
        }
    }

    public class GetWidgetLife : DHJassNativeFunction
    {
        public GetWidgetLife()
        {
            this.CreateArguments(new DHJassHandle("whichWidget", 0));
        }

        protected override DHJassValue Run()
        {
            widget w = args["whichWidget"].HandleValue as widget;
            if (w == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, w.GetLife());
        }
    }
    public class SetWidgetLife : DHJassNativeFunction
    {
        public SetWidgetLife()
        {
            this.CreateArguments(new DHJassHandle("whichWidget", 0),
                                new DHJassReal("newLife", 0.0));
        }

        protected override DHJassValue Run()
        {
            widget w = args["whichWidget"].HandleValue as widget;
            if (w == null) return null;

            w.SetLife(args["newLife"].RealValue);
            return null;
        }
    }

    public class GetRandomInt : DHJassNativeFunction
    {
        public GetRandomInt()
        {
            this.CreateArguments(new DHJassInt("lowBound", 0),
                                new DHJassInt("highBound", 0));
        }

        protected override DHJassValue Run()
        {
            return new DHJassInt(null,
                DHJassExecutor.randomizer.Next(args["lowBound"].IntValue, args["highBound"].IntValue));
        }
    }
    public class GetRandomReal : DHJassNativeFunction
    {
        public GetRandomReal()
        {
            this.CreateArguments(new DHJassReal("lowBound", 0.0),
                                new DHJassReal("highBound", 0.0));
        }

        protected override DHJassValue Run()
        {
            double randomRange = args["highBound"].RealValue - args["lowBound"].RealValue;

            randomRange *= DHJassExecutor.randomizer.NextDouble();

            return new DHJassReal(null, args["lowBound"].RealValue + randomRange);
        }
    }
    public class Cos : DHJassNativeFunction
    {
        public Cos()
        {
            this.CreateArguments(new DHJassReal("radians", 0.0));
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Math.Cos(args["radians"].RealValue));
        }
    }
    public class Sin : DHJassNativeFunction
    {
        public Sin()
        {
            this.CreateArguments(new DHJassReal("radians", 0.0));
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Math.Sin(args["radians"].RealValue));
        }
    }

    public class Rect : DHJassNativeFunction
    {
        public Rect()
        {
            this.CreateArguments(new DHJassReal("minx", 0.0),
                                 new DHJassReal("miny", 0.0),
                                 new DHJassReal("maxx", 0.0),
                                 new DHJassReal("maxy", 0.0)
                                 );
        }
        protected override DHJassValue Run()
        {
            rect r = new rect(
                args["minx"].RealValue,
                args["miny"].RealValue,
                args["maxx"].RealValue,
                args["maxy"].RealValue);

            return new DHJassHandle(null, r.handle);
        }
    }
    public class GetRectMinX : DHJassNativeFunction
    {
        public GetRectMinX()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.MinX);
        }
    }
    public class GetRectMaxX : DHJassNativeFunction
    {
        public GetRectMaxX()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.MaxX);
        }
    }
    public class GetRectMinY : DHJassNativeFunction
    {
        public GetRectMinY()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.MinY);
        }
    }
    public class GetRectMaxY : DHJassNativeFunction
    {
        public GetRectMaxY()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.MaxY);
        }
    }
    public class GetRectCenterX : DHJassNativeFunction
    {
        public GetRectCenterX()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.GetCenterX());
        }
    }
    public class GetRectCenterY : DHJassNativeFunction
    {
        public GetRectCenterY()
        {
            this.CreateArguments(new DHJassHandle("whichRect", 0));
        }

        protected override DHJassValue Run()
        {
            rect r = args["whichRect"].HandleValue as rect;
            if (r == null) return new DHJassReal(null, 0.0);

            return new DHJassReal(null, r.GetCenterY());
        }
    }

    public class GetWorldBounds : DHJassNativeFunction
    {
        public GetWorldBounds()
        {
        }

        protected override DHJassValue Run()
        {
            rect r = new rect(-500, -500, 500, 500);// 1000x1000                        
            return new DHJassHandle(null, r.handle);
        }
    }

    public class GetCameraBoundMinX : DHJassNativeFunction
    {
        public GetCameraBoundMinX()
        {
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Constants.map.minX);
        }
    }
    public class GetCameraBoundMinY : DHJassNativeFunction
    {
        public GetCameraBoundMinY()
        {
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Constants.map.minY);
        }
    }
    public class GetCameraBoundMaxX : DHJassNativeFunction
    {
        public GetCameraBoundMaxX()
        {
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Constants.map.maxX);
        }
    }
    public class GetCameraBoundMaxY : DHJassNativeFunction
    {
        public GetCameraBoundMaxY()
        {
        }

        protected override DHJassValue Run()
        {
            return new DHJassReal(null, Constants.map.maxY);
        }
    }

    public class CreateRegion : DHJassNativeFunction
    {
        public CreateRegion()
        {
        }
        protected override DHJassValue Run()
        {
            region r = new region();
            return new DHJassHandle(null, r.handle);
        }
    }
    public class RegionAddRect : DHJassNativeFunction
    {
        public RegionAddRect()
        {
            this.CreateArguments(new DHJassHandle("whichRegion", 0),
                                 new DHJassHandle("r", 0)
                                 );
        }
        protected override DHJassValue Run()
        {
            region region = args["whichRegion"].HandleValue as region;
            if (region == null) return null;

            rect r = args["r"].HandleValue as rect;
            if (r == null) return null;

            region.addRect(r);

            return null;
        }
    }
    public class RemoveRegion : DHJassNativeFunction
    {
        public RemoveRegion()
        {
            this.CreateArguments(new DHJassHandle("whichRegion", 0));
        }

        protected override DHJassValue Run()
        {
            region r = args["whichRegion"].HandleValue as region;
            if (r != null) r.destroy();
            return null;
        }
    }
    public class IsUnitInRegion : DHJassNativeFunction
    {
        public IsUnitInRegion()
        {
            this.CreateArguments(new DHJassHandle("whichRegion", 0),
                                 new DHJassHandle("whichUnit", 0)
                                 );
        }
        protected override DHJassValue Run()
        {
            region region = args["whichRegion"].HandleValue as region;
            if (region == null) return new DHJassBoolean(null, false);

            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return new DHJassBoolean(null, false);            

            return new DHJassBoolean(null, region.ContainsXY(u.x, u.y));
        }
    }

    public class SelectUnit : DHJassNativeFunction
    {
        public SelectUnit()
        {
            this.CreateArguments(new DHJassHandle("whichUnit", 0),
                                new DHJassBoolean("flag", false)
                                );
        }

        protected override DHJassValue Run()
        {
            unit u = args["whichUnit"].HandleValue as unit;
            if (u == null) return null;

            if (args["flag"].BoolValue)
            {
                if (!Current.player.selection.Contains(u))
                    Current.player.selection.Add(u);
            }
            else
                Current.player.selection.Remove(u);

            return null;
        }
    }
    public class ClearSelection : DHJassNativeFunction
    {
        public ClearSelection()
        {
            this.CreateArguments();
        }

        protected override DHJassValue Run()
        {
            Current.player.selection.Clear();
            return null;
        }
    }
}
