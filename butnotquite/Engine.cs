namespace butnotquite.Engine
{
    using butnotquite.Objects;

    internal sealed class Engine
    {
        public static void Main()
        {
            Chessboard board = new Chessboard();

            board.InitializeStartingPosition();
            board.GetAvailableMoves(Defaults.Color.White);
        }
    }
}
