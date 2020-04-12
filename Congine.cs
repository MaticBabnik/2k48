using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace _2k48
{
    /*
       !🎮 Congine - high performance console "drawing" functions
        Matic Babnik 2020
    */

    public class Congine
    {
        #region Vars&Properties
        public Win32.CharInfo[] Buffer;
        public Win32.SmallRect Rectangle;
        public bool Error
        {
            get;
        }

        private SafeFileHandle h;
        private Win32.Coord Position = new Win32.Coord() { X = 0, Y = 0 };
        private Win32.Coord Size;
        #endregion

        /// <summary>
        /// Initalizes an instance of Congine
        /// </summary>
        /// <param name="width">Buffer Width</param>
        /// <param name="height">Buffer Height</param>
        public Congine(int width, int height)
        {
            Error = false;
            h = Win32.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (h.IsInvalid)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error getting the console handle!");
                Error = true;
                return;
            }

            Rectangle = new Win32.SmallRect() { Left = 0, Top = 0, Bottom = (short)height, Right = (short)width };
            Size = new Win32.Coord() { X = (short)width, Y = (short)height };

            Buffer = new Win32.CharInfo[width * height];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Buffer[i * width + j].Attributes = Win32.TwoColor(Win32.CColor.LtRed, Win32.CColor.White);
                    Buffer[i * width + j].Char.UnicodeChar = ' ';
                }
            WriteToConsole();
        }

        /// <summary>
        /// Writes the buffer to console
        /// </summary>
        public void WriteToConsole()
        {
            Win32.WriteConsoleOutput(h, Buffer, Size, Position, ref Rectangle);
        }

        /// <summary>
        /// Fills the buffer with the same char and attributes
        /// </summary>
        /// <param name="fillChar">Char</param>
        /// <param name="fillAtr">Attributes</param>
        public void FillBuffer(char fillChar, short fillAtr)
        {
            short width = Size.X, height = Size.Y;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Buffer[i * width + j].Attributes = fillAtr;
                    Buffer[i * width + j].Char.UnicodeChar = fillChar;
                }
        }

        /// <summary>
        /// Fills a rectangle with the same char and char attributes
        /// </summary>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="atr">Attributes</param>
        /// <param name="chr">Char</param>
        public void FillRect(int x, int y, int w, int h, short atr, char chr)
        {
            short width = Size.X, height = Size.Y;
            for (int i = x; i < x + w; i++)
                for (int j = y; j < y + h; j++)
                {
                    Buffer[j * width + i].Attributes = atr;
                    Buffer[j * width + i].Char.UnicodeChar = chr;
                }
        }

        /// <summary>
        /// SetText() but with attributes
        /// </summary>
        /// <param name="x">X coord of first char</param>
        /// <param name="y">Line number</param>
        /// <param name="text">Text to place</param>
        /// <param name="atr">Char Attributes for the text</param>
        public void SetTextAndAttribute(int x, int y, string text, short atr)
        {
            for (int i = x; i < x + text.Length; i++)
            {
                Buffer[y * Size.X + i].Attributes = atr;
                Buffer[y * Size.X + i].Char.UnicodeChar = text[i - x];
            }
        }

        /// <summary>
        /// Places text into buffer
        /// </summary>
        /// <param name="x">X coordinate of first char</param>
        /// <param name="y">Line number</param>
        /// <param name="text">Text to place</param>
        public void SetText(int x, int y, string text)
        {
            for (int i = x; i < x + text.Length; i++)
            {
                var a = System.Text.Encoding.ASCII.GetBytes(text);
                Buffer[y * Size.X + i].Char.AsciiChar = a[i - x];
            }
        }

        /// <summary>
        /// Prints a block of text.
        /// </summary>
        /// <param name="x">X location of topleft corner</param>
        /// <param name="y">Y location of topleft corner</param>
        /// <param name="data">Multiline string</param>
        public void WriteImg(int x, int y, string[] data)
        {
            int i = 0;
            foreach (string line in data)
            {
                SetText(x, y + i, line);
                i++;
            }
        }
        public void SetChar(int x, int y, byte chr, short atr)
        {
            Buffer[y * Size.X + x].Attributes = atr;
            Buffer[y * Size.X + x].Char.AsciiChar = chr;
        }
    }

    /// <summary>
    /// Windows console API
    /// </summary>
    public static class Win32
    {
#pragma warning disable CA1401 // P/Invokes should not be visible

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]

        public static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] uint fileAccess, [MarshalAs(UnmanagedType.U4)] uint fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] int flags, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteConsoleOutput(SafeFileHandle hConsoleOutput, CharInfo[] lpBuffer, Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpWriteRegion);

#pragma warning restore CA1401 // P/Invokes should not be visible

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        /// <summary>
        /// Struct that contains a char and its attributes
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        /// <summary>
        /// Windows.h struct for a rectange
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        /// <summary>
        /// Enumerator for 16 console colors
        /// </summary>
        public enum CColor : byte
        {
            Black,
            Blue,
            Green,
            Cyan,
            Red,
            Pink,
            Yellow,
            LtGray,
            Gray,
            LtBlue,
            LtGreen,
            LtCyan,
            LtRed,
            LtPink,
            LtYellow,
            White
        }

        /// <summary>
        /// Converts two CColors to a short.
        /// </summary>
        /// <param name="bg">Char background color</param>
        /// <param name="fg">Char color</param>
        /// <returns>short</returns>
        public static short TwoColor(CColor bg, CColor fg)
        {
            return (short)((byte)bg << 4 | (byte)fg);
        }
    }
}