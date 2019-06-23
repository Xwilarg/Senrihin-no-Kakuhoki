using IronOcr;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace Senrihin_no_Kakuhoki
{
    class Program
    {
        private static int GetPlayers(LogManager log)
        {
            ConsoleKeyInfo keyInfo;
            bool isKeyPressed = false;
            int players = 0;
            try
            {
                do
                {
                    keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.NumPad1 || keyInfo.Key == ConsoleKey.NumPad2
                        || keyInfo.Key == ConsoleKey.NumPad3 || keyInfo.Key == ConsoleKey.NumPad4)
                    {
                        isKeyPressed = true;
                        log.DisplayMessage(LogManager.LogLevel.Info, "There are " + keyInfo.Key.ToString().Substring(6) + " player(s) in your session.");
                    }
                    Thread.Sleep(1000);
                } while (isKeyPressed != true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.NumPad1:
                        players = 1;
                        break;
                    case ConsoleKey.NumPad2:
                        players = 2;
                        break;
                    case ConsoleKey.NumPad3:
                        players = 3;
                        break;
                    case ConsoleKey.NumPad4:
                        players = 4;
                        break;
                }
            }
            catch (ExternalException)
            { }
            return players;
        }
        private static Coordinates? GetRect(LogManager log, RECT rectangle)
        {
            int sizeX = rectangle.right - rectangle.left;
            int sizeY = rectangle.bottom - rectangle.top;

            log.DisplayMessage(LogManager.LogLevel.Debug, "Window position: (" + rectangle.left + ";" + rectangle.top + "), window size: (" + sizeX + ";" + sizeY + ")");

            int Players = GetPlayers(log);
            Coordinates Coords;

            try
            {
                Coords.StartX = (int)(sizeX / 4.00);
                Coords.StartY = (int)(sizeY / 2.62);
                Coords.EndX = (int)(sizeX / 1.33);
                Coords.EndY = (int)(sizeY / 2.30);
                Coords.Width = Coords.EndX - Coords.StartX;
                Coords.Height = Coords.EndY - Coords.StartY;
                Coords.SingleWidth = (int)((Coords.EndX - Coords.StartX) / 4.00);
                Coords.PlayerNum = Players;
                int middle = (int)((Coords.EndX - Coords.StartX) / 2.00);
                Coords.RealStartX = (middle - (int)(Coords.SingleWidth * Coords.PlayerNum / 2.00));

                log.DisplayMessage(LogManager.LogLevel.Debug, "Text rect position: (" + Coords.StartX + ";" + Coords.StartY + "), text rect size: (" + Coords.Width + ";" + Coords.Height + ")");

                return Coords;
            }
            catch (ExternalException)
            { }
            return null;
        }
        private static void OpenImage(string pathToFile)
        {
            Process imageViewer = new Process();
            imageViewer.StartInfo.FileName = @"C:\WINDOWS\system32\mspaint.exe";
            imageViewer.StartInfo.Arguments = pathToFile;
            imageViewer.Start();
        }
        private static void createAndReadImage(LogManager log, IntPtr gameWindow, Coordinates Coords)
        {
            int scale = 2;
            try
            {
                AutoOcr ocr = new AutoOcr();
                Bitmap bmpImage = new Bitmap(Coords.EndX, Coords.EndY, PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmpImage);
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                PrintWindow(gameWindow, hdcBitmap, 0);
                gfxBmp.ReleaseHdc(hdcBitmap);
                gfxBmp.Dispose();

                log.DisplayMessage(LogManager.LogLevel.Debug, "this is a debug test message");

                Bitmap imageToResize = bmpImage.Clone(new Rectangle(Coords.StartX, Coords.StartY, Coords.Width, Coords.Height), PixelFormat.Format32bppArgb);
                imageToResize.Save("Sample.bmp");
                Image image = Image.FromFile("Sample.bmp");
                Bitmap imageToOCR = new Bitmap(image, Coords.Width * scale, Coords.Height * scale);
                imageToOCR.Save("SampleOCR.bmp");

                string pathToImage;
                for (int i = 0; i < Coords.PlayerNum; ++i)
                {
                    pathToImage = "SampleOCR." + (i + 1) + ".bmp";
                    imageToOCR.Clone(new Rectangle(Coords.SingleWidth * i * scale, 0, (Coords.SingleWidth * scale) - 1, Coords.Height * scale), PixelFormat.Format32bppArgb).Save(pathToImage);
                    log.DisplayMessage(LogManager.LogLevel.Debug, "Text found on image number" + (i + 1) + ": " + ocr.Read(pathToImage).Text);
                    OpenImage(pathToImage);
                }
            }
            catch (ExternalException)
            { }
        }
        private static void Main(string[] args)
        {
            LogManager log = new LogManager(LogManager.LogLevel.Debug);

            log.DisplayMessage(LogManager.LogLevel.Info, "Looking for game window...");

            IntPtr gameWindow;
            do
            {
                gameWindow = FindWindow(IntPtr.Zero, "Warframe");
                Thread.Sleep(1000);
            } while (gameWindow.ToInt32() == 0);

            log.DisplayMessage(LogManager.LogLevel.Info, "Game window found!");

            RECT rectangle;
            do
            {
                GetWindowRect(gameWindow, out rectangle);
            } while (rectangle.top < 0);

            log.DisplayMessage(LogManager.LogLevel.Debug, "Game window address: 0x" + gameWindow.ToString("X"));

            var Coordinates = GetRect(log, rectangle);
            if (Coordinates != null)
                createAndReadImage(log, gameWindow, (Coordinates)Coordinates);
            while(true)
                Thread.Sleep(15000);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(IntPtr ptr, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
    }
}
