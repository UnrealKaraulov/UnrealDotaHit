using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ExpTreeLib;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Deerchao.War3Share.W3gParser;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Jass.Native.Types;
using BitmapUtils;

namespace DotaHIT.Extras
{
    public static class ReplayParserCore
    {
        public static readonly string CachePath = Application.StartupPath + "\\" + "Cache" + "\\" + "ReplayParser";

        public static string GetProperMapPath(string mapPath)
        {
            string path = DHCFG.Items["Path"].GetStringValue("War3") + "\\" + mapPath;
            if (File.Exists(path)) return path;

            // try default folder
            string defaultPath = DHCFG.Items["Path"].GetStringValue("War3") + "\\Maps\\Download\\" + Path.GetFileName(mapPath);
            if (File.Exists(defaultPath)) return defaultPath;

            return path;
        }

        public static void ParseLineups(Replay replay)
        {
            if (Current.map == null && replay.MapCache.IsLoaded == false) return;

            rect bottomRect;
            rect middleRect;
            rect topRect;
            rect sentinelBase;
            rect scourgeBase;

            HabPropertiesCollection hpcMapData = replay.MapCache.hpcMapData;

            if (replay.MapCache.IsLoaded == false)
            {
                if (Current.map == null) return;

                player sentinel = player.players[0];
                player scourge = player.players[6];

                unit sentinelTopTower = null;
                unit sentinelMidTower = null;
                unit sentinelBotTower = null;

                unit scourgeTopTower = null;
                unit scourgeMidTower = null;
                unit scourgeBotTower = null;

                // Sentinel Towers (Treant Protectors):
                // 
                // e00R - level1
                // e011 - level2
                // e00S - level3
                // e019 - level4             

                List<unit> sentinelLevel1Towers = new List<unit>(3);
                List<unit> sentinelLevel3Towers = new List<unit>(3);
                List<unit> unitList = new List<unit>(sentinel.units.Values);
                foreach (unit u in unitList)
                    switch (u.codeID.Text)
                    {
                        case "e00R": sentinelLevel1Towers.Add(u);
                            break;

                        case "e00S": sentinelLevel3Towers.Add(u);
                            break;
                    }

                if (sentinelLevel1Towers.Count == 0)
                    return;

                sentinelTopTower = GetTowerForLineUp(sentinelLevel1Towers, LineUp.Top, true);
                sentinelMidTower = GetTowerForLineUp(sentinelLevel1Towers, LineUp.Middle, true);
                sentinelBotTower = GetTowerForLineUp(sentinelLevel1Towers, LineUp.Bottom, true);

                // Scourge Towers (Spirit Towers):
                //
                // u00M - level1
                // u00D - level2
                // u00N - level3
                // u00T - level4

                List<unit> scourgeLevel1Towers = new List<unit>(3);
                List<unit> scourgeLevel3Towers = new List<unit>(3);
                unitList = new List<unit>(scourge.units.Values);
                foreach (unit u in unitList)
                    switch (u.codeID.Text)
                    {
                        case "u00M": scourgeLevel1Towers.Add(u);
                            break;

                        case "u00N": scourgeLevel3Towers.Add(u);
                            break;
                    }

                if (scourgeLevel1Towers.Count == 0)
                    return;

                scourgeTopTower = GetTowerForLineUp(scourgeLevel1Towers, LineUp.Top, false);
                scourgeMidTower = GetTowerForLineUp(scourgeLevel1Towers, LineUp.Middle, false);
                scourgeBotTower = GetTowerForLineUp(scourgeLevel1Towers, LineUp.Bottom, false);

                // create rectangles that will be used to determine the player's lineup

                bottomRect = new rect(
                    Math.Min(sentinelBotTower.x, scourgeBotTower.x), sentinelBotTower.y,
                    Jass.Native.Constants.map.maxX, scourgeBotTower.y);

                // middle rectange will be calculated 
                // as the rectange with Width = distance between middle towers
                // and Height = 990 (approximate value that should work fine for middle rectangle)
                // 990 ~== square_root(700^2 + 700^2), so X offset = 700 and Y offset = 700
                middleRect = new rect(
                    sentinelMidTower.x - 700, sentinelMidTower.y,
                    scourgeMidTower.x, scourgeMidTower.y + 700);

                topRect = new rect(
                    Jass.Native.Constants.map.minX, sentinelTopTower.y,
                    Math.Max(scourgeTopTower.x, sentinelTopTower.x), scourgeTopTower.y);

                // create rectangles that will be used to determine the base area for each team
                // rectangle's vertices calculation will be based on coordinates of level3 towers

                sentinelBase = GetBaseRectFromTowers(sentinelLevel3Towers, true);
                scourgeBase = GetBaseRectFromTowers(scourgeLevel3Towers, false);

                // write values to cache
                if (DHCFG.Items["Extra"].GetHpcValue("ReplayParser", true).GetIntValue("Map", "UseCache", 1) == 1)
                {
                    hpcMapData["Lanes", "Bottom"] = new List<string>(new string[] { bottomRect.MinX.ToString(), bottomRect.MinY.ToString(), bottomRect.MaxX.ToString(), bottomRect.MaxY.ToString() });
                    hpcMapData["Lanes", "Middle"] = new List<string>(new string[] { middleRect.MinX.ToString(), middleRect.MinY.ToString(), middleRect.MaxX.ToString(), middleRect.MaxY.ToString() });
                    hpcMapData["Lanes", "Top"] = new List<string>(new string[] { topRect.MinX.ToString(), topRect.MinY.ToString(), topRect.MaxX.ToString(), topRect.MaxY.ToString() });
                    hpcMapData["Bases", "Sentinel"] = new List<string>(new string[] { sentinelBase.MinX.ToString(), sentinelBase.MinY.ToString(), sentinelBase.MaxX.ToString(), sentinelBase.MaxY.ToString() });
                    hpcMapData["Bases", "Scourge"] = new List<string>(new string[] { scourgeBase.MinX.ToString(), scourgeBase.MinY.ToString(), scourgeBase.MaxX.ToString(), scourgeBase.MaxY.ToString() });
                }
            }
            else
            {
                List<string> list = hpcMapData.GetStringListValue("Lanes", "Bottom");
                if (list.Count == 0)
                    return;

                bottomRect = new rect(Convert.ToDouble(list[0]), Convert.ToDouble(list[1]), Convert.ToDouble(list[2]), Convert.ToDouble(list[3]));

                list = hpcMapData.GetStringListValue("Lanes", "Middle");
                middleRect = new rect(Convert.ToDouble(list[0]), Convert.ToDouble(list[1]), Convert.ToDouble(list[2]), Convert.ToDouble(list[3]));

                list = hpcMapData.GetStringListValue("Lanes", "Top");
                topRect = new rect(Convert.ToDouble(list[0]), Convert.ToDouble(list[1]), Convert.ToDouble(list[2]), Convert.ToDouble(list[3]));

                list = hpcMapData.GetStringListValue("Bases", "Sentinel");
                sentinelBase = new rect(Convert.ToDouble(list[0]), Convert.ToDouble(list[1]), Convert.ToDouble(list[2]), Convert.ToDouble(list[3]));

                list = hpcMapData.GetStringListValue("Bases", "Scourge");
                scourgeBase = new rect(Convert.ToDouble(list[0]), Convert.ToDouble(list[1]), Convert.ToDouble(list[2]), Convert.ToDouble(list[3]));
            }

            setLineUpsForPlayers(replay, topRect, middleRect, bottomRect, sentinelBase, scourgeBase);
        }

