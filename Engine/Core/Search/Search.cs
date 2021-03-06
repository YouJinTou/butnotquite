﻿namespace Engine.Core.Search
{
    using Engine.Core.Search.Models;
    using Engine.Core.Search.Helpers;
    using Engine.Core.Search.Zobrist;
    using Engine.Defaults;
    using Engine.Models;

    using System.Collections.Generic;
    using System.Linq;

    internal static class Search
    {
        internal static int VisitedNodes;

        private static Chessboard position;
        private static int maxDepth;
        private static Color maximizingSide;
        private static IDictionary<int, Move[]> killerMoves;

        internal static void Initialize(Chessboard currentPosition, int depth)
        {
            VisitedNodes = 0;
            position = currentPosition;
            maxDepth = depth;
            maximizingSide = position.SideToMove;
            killerMoves = new Dictionary<int, Move[]>();

            position.TranspositionTable.Clear();

            DoAlphaBetaPruning(0, int.MinValue, int.MaxValue);
        }

        private static int DoAlphaBetaPruning(int depth, int alpha, int beta)
        {
            VisitedNodes++;

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            position.PieceActivity = availableMoves.Count;
            int gameStateScore = GetGameStateScore(position, availableMoves.Count, depth);

            if (gameStateScore != 9999)
            {
                return gameStateScore;
            }

            MoveSorter.SortMoves(position, availableMoves, killerMoves, depth);

            for (int moveIndex = 0; moveIndex < availableMoves.Count; moveIndex++)
            {
                Move currentMove = availableMoves[moveIndex];

                position.MakeMove(currentMove);

                long zobristKey = ZobristHasher.GetZobristHash(position);

                if (!position.GameHistory.ContainsKey(zobristKey))
                {
                    position.GameHistory.Add(zobristKey, 1);
                }
                else
                {
                    position.GameHistory[zobristKey]++;
                }

                int score =
                    (position.TranspositionTable.ContainsKey(zobristKey) && 
                    position.TranspositionTable[zobristKey].Depth >= depth) ?
                    position.TranspositionTable[zobristKey].Score :
                    DoAlphaBetaPruning(depth + 1, alpha, beta);

                position.UndoMove(currentMove);
                position.GameHistory.Remove(zobristKey);

                if (position.SideToMove == maximizingSide)
                {
                    if (score >= beta)
                    {
                        TryAddKillerMove(currentMove, depth);

                        return beta;
                    }

                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }

                        UpdatePrincipalVariation(depth, currentMove);
                        TryAddTableEntry(zobristKey, score, depth, currentMove);
                    }
                }
                else
                {
                    if (score <= alpha)
                    {
                        return alpha;
                    }

                    if (score <= beta)
                    {
                        beta = score;

                        UpdatePrincipalVariation(depth, currentMove);
                    }
                }
            }

            return (position.SideToMove == maximizingSide) ? alpha : beta;
        }

        #region Helpers

        private static int GetGameStateScore(Chessboard position, int availalbeMovesCount, int depth)
        {
            if (ThreefoldRepetitionEnforceable(position) || position.FiftyMoveCounter >= 100)
            {
                return 0;
            }

            if (availalbeMovesCount != 0)
            {
                if (depth == maxDepth)
                {
                    return InvertScore(Evaluator.EvaluatePosition(position));
                }

                return 9999;
            }

            if (position.SideToMove == maximizingSide)
            {
                if (position.KingInCheck)
                {
                    return int.MinValue + depth + 1;
                }
            }
            else
            {
                if (position.KingInCheck)
                {
                    return int.MaxValue - depth - 1;
                }
            }

            return 0; // Stalemate
        }

        private static bool ThreefoldRepetitionEnforceable(Chessboard position)
        {
            if (position.Board[position.LastMove.ToSquare].OccupiedBy.Type == PieceType.Pawn)
            {
                return false;
            }

            long zobristKey = ZobristHasher.GetZobristHash(position);
            bool drawEnforcable = position.GameHistory.ContainsKey(zobristKey) ? 
                position.GameHistory[zobristKey] >= 3 : false;

            return drawEnforcable;
        }

        private static bool InsufficientMaterial()
        {
            return false;
        }

        private static int InvertScore(int score)
        {
            return (maximizingSide == Color.White) ? score : -score;
        }

        private static void TryAddTableEntry(long zobristKey, int score, int depth, Move bestMove)
        {
            if (!position.TranspositionTable.ContainsKey(zobristKey))
            {
                position.TranspositionTable.Add(zobristKey, new TableEntry(depth, score, bestMove));
            }
        }

        private static void TryAddKillerMove(Move currentMove, int depth)
        {            
            if (currentMove.Direction == Direction.Castle || 
                position.Board[currentMove.ToSquare].OccupiedBy.Type == PieceType.None)
            {
                if (!killerMoves.ContainsKey(depth))
                {
                    killerMoves.Add(depth, new Move[2]);
                    killerMoves[depth][0] = Utils.Utils.MakeDeepCopy(currentMove);
                }
                else
                {
                    if (killerMoves[depth][1].Id == 0)
                    {
                        killerMoves[depth][1] = Utils.Utils.MakeDeepCopy(currentMove);
                    }
                    else
                    {
                        killerMoves[depth][0] = Utils.Utils.MakeDeepCopy(currentMove);
                    }
                }
            }
        }

        private static void UpdatePrincipalVariation(int depth, Move bestMove)
        {
            if (!position.PrincipalVariation.ContainsKey(depth))
            {
                position.PrincipalVariation.Add(depth, bestMove);
            }
            else
            {
                position.PrincipalVariation[depth] = bestMove;
            }
        }

        #endregion
    }
}