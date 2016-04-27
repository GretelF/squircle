using System.Globalization;
using System.Threading;

namespace Squircle
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
#endif
}

