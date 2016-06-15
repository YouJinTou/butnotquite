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
            board.GetAvailableMoves(Color.White);
        }
    }
}
