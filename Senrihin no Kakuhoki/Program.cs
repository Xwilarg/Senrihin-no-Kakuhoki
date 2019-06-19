using IronOcr;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
                window = FindWindow(IntPtr.Zero, "Wireframe.png");
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
            int startX = sizeX / 4;
            int startY = (int)(sizeY / 2.57);
            int textSizeX = (int)(sizeX / 1.33);
            int textSizeY = (int)(sizeY / 2.27);
            int textFinalSizeX = textSizeX - startX;
            int textFinalSizeY = textSizeY - startY;
            log.DisplayMessage(LogManager.LogLevel.Debug, "Text rect position: (" + startX + ";" + startY + "), text rect size: (" + textFinalSizeX + ";" + textFinalSizeY + ")");
            log.DisplayMessage(LogManager.LogLevel.Info, "Game window found!");
            AutoOcr ocr = new AutoOcr();
            while (true)
            {
                try
                {
                    Bitmap bmp = new Bitmap(textSizeX, textSizeY, PixelFormat.Format32bppArgb);
                    Graphics gfxBmp = Graphics.FromImage(bmp);
                    IntPtr hdcBitmap = gfxBmp.GetHdc();
                    PrintWindow(window, hdcBitmap, 0);
                    gfxBmp.ReleaseHdc(hdcBitmap);
                    gfxBmp.Dispose();
                    bmp.Clone(new Rectangle(startX, startY, textFinalSizeX, textFinalSizeY), PixelFormat.Format32bppArgb).Save("Debug.bmp");
                    log.DisplayMessage(LogManager.LogLevel.Debug, "Text found on window: " + ocr.Read("Debug.bmp").Text);
                } catch (ExternalException)
                { }
                Thread.Sleep(5000);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(IntPtr ptr, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
    }
}
