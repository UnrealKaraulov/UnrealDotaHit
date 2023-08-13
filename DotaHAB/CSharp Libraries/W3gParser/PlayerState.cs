using System;
using System.Collections.Generic;
using System.Text;

namespace Deerchao.War3Share.W3gParser
{
    public class PlayerState
    {
        public static readonly int CanResearchAtOnce = 2; // assume that hero can have up to 2 research points available at once

        public List<int> CurrentSelection = new List<int>(2);

        private bool isResearching;
        private int researchPoints;
        private int heroLevelOnResearchStart;
        private Hero heroSelectedForResearch;

        public bool IsResearching
        {
            get
            {
                return isResearching;
            }
        }        

        public void BeginResearch(Hero hero)
        {
            heroSelectedForResearch = hero;
            heroLevelOnResearchStart = hero.Level;
            researchPoints = CanResearchAtOnce;
            isResearching = true;
        }

        public void CompleteResearch()
        {
            researchPoints--;
            if (researchPoints <= 0) EndResearch();            
        }

        public void EndResearch()
        {
            if (isResearching)
            {
                // calculate level required for the hero to research skills that he did research
                int researchPointsUsed = CanResearchAtOnce - researchPoints;
                int levelRequiredToResearch = heroLevelOnResearchStart + researchPointsUsed;

                // now specify that hero level in his last researched skills
                List<OrderItem> skillsResearched = heroSelectedForResearch.Abilities.BuildOrders;

                for (int i = skillsResearched.Count - researchPointsUsed;
                    i >= 0 && i < skillsResearched.Count;
                    i++)
                {
                    skillsResearched[i].Tag = Math.Max(levelRequiredToResearch, skillsResearched[i].Tag);
                }
            }

            heroSelectedForResearch = null;
            researchPoints = 0;
            isResearching = false;
        }

        public int MaxAllowedLevelForResearch
        {
            get { return heroLevelOnResearchStart + CanResearchAtOnce; }
        }
    }
}