using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _2k48
{
    class Program
    {
        public static Congine engine;
        
        static void Main(string[] args)
        {
            engine = new Congine(42,22);
            Game.Game g = new Game.Game(ref engine, 0);
            g.Run();
                        
        }

    }
    
}
