using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DotaHIT.Extras.Replay_Parser
{
    using Deerchao.War3Share.W3gParser;
    using DotaHIT.DatabaseModel.Format;
    using DotaHIT.Core.Resources;

    public partial class ReplayDataExtractForm : Form
    {
        string path;
        MapRequiredEventHandler mapRequiredEvent;
        public ReplayDataExtractForm(string path, MapRequiredEventHandler mapRequiredEvent)
        {
            InitializeComponent();

            this.path = path;
            this.mapRequiredEvent = mapRequiredEvent;
            filenameTextBox.Text = Path.GetFileName(path);
        }

        private void okB_Click(object sender, EventArgs e)
        {
            if (!chatlogCB.Checked && !killLogCB.Checked && !statisticsCB.Checked)
                return;

            Replay replay = new Replay(path, mapRequiredEvent);

            string filename;
            string directory = Path.GetDirectoryName(path);
            string replayName = Path.GetFileNameWithoutExtension(path);
            string output = "The following files:" + Environment.NewLine + Environment.NewLine;
          
            if (chatlogCB.Checked)
            {
                filename = replayName + "_chatlog.txt";                
                string[] lines = ChatsToLines(replay.Chats);

                File.WriteAllLines(directory + "\\" + filename, lines, Encoding.UTF8);

                output += filename + Environment.NewLine;
            }

            if (killLogCB.Checked)
            {
                filename = replayName + "_killLog.txt";
                string[] lines = KillsToLines(replay.Kills);

                File.WriteAllLines(directory + "\\" + filename, lines, Encoding.UTF8);

                output += filename + Environment.NewLine;
            }

            if (statisticsCB.Checked)
            {
                filename = replayName + "_stats.txt";
                string[] lines = PlayerStatsToLines(replay.Players);

                File.WriteAllLines(directory + "\\" + filename, lines, Encoding.UTF8);

                output += filename + Environment.NewLine;
            }
            
            output += Environment.NewLine + "are saved to the same folder as the selected replay";
            MessageBox.Show(output, "Replay Data Extraction Complete");

            this.Close();
        }

        public static string[] ChatsToLines(List<ChatInfo> chats)
        {
            string[] lines = new string[chats.Count];

            bool isPaused = false;
            for (int i = 0; i < chats.Count; i++)
            {
                string line = "";
                ChatInfo ci = chats[i];

                if (ci.To != TalkTo.System)
                {
                    line += DHFormatter.ToString(ci.Time) + " ";

                    switch (ci.To)
                    {
                        case TalkTo.Allies:
                        case TalkTo.All:
                        case TalkTo.Observers:
                            line += "[" + ci.To.ToString() + "] ";
                            break;
                        default:
                            continue;
                    }

                    line += ci.From.Name + ": " + ci.Message;
                }
                else
                {
                    switch (ci.Message)
                    {
                        case "pause":
                            if (!isPaused)
                            {
                                isPaused = true;
                                line += DHFormatter.ToString(ci.Time) + " " + ci.From.Name + " " + "paused the game.";
                            }
                            break;

                        case "resume":
                            if (isPaused)
                            {
                                isPaused = false;
                                line += DHFormatter.ToString(ci.Time) + " " + ci.From.Name + " " + "has resumed the game.";
                            }
                            break;

                        case "save":
                            line += DHFormatter.ToString(ci.Time) + " " + "Game was saved by " + ci.From.Name;
                            break;

                        case "leave":
                            line += DHFormatter.ToString(ci.Time) + " " + ci.From.Name + " " + "has left the game.";
                            break;
                    }
                }

                lines[i] = line;
            }

            return lines;
        }
        public static string[] KillsToLines(List<KillInfo> kills)
        {            
            string[] lines = new string[kills.Count];

            TimeSpan lastKillTime = TimeSpan.MinValue;            
            for (int i = 0; i < kills.Count; i++)
            {
                string line = "";
                KillInfo ki = kills[i];

                line += DHFormatter.ToString(ki.Time) + ((ki.Time.TotalSeconds - lastKillTime.TotalSeconds < 8)? "* " : "  "); 

                if (ki.Killer != null)                
                    line += ki.Killer.Name + " (" + ki.Killer.GetMostUsedHeroClass() + ")";                
                else
                    line += "Creeps";

                line += "  killed  " + ki.Victim.Name + " (" + ki.Victim.GetMostUsedHeroClass() + ")";                
                lines[i] = line;

                lastKillTime = ki.Time;
            }

            return lines;
        }
        public static string[] PlayerStatsToLines(List<IPlayer> players)
        {
            List<string> lines = new List<string>(players.Count);

            foreach (Player p in players)
                if (!p.IsComputer && !p.IsObserver)
                {
                    string line = "";

                    line += (p.SlotNo + 1) + "; ";
                    line += p.Name + "; ";
                    
                    if (p.Heroes.Count != 0)
                        line += DHFormatter.ToString(p.GetMostUsedHeroClass());

                    line += "; ";
                    
                    line += "" + (int)p.Apm + "; ";
                    line += p.getGCVStringValue("kills", p.getGCVStringValue("1", "")) + "/" + p.getGCVStringValue("deaths", p.getGCVStringValue("2", "")) + "/" + p.getGCVStringValue("5", "") + "; ";
                    line += p.getGCVStringValue("creeps", p.getGCVStringValue("3", "")) + "/" + p.getGCVStringValue("denies", p.getGCVStringValue("4", "")) + "/" + p.getGCVStringValue("7", "") + "; ";

                    lines.Add(line);
                }

            return lines.ToArray();
        }
    }
}