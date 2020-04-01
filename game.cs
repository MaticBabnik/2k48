using System;

namespace _2k48
{
    namespace Game
    {
        /// <summary>
        /// the 2k48 game
        /// basicaly 2048 but crapier and in terminal
        /// </summary>
        public class Game
        {
            #region Vars&Stuff
            Congine ge;
            int[,] Playfield;
            public int Score;
            Move InputState;
            Move nInputState;
            int repeat;
            int Size;
            Random rnd = new Random();
            #endregion

            /// <summary>
            /// Inits the 2k48 game
            /// </summary>
            /// <param name="engine">reference to Congine instance</param>
            /// <param name="size">Not implemented</param>
            public Game(ref Congine engine, int size)
            {
                ge = engine;
                InputState = Move.NoMove;

                Score = 0;
                repeat = 0;
                Playfield = new int[4, 4]
                {
                    {0,0,0,0},
                    {0,2,2,0},
                    {0,0,0,0},
                    {0,2,0,2}

                };
                Size = size;
                ge.FillBuffer(' ', 0x01);
                ge.WriteImg(4, 0, Resources.logo);
                ge.SetTextAndAttribute(20, 19, "Alphamale games!", 0x0B);
                ge.SetTextAndAttribute(20, 20, "<press spacebar>", 0x08);
                ge.WriteToConsole();
                /*for (int x = 0; x < 4; x++)
                    for (int y = 0; y < 4; y++)
                        Playfield[x, y] = 0;*/
            }

            /// <summary>
            /// Runs the game
            /// </summary>
            /// <returns></returns>
            public int Run()
            {
                Render();
                while (true)
                {
                    nInputState = GetInput();

                    if (nInputState == InputState)
                    {
                        repeat++;
                    }
                    else
                    {
                        repeat = 0;
                        InputState = nInputState;
                    }

                    CreateMove(InputState);

                    Score = getScore(ref Playfield, Size);

                    Render();
                }
            }

            /// <summary>
            /// Renders the game
            /// </summary>
            private void Render()
            {
                ge.FillBuffer(' ', 0x0F);                // clear the buffer

                ge.FillRect(0, 0, 38, 21, 0x70, ' ');    //make the game and side panel
                ge.FillRect(38, 0, 17, 21, 0x8F, ' ');

                for (int y = 0; y < Size; y++)              //draw the tiles
                {
                    for (int x = 0; x < Size; x++)
                    {
                        ge.FillRect((x * 9) + 2, y * 5 + 1, 7, 4, NumToColor(Playfield[x, y]), ' ');
                        ge.SetText((x * 9) + 3, (y * 5) + 2, Playfield[x, y].ToString());
                    }
                }

                ge.SetText(38, 2, "----- 2K48 ------");                     //draw the side panel content
                ge.SetTextAndAttribute(40, 4, "Matic Babnik", 0x87);
                ge.SetTextAndAttribute(39, 8, "Score:", 0x80);
                ge.SetTextAndAttribute(39, 9, Score.ToString(), 0x80);

                ge.SetTextAndAttribute(40, 14, InputState.ToString(), 0xCF);
                ge.SetTextAndAttribute(40, 15, repeat.ToString(), 0xCF);

                ge.WriteToConsole(); //update the console
            }

            /// <summary>
            /// Checks if the move changes the playfield, changes the playfield and then creates a new tile
            /// </summary>
            /// <param name="m">Move direction</param>
            private void CreateMove(Move m)
            {
                int[,] PrevState = (int[,])Playfield.Clone();
                MoveAndMerge(m);
                if (!ComparePlayfields(Playfield, PrevState))
                    CreateNewBlock();
            }

