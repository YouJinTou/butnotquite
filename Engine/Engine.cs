namespace butnotquite.Engine
{
    using Core;
    using Models;
    using Utils;

    using System;
    using System.Diagnostics;

    internal static class Engine
    {
        public static void Main()
        {
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
