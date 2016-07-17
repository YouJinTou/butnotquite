namespace butnotquite.Core
{
    using Defaults;
    using Models;

    internal static class Evaluator
    {
        private static Chessboard position;

        internal static int EvaluatePosition(Chessboard currentPosition)
        {
            position = currentPosition;
            int evaluation = 0;

            evaluation += EvaluateMaterial();

            return evaluation;
        }

        private static int EvaluateMaterial()
        {
            int materialScore = 0;

            for (int i = 0; i < position.Board.Length; i++)
            {
                Piece currentPiece = position.Board[i].OccupiedBy;

                if (currentPiece.Type == PieceType.None || currentPiece.Color != position.SideToMove)
                {
                    continue;
                }

                materialScore += currentPiece.Value;
            }

            return materialScore;
        }
    }
}
