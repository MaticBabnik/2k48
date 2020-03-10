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
        SafeFileHandle h;
        public Win32.SmallRect Rectangle;
        public bool Error
        {
            get;
        }
        public Win32.CharInfo[] Buffer
        {
            get; set;
        }
        private Win32.Coord Position = new Win32.Coord() { X = 0, Y = 0 };
        private Win32.Coord Size;
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
                    Buffer[i * width + j].Attributes = Win32.TwoColor(Win32.Color.LtRed, Win32.Color.White);
                    Buffer[i * width + j].Char.UnicodeChar = ' ';
                }
            DrawBuffer();
        }
        public void DrawBuffer()
        {
            Win32.WriteConsoleOutput(h, Buffer, Size, Position, ref Rectangle);
        }
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

    }
    public static class Win32
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] uint fileAccess, [MarshalAs(UnmanagedType.U4)] uint fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] int flags, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteConsoleOutput(SafeFileHandle hConsoleOutput, CharInfo[] lpBuffer, Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpWriteRegion);

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

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
        public enum Color : byte
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
        public static short TwoColor(Color bg, Color fg)
        {
            return (short)((byte)bg << 4 | (byte)fg);
        }
    }
}