using System;

namespace Deerchao.War3Share.W3gParser
{
    public struct OrderInfo
    {
        public int time;
        public Player player;
        public float x;
        public float y;

        public OrderInfo(int time, Player player, float x, float y)
        {
            this.time = time;
            this.player = player;
            this.x = x;
            this.y = y;
        }        
    }
}