            /// <summary>
            ///    Compares two playfields.
            /// </summary>
            /// <param name="a">Playfield A</param>
            /// <param name="b">Playfield B</param>
            /// <returns>True if same</returns>
            private bool ComparePlayfields(int[,] a, int[,] b)
            {
                if (a.Length == b.Length)
                {
                    for (int x = 0; x < Size; x++)
                        for (int y = 0; y < Size; y++)
                            if (a[x, y] != b[x, y]) return false;
                    return true;
                }
                else return false;
            }

            //TODO REWRITE THIS CRAP its over 100 lines
            //TODO MAKE A VARIANT THAT JUST CHECKS IF THE MOVE IS POSSIBLE/CHANGES STUFF
            /// <summary>
            /// Changes the playfield
            /// </summary>
            /// <param name="dir">Move Direction</param>
            private void MoveAndMerge(Move dir)
            {
                switch (dir)
                {
                    case Move.Right:
                        for (int y = 0; y < Size; y++)
                        {
                            for (int x = Size - 2; x >= 0; x--) //we dont have to check the rightmost tile, since its content cant be moved
                            {
                                if (Playfield[x, y] != 0)
                                {// the cell isnt empty
                                    int maxMove = MaxMove(x, y, dir);
                                    if (maxMove > 0)
                                    {
                                        Playfield[x + maxMove, y] = Playfield[x, y];
                                        Playfield[x, y] = 0;
                                    }
                                    if (x + maxMove + 1 < 4)
                                    {
                                        if (Playfield[x + maxMove, y] == Playfield[x + maxMove + 1, y])
                                        {
                                            Playfield[x + maxMove + 1, y] *= 2;
                                            Playfield[x + maxMove, y] = 0;
                                        }
                                    }

                                }
                            }
                        }
                        break;

                    case Move.Left:
                        for (int y = 0; y < Size; y++)
                        {
                            for (int x = 1; x < Size; x++) //we dont have to check the leftmost tile, since its content cant be moved
                            {
                                if (Playfield[x, y] != 0)
                                {// the cell isnt empty
                                    int maxMove = MaxMove(x, y, dir);
                                    if (maxMove > 0)
                                    {
                                        Playfield[x - maxMove, y] = Playfield[x, y];
                                        Playfield[x, y] = 0;
                                    }
                                    if (x - maxMove - 1 >= 0)
                                    {
                                        if (Playfield[x - maxMove, y] == Playfield[x - maxMove - 1, y])
                                        {
                                            Playfield[x - maxMove - 1, y] *= 2;
                                            Playfield[x - maxMove, y] = 0;
                                        }
                                    }

                                }
                            }
                        }
                        break;
                    case Move.Up:
                        for (int x = 0; x < Size; x++)
                        {
                            for (int y = 1; y < Size; y++) //we dont have to check the top tile, since its content cant be moved
                            {
                                if (Playfield[x, y] != 0)
                                {// the cell isnt empty
                                    int maxMove = MaxMove(x, y, dir);
                                    if (maxMove > 0)
                                    {
                                        Playfield[x, y - maxMove] = Playfield[x, y];
                                        Playfield[x, y] = 0;
                                    }
                                    if (y - maxMove - 1 >= 0)
                                    {
                                        if (Playfield[x, y - maxMove] == Playfield[x, y - maxMove - 1])
                                        {
                                            Playfield[x, y - maxMove - 1] *= 2;
                                            Playfield[x, y - maxMove] = 0;
                                        }
                                    }

                                }
                            }
                        }
                        break;
                    case Move.Down:
                        for (int x = 0; x < Size; x++)
                        {
                            for (int y = Size - 2; y >= 0; y--) //we dont have to check the bottom tile, since its content cant be moved
                            {
                                if (Playfield[x, y] != 0)
                                {// the cell isnt empty
                                    int maxMove = MaxMove(x, y, dir);
                                    if (maxMove > 0)
                                    {
                                        Playfield[x, y + maxMove] = Playfield[x, y];
                                        Playfield[x, y] = 0;
                                    }
                                    if (y + maxMove + 1 < Size)
                                    {
                                        if (Playfield[x, y + maxMove] == Playfield[x, y + maxMove + 1])
                                        {
                                            Playfield[x, y + maxMove + 1] *= 2;
                                            Playfield[x, y + maxMove] = 0;
                                        }
                                    }

                                }
                            }
                        }
                        break;
                }
            }

