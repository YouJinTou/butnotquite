namespace butnotquite.Engine
{
    using Core;
    using Models;
    using Utils;

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
            Chessboard position = Utils.LoadPositionFromFenString("4kbnr/p1p1pppp/4b3/N7/3P4/2P1P3/5PPP/2B1KBNR b Kk - 2 32");
            position.PrintBoard();

            while (true)
            {
                Search.Initialize(position);

                if (position.MaximizingSideBestMove.Direction != Defaults.Direction.Castle)
                {
                    position.MakeMove(new Move(
                        position.MaximizingSideBestMove.FromSquare,
                        position.MaximizingSideBestMove.ToSquare,
                        position.MaximizingSideBestMove.Direction
                        ));
                }
                else
                {
                    position.MakeMove(new Move(
                        position.MaximizingSideBestMove.KingFromSquare,
                        position.MaximizingSideBestMove.KingToSquare,
                        position.MaximizingSideBestMove.RookFromSquare,
                        position.MaximizingSideBestMove.RookToSquare
                        ));
                }

                position.PrintBoard();
            }
        }
    }
}
