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
            //Chessboard position = new Chessboard(false);
            //Chessboard position = Utils.LoadPositionFromFenString("1rbq1bnr/pp1kpppp/2p5/P1PP4/Q7/4P3/5PPP/RNB1KBNR w KQ - 0 15");
            //Chessboard position = Utils.LoadPositionFromFenString("r1k2bnr/p3pppp/8/2P5/4b3/8/3N1PPP/R1B1KBNR b KQ - 1 20"); // Castling problems
            //Chessboard position = Utils.LoadPositionFromFenString("R1rqkb1r/1Ppbpppp/7n/8/3P4/2P1P3/3N1PPP/2BQKBNR b KQkq - 2 25");
            //Chessboard position = Utils.LoadPositionFromFenString("r1bqkbnr/pppppppp/2P5/8/P2P4/2P1P3/5PPP/RNBQKBNR b KQkq - 0 12");
            //Chessboard position = Utils.LoadPositionFromFenString("1r2kbnr/pqpbpppp/8/8/3P4/2P1P3/3N1PPP/1RBQKBNR b Kk - 2 30");
            //Chessboard position = Utils.LoadPositionFromFenString("4kbnr/p1p1pppp/4b3/N7/3P4/2P1P3/5PPP/2B1KBNR b Kk 16 2 32");
            //Chessboard position = Utils.LoadPositionFromFenString("8/3nbkpp/p2rp3/8/K7/2P5/4BPPP/2B3NR w - - 10 42");
            //Chessboard position = Utils.LoadPositionFromFenString("8/8/5k2/K3R1p1/6p1/2r5/7P/6N1 w - - 10 48");
            //Chessboard position = Utils.LoadPositionFromFenString("2k2bnr/pr2pppp/2P5/8/4b3/8/3N1PPP/R1B1KBNR w KQ - 5 21");
            //Chessboard position = Utils.LoadPositionFromFenString("6r1/pk1b1pp1/2p5/2P2n1p/8/8/5PPP/2B1KBNR w K - 3 54");
            //Chessboard position = Utils.LoadPositionFromFenString("4r3/pk1b1pp1/2p5/2P2n1B/8/8/5PPP/2B1K1NR w K - 0 56"); //Problematic
            Chessboard position = Utils.LoadPositionFromFenString("2k1b3/3r1pp1/2p4n/2P3K1/5P2/3B4/3B2PP/6NR w - - 7 70");
            position.PrintBoard();

            while (true)
            {
                Search.Initialize(position);

                if (position.MaximizingSideBestMove.Direction != Defaults.Direction.Castle)
                {
                    Move bestMove = new Move(
                        position.MaximizingSideBestMove.FromSquare,
                        position.MaximizingSideBestMove.ToSquare,
                        position.MaximizingSideBestMove.Direction);

                    //if (bestMove.FromSquare == position.LastMove.FromSquare && bestMove.ToSquare == position.LastMove.ToSquare)
                    //{
                    //    Console.WriteLine("Draw agreed.");

                    //    break;
                    //}

                    position.MakeMove(bestMove);
                }
                else
                {
                    Move bestMove = new Move(
                        position.MaximizingSideBestMove.KingFromSquare,
                        position.MaximizingSideBestMove.KingToSquare,
                        position.MaximizingSideBestMove.RookFromSquare,
                        position.MaximizingSideBestMove.RookToSquare);

                    position.MakeMove(bestMove);
                }

                position.PrintBoard();
                Console.WriteLine("\nMove: " + position.MoveCounter);
                Console.WriteLine("Fifty-move rule: " + position.FiftyMoveCounter);
                Console.WriteLine("Hash: " + butnotquite.Core.Zobrist.ZobristHasher.GetZobristHash(position));
                Console.ReadLine();
            }
        }
    }
}
