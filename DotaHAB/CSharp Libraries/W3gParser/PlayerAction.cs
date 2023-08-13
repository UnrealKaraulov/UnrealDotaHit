using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Deerchao.War3Share.W3gParser
{
    public class PlayerAction
    {
        private readonly PlayerActionType type;
        private readonly double x;
        private readonly double y;
        private readonly int time;
        private readonly int object1;
        private readonly int object2;

        public PlayerAction(PlayerActionType type, double x, double y, int time, int object1, int object2)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.time = time;
            this.object1 = object1;
            this.object2 = object2;
        }

        public PlayerActionType Type
        {
            get { return type; }
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }

        public int Time
        {
            get { return time; }
        }

        public int Object1
        {
            get { return object1; }
        }

        public int Object2
        {
            get { return object2; }
        }

        public bool IsValidObjects
        {
            get
            {
                return !(object1 == object2 && object1 == -1);
            }
        }
    }

    public enum PlayerActionType
    {
        RightClick,
        Attack
    }
}