            //TODO MAKE IT BETTER
            /// <summary>
            /// Returns max move distance for a tile. (Merge is not included)
            /// </summary>
            /// <param name="x">Tile X</param>
            /// <param name="y">Tile Y</param>
            /// <param name="dir">Move direction</param>
            /// <returns>Max move distance.</returns>
            private int MaxMove(int x, int y, Move dir)
            {
                switch (dir)
                {
                    case Move.Right:
                        for (int i = 0; i < Size; i++)
                        {
                            if (i + x + 1 < Size)
                            {//exists
                                if (Playfield[i + 1 + x, y] != 0)
                                {
                                    return i;
                                }
                            }
                            else return i;
                        }
                        return 0;
                    case Move.Left:
                        for (int i = 0; i < Size; i++)
                        {
                            if (x - i - 1 >= 0)
                            {//exists
                                if (Playfield[x - i - 1, y] != 0)
                                {
                                    return i;
                                }
                            }
                            else return i;
                        }
                        return 0;
                    case Move.Down:
                        for (int i = 0; i < Size; i++)
                        {
                            if (i + y + 1 < Size)
                            {//exists
                                if (Playfield[x, y + i + 1] != 0)
                                {
                                    return i;
                                }
                            }
                            else return i;
                        }
                        return 0;
                    case Move.Up:
                        for (int i = 0; i < Size; i++)
                        {
                            if (y - i - 1 >= 0)
                            {//exists
                                if (Playfield[x, y - i - 1] != 0)
                                {
                                    return i;
                                }
                            }
                            else return i;
                        }
                        return 0;
                }
                return 0;
            }

            /// <summary>
            /// Creates a new block at a random pos
            /// </summary>
            private void CreateNewBlock() //TODO new generator
            {
                while (true)
                {
                    int x = 0, y = 0;
                    x = rnd.Next(0, Size);
                    y = rnd.Next(0, Size);
                    if (Playfield[x, y] == 0)
                    {
                        Playfield[x, y] = rnd.Next(1, 2) * 2;
                        return;
                    }
                }
            }

            /// <summary>
            /// Waits for input
            /// </summary>
            /// <returns>Direction</returns>
            private Move GetInput()
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        return Move.Up;
                    case ConsoleKey.RightArrow:
                        return Move.Right;
                    case ConsoleKey.DownArrow:
                        return Move.Down;
                    case ConsoleKey.LeftArrow:
                        return Move.Left;
                }
                return Move.NoMove;
            }

            /// <summary>
            /// Direction enum
            /// </summary>
            enum Move
            {
                NoMove,
                Up,
                Right,
                Down,
                Left
            }

            /// <summary>
            /// (Bad Code BTW) converts tile number into charAttributes
            /// </summary>
            /// <param name="num">Char attributes</param>
            /// <returns>Console color</returns>
            private short NumToColor(int num)
            {
                switch (num)
                {
                    case (0):
                        return 0xFF;
                    case (2):
                        return 0xE0;
                    case (4):
                        return 0x60;
                    case (8):
                        return 0xC0;
                    case (16):
                        return 0x4F;
                    case (32):
                        return 0xDF;
                    case (64):
                        return 0x5F;
                    case (128):
                        return 0xB0;
                    case (256):
                        return 0x30;
                    case (512):
                        return 0x9F;
                    case (1024):
                        return 0x1F;
                    case (2048):
                        return 0xA0;
                }
                return 0x00;
            }

            private int getScore(ref int[,] playfield, int size)
            {
                int score = 0;
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                    {
                        score += playfield[x, y];
                    }
                return score;
            }
        }
    }

}
