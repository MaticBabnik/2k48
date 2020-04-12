using System;

namespace _2k48
{
    class Program
    {
        public static Congine engine;        

        /// <summary>
        /// Entry point for the program
        /// </summary>
        /// <param name="args">Console args</param>
        static void Main(string[] args)
        {
            Console.Clear();
            Console.CursorVisible = false;
            engine = new Congine(55,21);
            Console.SetCursorPosition(0, 23);
            Game.Game g = new Game.Game(ref engine, 4); //auto shows logo

            Console.ReadKey(); //press key to start

            var s = g.Run(); //! gamedev = ez

            Console.WriteLine("Score: {0}", s);
            Console.ReadKey(true);
            
        }
    }   
}