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
            //Chessboard position = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            //Chessboard position = Utils.LoadPositionFromFenString("1rbq1bnr/pp1kpppp/2p5/P1PP4/Q7/4P3/5PPP/RNB1KBNR w KQ - 0 15");
            Chessboard position = Utils.LoadPositionFromFenString("r1k2bnr/p3pppp/8/2P5/4b3/8/3N1PPP/R1B1KBNR b KQ - 1 20"); // Castling problems

            position.PrintBoard();

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
