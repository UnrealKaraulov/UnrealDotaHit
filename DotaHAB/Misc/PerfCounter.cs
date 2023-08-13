using System;
using System.Runtime.InteropServices;

namespace Utils
{
    /// <summary>
    /// Эта структура позволяет подсчитать скорость выполнения кода одним из
    /// наиболее точным способов. Фактически вычисления производятся в тактах
    /// процессора, а потом переводятся в милисекунд (десятичная часть 
    /// является долями секунды).
    /// </summary>
    public struct PerfCounter
    {
        Int64 _start;

        /// <summary>
        /// Начинает подсчет вермени выполнения.
        /// </summary>
        public void Start()
        {
            _start = 0;
            QueryPerformanceCounter(ref _start);
        }

        /// <summary>
        /// Завершает полсчет вермени исполнения и возвращает время в секундах.
        /// </summary>
        /// <returns>Время в секундах потраченое на выполнение участка
        /// кода. Десятичная часть отражает доли секунды.</returns>
        public float Finish()
        {
            Int64 finish = 0;
            QueryPerformanceCounter(ref finish);

            Int64 freq = 0;
            QueryPerformanceFrequency(ref freq);
            return (((float)(finish - _start) / (float)freq));
        }

        [DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceCounter(ref Int64 performanceCount);

        [DllImport("Kernel32.dll")]
        static extern bool QueryPerformanceFrequency(ref Int64 frequency);
    }
}
