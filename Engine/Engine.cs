namespace butnotquite.Engine
{
    using Models;

    internal sealed class Engine
    {
        public static void Main()
        {
            Chessboard board = new Chessboard();

            board.InitializeStartingPosition();
            board.GetAvailableMoves();
            board.MakeMove(52, 36);
            board.GetAvailableMoves();
        }
    }
}
