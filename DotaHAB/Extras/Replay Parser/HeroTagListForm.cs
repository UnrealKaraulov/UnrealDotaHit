using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotaHIT.Core.Resources;
using DotaHIT.Core;
using DotaHIT.DatabaseModel.Format;

namespace DotaHIT.Extras
{
    public partial class HeroTagListForm : Form
    {
        ReplayMapCache cache;
        HabPropertiesCollection hpcHeroTagsContainer;        
        List<string> heroes;
        string cfgFileName = null;

        public HeroTagListForm(ReplayMapCache cache)
        {
            InitializeComponent();

            this.cache = cache;

            heroes = new List<string>(cache.dcHeroesTaverns.Keys.Count);
            foreach (string key in cache.dcHeroesTaverns.Keys)
                heroes.Add(key);

            heroes.Sort((Comparison<string>)delegate(string a, string b)
            {
                return StringComparer.OrdinalIgnoreCase.Compare(this.cache.hpcUnitProfiles[a].GetStringValue("Name"), this.cache.hpcUnitProfiles[b].GetStringValue("Name"));
            });
        }

        private void heroTagGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            HabProperties hpsHero = cache.hpcUnitProfiles[heroes[e.RowIndex]];

            switch (e.ColumnIndex)
            {
                case 0: // image
                    e.Value = cache.Resources.GetImage(hpsHero.GetStringValue("Art"));
                    break;

                case 1: // tag
                    e.Value = hpcHeroTagsContainer.GetStringValue("HeroTags", hpsHero.name);
                    break;

                case 2: // class
                    e.Value = hpcHeroTagsContainer.GetStringValue("HeroClasses", hpsHero.name, hpsHero.GetStringValue("Name").Trim('\"'));
                    break;

                case 3: // name
                    e.Value = hpcHeroTagsContainer.GetStringValue("HeroNames", hpsHero.name, hpsHero.GetStringListValue("Propernames")[0]);
                    break;
            }
        }

        private void heroTagGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            string heroName = heroes[e.RowIndex];

            switch (e.ColumnIndex)
            {
                case 1: // tag
                    hpcHeroTagsContainer["HeroTags", heroName] = e.Value;
                    break;

                case 2: // class
                    hpcHeroTagsContainer["HeroClasses", heroName] = e.Value;
                    break;

                case 3: // name
                    hpcHeroTagsContainer["HeroNames", heroName] = e.Value;
                    break;
            }            
        }

        private void HeroTagListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            hpcHeroTagsContainer.SaveToFile(cfgFileName);
        }

        public DialogResult ShowDialog(string cfgFileName, HabPropertiesCollection hpcCfg)
        {
            this.cfgFileName = cfgFileName;
            this.hpcHeroTagsContainer = hpcCfg;            

            heroTagGridView.RowCount = heroes.Count; 

            return base.ShowDialog();
        }
    }   
}