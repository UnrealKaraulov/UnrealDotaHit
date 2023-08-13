using System;
using System.Runtime.InteropServices;

namespace Utils
{
    /// <summary>
    /// ��� ��������� ��������� ���������� �������� ���������� ���� ����� ��
    /// �������� ������ ��������. ���������� ���������� ������������ � ������
    /// ����������, � ����� ����������� � ���������� (���������� ����� 
    /// �������� ������ �������).
    /// </summary>
    public struct PerfCounter
    {
        Int64 _start;

        /// <summary>
        /// �������� ������� ������� ����������.
        /// </summary>
        public void Start()
        {
            _start = 0;
            QueryPerformanceCounter(ref _start);
        }

        /// <summary>
        /// ��������� ������� ������� ���������� � ���������� ����� � ��������.
        /// </summary>
        /// <returns>����� � �������� ���������� �� ���������� �������
        /// ����. ���������� ����� �������� ���� �������.</returns>
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
