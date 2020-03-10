using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2k48
{
    namespace Game
    {
        public class Game
        {



            Congine ge;
            int[,] Playfield;
            public Game(ref Congine engine, int size)
            {
                ge = engine;
                Playfield = new int[4, 4]
                {
                    {   0,   2,   4,   8},
                    { 128,  64,  32,  16},
                    { 256, 512,1024,2048},
                    {   0,   0,   0,   0}
                };
                /*for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        Playfield[x, y] = 0;*/
            }



            public int Run()
            {
                while (true)
                {
                    Render();
                }
            }




            private void Render()
            {
                ge.FillBuffer(' ', Win32.TwoColor(Win32.Color.Gray, Win32.Color.White));
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        ge.FillRect((x * 9) + 2, y * 5 + 1, 7, 4, NumToColor(Playfield[x,y]), ' ');
                    }
                }
                ge.DrawBuffer();
            }
            private short NumToColor(int num)
            {
                switch (num)
                {
                    case (0):
                        return 0x77; //ltgray,ltgray
                    case (2):
                        return 0xE0; //ltyellow,black
                    case (4):
                        return 0x60; //ltyellow,black
                    case (8):
                        return 0xC0; //ltyellow,black
                    case (16):
                        return 0x40; //ltyellow,black
                    case (32):
                        return 0xD0; //ltyellow,black
                    case (64):
                        return 0x50; //ltyellow,black
                    case (128):
                        return 0xB0; //ltyellow,black
                    case (256):
                        return 0x30; //ltyellow,black
                    case (512):
                        return 0x90; //ltyellow,black
                    case (1024):
                        return 0x10; //ltyellow,black
                    case (2048):
                        return 0xA0; //ltyellow,black
                }
                return 0x00;
            }


        }
    }

}
