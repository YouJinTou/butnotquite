namespace butnotquite.Core
{
    using Defaults;
    using Models;

    using System.Collections.Generic;

    internal static class Search
    {
        internal const int MaxDepth = 4;

        internal static int DoAlphaBetaPruning(int depth, int alpha, int beta, Chessboard position)
        {
            if (depth > MaxDepth)
            {
                return Evaluator.EvaluatePosition(position);
            }

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            if (availableMoves.Count == 0 && !position.Stalemate)
            {
                return (position.SideToMove == Color.White) ? int.MinValue : int.MaxValue;
            }

            for (int i = 0; i < availableMoves.Count; i++)
            {
                Move currentMove = availableMoves[i];

                position.MakeMove(currentMove.FromSquare, currentMove.ToSquare, currentMove.Direction);

                int score = DoAlphaBetaPruning(depth + 1, alpha, beta, position);

                if (score >= beta)
                {
                    return beta;
                }

                if (score > alpha)
                {

                }

                position.UndoMove(currentMove);
            }

            return alpha;
        }
    }
}