        static void setLineUpsForPlayers(Replay rp, rect top, rect mid, rect bot, rect sentinelBase, rect scourgeBase)
        {
            // approximate creep spawn time
            int creepSpawnTime = GetFirstCreepAttackTime(rp, top, mid, bot);

            foreach (Team team in rp.Teams)
                switch (team.Type)
                {
                    case TeamType.Sentinel:
                    case TeamType.Scourge:
                        List<int> alliedHeroes = GetListOfAlliedHeroes(team);
                        foreach (Player p in team.Players)
                        {
                            location l = GetAverageLocation(p, (team.Type == TeamType.Sentinel ? sentinelBase : scourgeBase), alliedHeroes, creepSpawnTime);

                            p.LineUpLocation = l;

                            if (top.ContainsXY(l.x, l.y))
                                p.LineUp = LineUp.Top;
                            else
                                if (mid.ContainsXY(l.x, l.y))
                                    p.LineUp = LineUp.Middle;
                                else
                                    if (bot.ContainsXY(l.x, l.y))
                                        p.LineUp = LineUp.Bottom;
                                    else
                                        p.LineUp = LineUp.JungleOrRoaming;
                        }
                        break;
                }
        }
        public static unit GetTowerForLineUp(List<unit> towers, LineUp lineUp, bool isSentinel)
        {
            unit selected = null;

            switch (lineUp)
            {
                case LineUp.Top:
                    if (isSentinel)
                    {
                        // top-most for sentinel
                        foreach (unit tower in towers)
                            if (selected == null || selected.y < tower.y)
                                selected = tower;
                    }
                    else// left-most for scourge
                        foreach (unit tower in towers)
                            if (selected == null || selected.x > tower.x)
                                selected = tower;
                    break;

                case LineUp.Middle:
                    // calculating hypotenuse as the measure 
                    // of the nearest tower to map center
                    foreach (unit tower in towers)
                        if (selected == null ||
                            ((selected.y * selected.y) + (selected.x * selected.x) > (tower.y * tower.y) + (tower.x * tower.x)))
                            selected = tower;
                    break;

                case LineUp.Bottom:
                    if (isSentinel)
                    {
                        // right-most for sentinel
                        foreach (unit tower in towers)
                            if (selected == null || selected.x < tower.x)
                                selected = tower;
                    }
                    else// bottom-most for scourge
                        foreach (unit tower in towers)
                            if (selected == null || selected.y > tower.y)
                                selected = tower;
                    break;
            }

            return selected;
        }
        public static rect GetBaseRectFromTowers(List<unit> towers, bool isSentinel)
        {
            double maxx;
            double maxy;

            if (isSentinel)
            {
                maxx = Jass.Native.Constants.map.minX;
                maxy = Jass.Native.Constants.map.minY;
                foreach (unit tower in towers)
                {
                    if (maxx < tower.x)
                        maxx = tower.x;

                    if (maxy < tower.y)
                        maxy = tower.y;
                }

                return new rect(
                    Jass.Native.Constants.map.minX, Jass.Native.Constants.map.minY,
                    maxx, maxy);
            }
            else
            {
                maxx = Jass.Native.Constants.map.maxX;
                maxy = Jass.Native.Constants.map.maxY;
                foreach (unit tower in towers)
                {
                    if (maxx > tower.x)
                        maxx = tower.x;

                    if (maxy > tower.y)
                        maxy = tower.y;
                }

                return new rect(
                    maxx, maxy,
                    Jass.Native.Constants.map.maxX, Jass.Native.Constants.map.maxY);
            }
        }

