namespace butnotquite.Engine
{
    using Defaults;
    using Models;

    internal sealed class Engine
    {
        public static void Main()
        {
            Chessboard board = new Chessboard();

            board.InitializeStartingPosition();
            board.MakeMove(52, 36);
            board.GetAvailableMoves();
        }
    }
}
