using System;

namespace Poly2TriTextureExample
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TextureMappingExample game = new TextureMappingExample())
            {
                game.Run();
            }
        }
    }
}

