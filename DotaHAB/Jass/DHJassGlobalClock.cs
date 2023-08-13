using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DotaHIT.Jass
{
    public class DHJassGlobalClock
    {
        static bool enabled = false;
        static event MethodInvoker tick;
        static Thread clockThread;

        public static readonly double TickInterval = 0.10; // 100 milliseconds
        public static event MethodInvoker Tick
        {
            add
            {
                tick += value;
                if (!enabled) Enable();

            }
            remove
            {
                tick -= value;
                if (tick == null || tick.GetInvocationList().Length == 0)
                    Disable();
            }
        }

        static void Enable()
        {
            clockThread = new Thread(
                delegate() 
                {                    
                    int msTickInterval = (int)(TickInterval * 1000);                    

                    while (enabled)
                    {
                        Thread.Sleep(msTickInterval);
                        if (tick != null) tick();
                    }                  
                });

            enabled = true;
            clockThread.Start();
        }

        static void Disable()
        {
            if (clockThread != null)
            {
                enabled = false;
                tick = null;                
                clockThread = null;                
            }
        }
    }
}
