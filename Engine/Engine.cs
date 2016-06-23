namespace butnotquite.Engine
{
    using Core;
    using Defaults;
    using Models;
    using Utils;

    using System;
    using System.Diagnostics;

    internal static class Engine
    {
        public static void Main()
        {
            Chessboard position = Utils.LoadPositionFromFenString("1q3n1r/P5P1/3k4/8/8/NnP1p1p1/PP3P11/5K2 w - - 0 50");
            Stopwatch sw = new Stopwatch();

            sw.Start();

            int score = Search.GetAlphaBeta(0, int.MinValue, int.MaxValue, position);
            
            sw.Stop();

            Console.WriteLine("Final score: {0}",score);
            Console.WriteLine(sw.Elapsed);
        }

        private static void PlayEngineGame()
        {
            Chessboard startingPosition = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            while (true)
            {

            }
        }

        private static void PrintPosition(Chessboard position)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {

                }
            }
        }
    }
}
