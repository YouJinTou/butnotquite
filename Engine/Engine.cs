namespace butnotquite.Engine
{
    using butnotquite.Core.Search;
    using butnotquite.Core.Search.Zobrist;
    using butnotquite.Models;
    using butnotquite.Utils;

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
            //Chessboard position = Utils.LoadPositionFromFenString("2k1b3/3r1pp1/2p4n/2P3K1/5P2/3B4/3B2PP/6NR w - - 7 70");
            //Chessboard position = Utils.LoadPositionFromFenString("4k3/7K/2p5/2P5/8/8/8/8 w - - 10 83");
            //Chessboard position = Utils.LoadPositionFromFenString("8/2kbrpp1/p1p5/2P2n2/5P2/6PB/4N2P/2B1K2R b K - 0 67");
            //Chessboard position = Utils.LoadPositionFromFenString("8/4k3/2p2pp1/p4n2/1rb2PB1/8/6PP/2B1K1NR w K - 8 81");
            //Chessboard position = Utils.LoadPositionFromFenString("4k3/1R2N1K1/8/8/8/8/8/8 b - - 4 151");
            //Chessboard position = Utils.LoadPositionFromFenString("3k4/R5K1/3N4/8/8/8/8/8 b - - 14 156");
            Chessboard position = Utils.LoadPositionFromFenString("8/4k3/5pp1/p1p2n2/1rb2PB1/8/6PP/2B1K1NR w K - 8 81");
            //Chessboard position = Utils.LoadPositionFromFenString("8/4k3/5pp1/p1p2B2/1rb2P2/8/6PP/2B1K1NR b K - 1 82");
            //Chessboard position = Utils.LoadPositionFromFenString("8/5k2/5pp1/p1p2n2/1rb2PB1/8/6PP/2B1K1NR w K - 8 81");
            //Chessboard position = Utils.LoadPositionFromFenString("8/1k6/5pp1/p1p5/1rb2P1P/3B4/3B2P1/4K1NR b K - 12 41");

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
                Console.WriteLine("\n\nMove: " + position.MoveCounter);
                Console.WriteLine("Fifty-move rule: " + position.FiftyMoveCounter);
                Console.WriteLine("Hash: " + ZobristHasher.GetZobristHash(position));
                Console.WriteLine("Side to move: " + position.SideToMove);
                Console.WriteLine("EnPassant square: " + position.EnPassantSquare);
                Console.WriteLine("Visited nodes: " + Search.VisitedNodes);
                Console.ReadLine();
            }
        }
    }
}
