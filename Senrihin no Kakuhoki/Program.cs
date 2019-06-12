using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Senrihin_no_Kakuhoki
{
    class Program
    {
        private static void Main(string[] args)
        {
            LogManager log = new LogManager(LogManager.LogLevel.Debug);
            log.DisplayMessage(LogManager.LogLevel.Info, "Looking for game window...");
            IntPtr window;
            do
            {
                window = FindWindow(IntPtr.Zero, "Sans titre - Paint");
                Thread.Sleep(1000);
            } while (window.ToInt32() == 0);
            log.DisplayMessage(LogManager.LogLevel.Debug, "Game window address: 0x" + window.ToString("X"));
            RECT rect;
            do
            {
                GetWindowRect(window, out rect);
            } while (rect.top < 0);
            int sizeX = rect.right - rect.left;
            int sizeY = rect.bottom - rect.top;
            log.DisplayMessage(LogManager.LogLevel.Debug, "Window position: (" + rect.left + ";" + rect.top + "), window size: (" + sizeX + ";" + sizeY + ")");
            log.DisplayMessage(LogManager.LogLevel.Info, "Game window found!");
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(IntPtr ptr, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    }
}
