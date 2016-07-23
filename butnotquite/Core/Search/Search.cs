﻿namespace butnotquite.Core.Search
{
    using butnotquite.Models;
    using Defaults;
    using Zobrist;

    using System.Collections.Generic;
    using System.Linq;

    internal static class Search
    {
        private const int MaxDepth = 7;

        private static Color maximizingSide;
        internal static int VisitedNodes;

        internal static void Initialize(Chessboard position)
        {
            maximizingSide = position.SideToMove;

            DoAlphaBetaPruning(0, int.MinValue, int.MaxValue, position);
        }

        private static int DoAlphaBetaPruning(int depth, int alpha, int beta, Chessboard position)
        {
            VisitedNodes++;

            if (depth == MaxDepth)
            {
                return InvertScore(Evaluator.EvaluatePosition(position));
            }

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            int gameStateScoreBeforeMoving = GetGameStateScore(position, availableMoves.Count, true);

            if (gameStateScoreBeforeMoving != -1)
            {
                return gameStateScoreBeforeMoving;
            }

            for (int moveIndex = 0; moveIndex < availableMoves.Count; moveIndex++)
            {
                Move currentMove = availableMoves[moveIndex];

                position.MakeMove(currentMove);

                int score = 0;
                long zobristKey = ZobristHasher.GetZobristHash(position);

                position.GameHistory.Push(zobristKey);

                if (!position.TranspositionTable.ContainsKey(zobristKey))
                {
                    int gameStateScore = GetGameStateScore(position, availableMoves.Count, false);
                    score = (gameStateScore == -1) ? DoAlphaBetaPruning(depth + 1, alpha, beta, position) : gameStateScore;

                    if (!position.TranspositionTable.ContainsKey(zobristKey))
                    {
                        position.TranspositionTable.Add(zobristKey, score);
                    }
                }
                else
                {
                    score = position.TranspositionTable[zobristKey];
                }

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

        #region Helpers

        private static int InvertScore(int score)
        {
            return (maximizingSide == Color.White) ? score : -score;
        }

        private static int GetGameStateScore(Chessboard position, int availalbeMovesCount, bool beforeMoving)
        {
            if (!beforeMoving)
            {
                if (ThreefoldRepetitionEnforcable(position) || position.FiftyMoveCounter >= 100)
                {
                    return 0;
                }
            }

            if (availalbeMovesCount != 0)
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

        private static bool ThreefoldRepetitionEnforcable(Chessboard position)
        {
            bool drawEnforcable = (position.GameHistory
                .GroupBy(pos => pos)
                .Any(group => group.Count() >= 3));

            return drawEnforcable;
        }

        #endregion
    }
}