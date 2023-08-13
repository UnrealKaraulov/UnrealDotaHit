using System.Collections.Generic;
using DotaHIT.Extras;

namespace Deerchao.War3Share.W3gParser
{
    public class Heroes
    {
        readonly ReplayMapCache cache;
        readonly List<Hero> heroes = new List<Hero>();
        readonly List<KeyValuePair<int, Hero>> buildOrders = new List<KeyValuePair<int, Hero>>();

        public Heroes(ReplayMapCache cache)
        {
            this.cache = cache;
        }

        internal void Order(Hero h, int time)
        {
            heroes.Add(h);
            buildOrders.Add(new KeyValuePair<int, Hero>(time, h));
        }

        internal void Order(string name, int time)
        {
            foreach (Hero hero in heroes)
            {
                if (hero.Name == name)
                {
                    hero.Order(time);
                    return;
                }
            }
            Hero h = new Hero(cache, name);
            heroes.Add(h);
            buildOrders.Add(new KeyValuePair<int, Hero>(time, h));
        }

        public IEnumerable<Hero> Items
        {
            get
            {
                return heroes;
            }
        }

        /// <summary>
        /// list of key/value pairs, where key - time and value - hero
        /// </summary>
        public List<KeyValuePair<int, Hero>> BuildOrders
        {
            get
            {
                return buildOrders;
            }
        }

        public Hero this[string name]
        {
            get
            {
                foreach (Hero hero in heroes)
                    if (hero.Name == name)
                        return hero;
                return null;
            }
        }

        public Hero this[int index]
        {
            get
            {
                if (index < this.Count)
                    return heroes[index];    
                return null;
            }
        }

        internal void Cancel(string name, int time)
        {
            foreach (Hero hero in heroes)
            {
                if (hero.Name == name)
                {
                    hero.Cancel(time);
                    return;
                }
            }
        }

        internal bool Train(string ability, int time)
        {
            //string heroName = ParserUtility.GetHeroByAbility(ability);
            foreach (Hero hero in heroes)
            {
                //if (string.Compare(hero.Name, heroName, true)==0)
                //{
                    return hero.Train(ability, time, hero.Level);
                //}
            }

            return false;
        }

        internal void PossibleRetrained(int time)
        {
            foreach (Hero hero in heroes)
                hero.PossibleRetrained(time);
        }

        public int Count
        {
            get
            {
                return heroes.Count;
            }
        }        
    }
}