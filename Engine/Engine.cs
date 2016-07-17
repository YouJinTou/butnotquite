namespace butnotquite.Engine
{
    using Core;
    using Models;
    using Utils;

    using System;

    internal sealed class Engine
    {
        public static void Main()
        {
            PlayEngineGame();
        }

        private static void PlayEngineGame()
        {
            Chessboard position = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            while (true)
            {
                Search.Initialize(position);

                position.MakeMove(new Move(
                    position.MaximizingSideBestMove.FromSquare, 
                    position.MaximizingSideBestMove.ToSquare, 
                    position.MaximizingSideBestMove.Direction));
                position.PrintBoard();

                Console.ReadLine();
            }
        }        
    }
}
