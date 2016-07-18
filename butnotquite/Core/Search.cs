namespace butnotquite.Core
{
    using Defaults;
    using Models;

    using System.Collections.Generic;

    internal static class Search
    {
        internal const int MaxDepth = 4;

        private static Color maximizingSide;

        internal static void Initialize(Chessboard position)
        {
            maximizingSide = position.SideToMove;
            position.MaximizingSideBestMove = new Move();

            GetAlphaBetaScore(0, int.MinValue, int.MaxValue, position);
        }

        private static int GetAlphaBetaScore(int depth, int alpha, int beta, Chessboard position)
        {
            if (depth == MaxDepth)
            {
                return Evaluator.EvaluatePosition(position);
            }

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            int gameStateScore = GetGameStateScore(position, availableMoves);

            if (gameStateScore != -1)
            {
                return gameStateScore;
            }

            for (int i = 0; i < availableMoves.Count; i++)
            {
                Move currentMove = availableMoves[i];

                position.MakeMove(currentMove);

                int score = GetAlphaBetaScore(depth + 1, alpha, beta, position);

                position.UndoMove(currentMove);

                if (position.SideToMove == maximizingSide)
                {
                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }
                    }       
                }
                else
                {
                    if (score <= beta)
                    {
                        beta = score;
                    }
                }

                if (alpha >= beta)
                {
                    return (position.SideToMove == maximizingSide) ? alpha : beta;
                }
            }

            return (position.SideToMove == maximizingSide) ? alpha : beta;
        }

        private static int GetGameStateScore(Chessboard position, List<Move> availableMoves)
        {
            if (position.RepetitionEnforcable || position.FiftyMoveEnforcable)
            {
                return 0;
            }

            if (availableMoves.Count != 0)
            {
                return -1;
            }

            if (position.SideToMove == Color.White)
            {
                if (position.WhiteInCheck)
                {
                    return int.MinValue;
                }
            }
            else
            {
                if (position.BlackInCheck)
                {
                    return int.MaxValue;
                }
            }

            return 0; // Stalemate
        }
    }
}