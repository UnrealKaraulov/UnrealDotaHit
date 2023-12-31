using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.DatabaseModel.Data;
using DotaHIT.DatabaseModel.DataTypes;
using DotaHIT.Core;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Specialized;
using DotaHIT.Jass.Native.Types;
using DotaHIT.Jass;
using DotaHIT.Core.Resources;
using DotaHIT.Jass.Types;
using DotaHIT.Core.Resources.Media;
using System.IO;

namespace DotaHIT
{
    public partial class ActionsForm : Form
    {
        internal int mbX = -1;
        internal int mbY = -1;

        System.Windows.Forms.Timer blinkTimer = null;

        HabPropertiesCollection hpcData = DHMpqDatabase.UnitSlkDatabase["UnitData"];
        HabPropertiesCollection hpcAbils = DHMpqDatabase.UnitSlkDatabase["UnitAbilities"];
        HabPropertiesCollection hpcWeapons = DHMpqDatabase.UnitSlkDatabase["UnitWeapons"];
        HabPropertiesCollection hpcBalance = DHMpqDatabase.UnitSlkDatabase["UnitBalance"];
        HabPropertiesCollection hpcUI = DHMpqDatabase.UnitSlkDatabase["UnitUI"];
        HabPropertiesCollection hpcProfile = DHMpqDatabase.UnitSlkDatabase["Profile"];

        public ActionsForm()
        {
            InitializeComponent();
            streakCmbB.SelectedIndex = 0;
            suicideRB.Checked = true;
            deathsCmbB.SelectedIndex = 0;            

            PrepareCreepsList();
        }

        internal void PrepareCreepsList()
        {
            DHMpqFile jScript = DHRC.Default.GetFile(MpqPath.Script.Custom);
            if (jScript.IsNull)
                jScript = DHRC.Default.GetFile(MpqPath.Script.Custom2);
            MemoryStream ms = jScript.GetStream();
            byte[] buffer = ms.GetBuffer();
            string script;
            unsafe
            {
                fixed (byte* ptr = buffer)
                {
                    script = new string((sbyte*)ptr);
                }
            }

            List<HabProperties> list = new List<HabProperties>();

            foreach (HabProperties hpsUnit in hpcData)
            {
                HabProperties hpsBalance = hpcBalance[hpsUnit.name];
                if (hpsBalance.GetStringValue("Primary").Length > 1)
                    continue;

                HabProperties hpsProfile = hpcProfile[hpsUnit.name];
                if (hpsProfile == null) continue;

                if (hpsUnit.GetStringValue("targType") != "ground")
                    continue;

                HabProperties hpsWeapons = hpcWeapons[hpsUnit.name];
                if (hpsWeapons == null) continue;

                if (hpsWeapons.GetStringValue("weapTp1") == "_")
                    continue;

                HabProperties hpsUI = hpcUI[hpsUnit.name];
                if (hpsUI == null) continue;

                if (!hpsUI.GetStringValue("file").StartsWith("units", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!hpsWeapons.ContainsKey("mindmg1"))
                    continue;

                if (!script.Contains("'" + hpsUnit.name + "'") && !script.Contains(DHJassInt.id2int(hpsUnit.name)+""))
                    continue;

                list.Add(hpsProfile);
            }

            creepsCmbB.DataSource = list;
        }

        public void SetParent(MainForm parentForm)
        {         
            this.Owner = parentForm;         
        }

        private void captionB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mbX = MousePosition.X - this.Location.X;
                mbY = MousePosition.Y - this.Location.Y;
            }
            else
                mbX = mbY = -1;
        }

        private void captionB_MouseMove(object sender, MouseEventArgs e)
        {
            if (mbX != -1 && mbY != -1)
                if ((MousePosition.X - mbX) != 0 && (MousePosition.Y - mbY) != 0)
                    this.SetDesktopLocation(MousePosition.X - mbX, MousePosition.Y - mbY);
        }

