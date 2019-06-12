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
                window = FindWindowByCaption(IntPtr.Zero, "Sans titre - Paint");
                Thread.Sleep(1000);
            } while (window.ToInt32() == 0);
            log.DisplayMessage(LogManager.LogLevel.Info, "Game window found!");
            log.DisplayMessage(LogManager.LogLevel.Debug, "Game window address: 0x" + window.ToString("X"));
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    }
}
