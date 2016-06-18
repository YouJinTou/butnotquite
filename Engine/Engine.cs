namespace butnotquite.Engine
{
    using Core;
    using Models;
    using Utils;

    internal sealed class Engine
    {
        public static void Main()
        {
            Chessboard board = Utils.LoadPositionFromFenString("1q3n1r/P5P1/3k4/8/8/NnP1p1p1/PP3P11/5K2 w - - 0 50 ");
            //Chessboard board = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            MoveGenerator.GetAvailableMoves(board);
            board.MakeMove(52, 36);
            MoveGenerator.GetAvailableMoves(board);
        }
    }
}
