namespace butnotquite.Core.Search
{
    using butnotquite.Core.Search.Models;
    using butnotquite.Core.Search.Helpers;
    using butnotquite.Core.Search.Zobrist;
    using butnotquite.Defaults;
    using butnotquite.Models;

    using System.Collections.Generic;
    using System.Linq;

    internal static class Search
    {
        private const int MaxDepth = 6;

        internal static int VisitedNodes;

        private static Chessboard position;
        private static Color maximizingSide;
        private static IDictionary<int, Move[]> killerMoves;

        internal static void Initialize(Chessboard currentPosition)
        {
            VisitedNodes = 0;
            position = currentPosition;
            maximizingSide = position.SideToMove;
            killerMoves = new Dictionary<int, Move[]>();

            position.TranspositionTable.Clear();

            DoAlphaBetaPruning(0, int.MinValue, int.MaxValue, position);
        }

        private static int DoAlphaBetaPruning(int depth, int alpha, int beta, Chessboard position)
        {
            VisitedNodes++;

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            int gameStateScore = GetGameStateScore(position, availableMoves.Count, depth);

            if (gameStateScore != -1)
            {
                return gameStateScore;
            }

            availableMoves = MoveSorter.SortMoves(position, availableMoves, killerMoves, depth);

            for (int moveIndex = 0; moveIndex < availableMoves.Count; moveIndex++)
            {
                Move currentMove = availableMoves[moveIndex];

                position.MakeMove(currentMove);

                long zobristKey = ZobristHasher.GetZobristHash(position);

                position.GameHistory.Push(zobristKey);

                int score =
                    (position.TranspositionTable.ContainsKey(zobristKey) && 
                    position.TranspositionTable[zobristKey].Depth >= depth) ?
                    position.TranspositionTable[zobristKey].Score :
                    DoAlphaBetaPruning(depth + 1, alpha, beta, position);

                position.UndoMove(currentMove);
                position.GameHistory.Pop();

                if (position.SideToMove == maximizingSide)
                {
                    if (score >= beta)
                    {
                        TryAddKillerMove(currentMove, depth);
                        // TryAddTableEntry(zobristKey, score, depth, currentMove);

                        return beta;
                    }

                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }

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
                    }
                }
            }

            return (position.SideToMove == maximizingSide) ? alpha : beta;
        }

        #region Helpers

        private static int GetGameStateScore(Chessboard position, int availalbeMovesCount, int depth)
        {
            if (ThreefoldRepetitionEnforcable(position) || position.FiftyMoveCounter >= 100)
            {
                return 0;
            }

            if (availalbeMovesCount != 0)
            {
                if (depth == MaxDepth)
                {
                    return InvertScore(Evaluator.EvaluatePosition(position));
                }

                return -1;
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

        private static bool ThreefoldRepetitionEnforcable(Chessboard position)
        {
            bool drawEnforcable = (position.GameHistory
                .GroupBy(pos => pos)
                .Any(group => group.Count() >= 3));

            return drawEnforcable;
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

        #endregion
    }
}