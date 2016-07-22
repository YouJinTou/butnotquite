namespace butnotquite.Core
{
    using Defaults;
    using Models;

    internal static class Evaluator
    {
        private static Chessboard position;

        internal static int EvaluatePosition(Chessboard currentPosition)
        {
            int evaluation = 0;
            position = currentPosition;
            evaluation += EvaluateMaterial();

            return evaluation;
        }

        private static int EvaluateMaterial()
        {
            int sideToMoveScore = 0;
            int oppositeSideScore = 0;

            for (int i = 0; i < position.Board.Length; i++)
            {
                Piece currentPiece = position.Board[i].OccupiedBy;

                if (currentPiece.Type == PieceType.None)
                {
                    continue;
                }

                if (currentPiece.Color == Color.White)
                {
                    sideToMoveScore += currentPiece.Value;
                }
                else
                {
                    oppositeSideScore -= currentPiece.Value;
                }
            }

            return (sideToMoveScore + oppositeSideScore);
        }
    }
}