        static List<int> GetListOfAlliedHeroes(Team team)
        {
            List<int> heroes = new List<int>(team.Players.Count);

            foreach (Player p in team.Players)
            {
                Hero h = p.GetMostUsedHero();
                if (h != null) heroes.Add(h.ObjectId);
            }

            return heroes;
        }

        static int GetFirstCreepAttackTime(Replay replay, rect top, rect mid, rect bot)
        {
            List<string> dcCreeps = new List<string>();

            // Sentinel creeps:
            //
            // esen - Treant
            // edry - Druid of the Talon

            dcCreeps.Add("esen");
            dcCreeps.Add("edry");

            // Scourge creeps:
            // 
            // unec - Necromancer
            // ugho - Ghoul

            dcCreeps.Add("unec");
            dcCreeps.Add("ugho");

            int minCreepAttackTime = int.MaxValue;

            foreach (Player p in replay.Players)
                foreach (PlayerAction pa in p.Actions)
                    if (pa.IsValidObjects)
                    {
                        string codeID1;
                        replay.dcObjectsCodeIDs.TryGetValue(pa.Object1, out codeID1);

                        if (codeID1 == null || dcCreeps.Contains(codeID1))
                        {
                            if (top.ContainsXY(pa.X, pa.Y) || mid.ContainsXY(pa.X, pa.Y) || bot.ContainsXY(pa.X, pa.Y))
                            {
                                minCreepAttackTime = Math.Min(minCreepAttackTime, pa.Time);
                                break; // go to next player
                            }
                        }
                    }

            return (minCreepAttackTime != int.MaxValue) ? minCreepAttackTime : 180000; // 180000 = 3 minutes (will be used in case of errors)
        }

        static location GetAverageLocation(Player player, rect baseArea, List<int> alliedHeroes, int creepSpawnTime)
        {
            double x = 0;
            double y = 0;
            int count = 0;

            foreach (PlayerAction pa in player.Actions)
            {
                // if creeps were not spawned yet, then skip this action
                if (pa.Time < creepSpawnTime) continue;

                // if it's > than 3 minutes since creep spawn then stop
                if (pa.Time > creepSpawnTime + 180000)
                    break;

                if (pa.IsValidObjects && !alliedHeroes.Contains(pa.Object1))
                {
                    // skip this action if it was performed on the base area
                    if (baseArea.ContainsXY(pa.X, pa.Y, 800))
                        continue;

                    if (x == 0 && y == 0)
                    {
                        x = pa.X;
                        y = pa.Y;
                    }
                    else
                    {
                        x += pa.X;
                        y += pa.Y;
                    }

                    count++;
                    if (count > 50) break;
                }
            }

            x /= count;
            y /= count;

            return new location(x, y);
        }        
    }
}
