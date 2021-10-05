using System;
using MGLib;

namespace LD49ogl
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1("LD49"))
                game.Run();
        }
    }
}