        private void captionB_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mbX = mbY = -1;
        }       

        private void closeB_Click(object sender, EventArgs e)
        {
            this.Hide();
        }        

        private void creepsCmbB_Format(object sender, ListControlConvertEventArgs e)
        {
            HabProperties hpsProfile = e.ListItem as HabProperties;
            e.Value = hpsProfile.GetStringValue("Name").Trim('"');
        }

        private void creepsCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            HabProperties hpsProfile = (sender as ComboBox).SelectedItem as HabProperties;
            if (hpsProfile != null)
            {
                creepArtPanel.BackgroundImage = DHRC.Default.GetImage(hpsProfile.GetStringValue("Art"));

                HabProperties hpsBalance = hpcBalance[hpsProfile.name];

                int dice = hpsBalance.GetIntValue("bountydice");
                int sides = hpsBalance.GetIntValue("bountysides");
                int plus = hpsBalance.GetIntValue("bountyplus");

                creepGoldTextBox.Text = "" + (dice + plus) + " - " + (dice*sides + plus);
                creepLevelTextBox.Text = hpsBalance.GetStringValue("level");
            }
            else
            {
                creepArtPanel.BackgroundImage = null;
                creepGoldTextBox.Text = "";
                creepLevelTextBox.Text = "";
            }
        }

        private void killCreepB_Click(object sender, EventArgs e)
        {
            HabProperties hpsProfile = creepsCmbB.SelectedItem as HabProperties;

            player p = player.players[12];

            for (int i = 0; i < (int)creepsNumUD.Value; i++)
            {
                unit u = p.add_unit(hpsProfile.name);

                u.OnDeath(Current.unit);                

                u.destroy();
            }
        }

        private void killHeroB_Click(object sender, EventArgs e)
        {
            int streak = streakCmbB.SelectedIndex;
            if (streak > 0) streak += 2;

            player enemyPlayer = player.players[7];
            player alliedPlayer = player.players[2];

            for (int i = 0; i < (int)heroesNumUD.Value; i++)
            {
                unit enemyHero = new unit();
                enemyHero.codeID = "Hero"; // so triggers could work with id
                enemyHero.ID.Value = "[Enemy Hero to be killed]";
                enemyHero.primary = PrimAttrType.Str; // only heroes have primary attributes
                enemyHero.Level = (int)heroLevelNumUD.Value;

                enemyHero.set_owningPlayer(enemyPlayer);

                // generate killing streak for enemy hero
                // by killing allied heroes
                for (int j = 0; j < streak; j++)
                {
                    unit victim = new unit();
                    victim.codeID = "Prey";
                    victim.ID.Value = "[Allied Victim]";
                    victim.primary = PrimAttrType.Str;

                    victim.set_owningPlayer(alliedPlayer);                    

                    victim.OnDeath(enemyHero);                    
                    victim.destroy();
                }

                // now our hero kills this enemy hero
                enemyHero.OnDeath(Current.unit);                
                enemyHero.destroy();
            }
        }

        private void suicideRB_CheckedChanged(object sender, EventArgs e)
        {
            deathsCmbB.Enabled = killedByRB.Checked;
            deathsCmbB_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void deathsCmbB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deathsCmbB.Enabled==false 
                || deathsCmbB.SelectedIndex == 1 // ally
                || deathsCmbB.SelectedIndex == 4)// unknown
                return;

            if (deathsCmbB.SelectedIndex == 0)// enemy
            {
                heroLevelNumUD.BackColor = Color.DeepSkyBlue;
                streakCmbB.BackColor = Color.DeepSkyBlue;
            }

            if (deathsCmbB.SelectedIndex >= 2)// creep            
                creepsCmbB.BackColor = Color.DeepSkyBlue;            

            if (blinkTimer != null)
            {
                blinkTimer.Stop();
                blinkTimer = null;
            }

            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 500;
            blinkTimer.Tick += new EventHandler(blinkTimer_Tick);
            blinkTimer.Start();
        }

        void blinkTimer_Tick(object sender, EventArgs e)
        {
            heroLevelNumUD.BackColor = Color.White;
            streakCmbB.BackColor = Color.White;
            creepsCmbB.BackColor = Color.White;
            blinkTimer.Stop();
            blinkTimer = null;
        }

        private void dieB_Click(object sender, EventArgs e)
        {
            if (suicideRB.Checked)
            {
                Current.unit.OnDeath(Current.unit);
                Current.unit.playSound(AnimSounds.Death);
            }
            else
                switch (deathsCmbB.SelectedIndex)
                {
                    case 0: // enemy hero
                        int streak = streakCmbB.SelectedIndex;
                        if (streak > 0) streak += 2;

                        player enemyPlayer = player.players[7];
                        player alliedPlayer = player.players[2];

                        unit enemyHero = new unit();
                        enemyHero.codeID = "Hero"; // so triggers could work with id
                        enemyHero.ID.Value = "[Enemy Hero that will kill you]";
                        enemyHero.primary = PrimAttrType.Str; // only heroes have primary attributes
                        enemyHero.Level = (int)heroLevelNumUD.Value;

                        enemyHero.set_owningPlayer(enemyPlayer);

                        // generate killing streak for enemy hero
                        // by killing allied heroes
                        for (int j = 0; j < streak; j++)
                        {
                            unit victim = new unit();
                            victim.codeID = "Prey";
                            victim.ID.Value = "[Allied Victim]";
                            victim.primary = PrimAttrType.Str;

                            victim.set_owningPlayer(alliedPlayer);

                            victim.OnDeath(enemyHero);
                            victim.destroy();
                        }

                        // enemy hero kills our hero
                        Current.unit.OnDeath(enemyHero);

                        enemyHero.destroy();
                        break;

                    case 1: // allied hero
                        alliedPlayer = player.players[2];
                        unit ally = new unit();
                        ally.codeID = "Ally";
                        ally.ID.Value = "[Allied Killer]";
                        ally.primary = PrimAttrType.Str;

                        ally.set_owningPlayer(alliedPlayer);

                        // allied hero kills our hero
                        Current.unit.OnDeath(ally);

                        ally.destroy();
                        break;

                    case 2: // enemy creep
                        HabProperties hpsProfile = creepsCmbB.SelectedItem as HabProperties;

                        player enemyCreepPlayer = player.players[6];

                        unit creep = enemyCreepPlayer.add_unit(hpsProfile.name);

                        // creep kills our hero
                        Current.unit.OnDeath(creep);

                        creep.destroy();
                        break;

                    case 3: // neutral creep
                        hpsProfile = creepsCmbB.SelectedItem as HabProperties;

                        player neutralPlayer = player.players[12];

                        creep = neutralPlayer.add_unit(hpsProfile.name);

                        // creep kills our hero
                        Current.unit.OnDeath(creep);

                        creep.destroy();
                        break;
                    case 4: // unknown source
                        Current.unit.OnDeath(null);
                        break;

                }
        }
    }
}