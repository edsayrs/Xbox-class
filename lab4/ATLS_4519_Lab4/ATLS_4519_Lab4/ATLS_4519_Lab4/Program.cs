using System;

namespace ATLS_4519_lab4
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ATLS_4519_Lab4.Game1 game = new ATLS_4519_Lab4.Game1())
            {
                game.Run();
            }
        }
    }
#endif
}