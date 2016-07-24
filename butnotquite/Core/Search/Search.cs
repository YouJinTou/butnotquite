namespace butnotquite.Core.Search
{
    using butnotquite.Core.Search.Models;
    using butnotquite.Core.Search.Zobrist;
    using butnotquite.Defaults;
    using butnotquite.Models;

    using System.Collections.Generic;
    using System.Linq;

    internal static class Search
    {
        private const int MaxDepth = 4;

        internal static int VisitedNodes;

        private static Chessboard position;
        private static Color maximizingSide;

        internal static void Initialize(Chessboard currentPosition)
        {
            position = currentPosition;
            maximizingSide = position.SideToMove;
            VisitedNodes = 0;

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

            availableMoves = SortMoves(availableMoves);

            for (int moveIndex = 0; moveIndex < availableMoves.Count; moveIndex++)
            {
                Move currentMove = availableMoves[moveIndex];

                position.MakeMove(currentMove);

                int score = 0;
                long zobristKey = ZobristHasher.GetZobristHash(position);

                position.GameHistory.Push(zobristKey);

                if (!position.TranspositionTable.ContainsKey(zobristKey))
                {
                    score = DoAlphaBetaPruning(depth + 1, alpha, beta, position);
                }
                else
                {
                    score = position.TranspositionTable[zobristKey].Score;
                }

                position.UndoMove(currentMove);

                position.GameHistory.Pop();

                if (position.SideToMove == maximizingSide)
                {
                    if (score >= beta)
                    {
                        if (!position.TranspositionTable.ContainsKey(zobristKey))
                        {
                            position.TranspositionTable.Add(zobristKey, new TableEntry(depth, score));
                        }

                        return beta;
                    }

                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }

                        if (!position.TranspositionTable.ContainsKey(zobristKey))
                        {
                            position.TranspositionTable.Add(zobristKey, new TableEntry(depth, score));
                        }
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

        #region Move Ordering

        private static List<Move> SortMoves(List<Move> availableMoves)
        {
            List<Move> sortedMoves = new List<Move>();

            sortedMoves.AddRange(SortByMvvLva(availableMoves));

            for (int i = 0; i < sortedMoves.Count; i++)
            {
                availableMoves.Remove(sortedMoves[i]);
            }

            availableMoves.InsertRange(0, sortedMoves);

            return availableMoves;
        }

        private static List<Move> SortByMvvLva(List<Move> availableMoves)
        {
            List<Move> captures = availableMoves.Where(
                m => position.Board[m.ToSquare].OccupiedBy.Value != 0).ToList();
            int[] captureScores = new int[captures.Count];
            Move[] sortedCaptures = new Move[captures.Count];

            for (int i = 0; i < captures.Count; i++)
            {
                int valueDifference =
                    position.Board[captures[i].FromSquare].OccupiedBy.Value -
                    position.Board[captures[i].ToSquare].OccupiedBy.Value;
                captureScores[i] = valueDifference;
            }

            captureScores = captureScores.OrderBy(s => s).ToArray();

            for (int i = 0; i < captures.Count; i++)
            {
                sortedCaptures[i] = captures.FirstOrDefault(c =>
                    (position.Board[c.FromSquare].OccupiedBy.Value -
                    position.Board[c.ToSquare].OccupiedBy.Value)
                    == captureScores[i]);
            }

            return sortedCaptures.ToList();
        }

        #endregion

        #region Helpers

        private static int InvertScore(int score)
        {
            return (maximizingSide == Color.White) ? score : -score;
        }

        private static int GetGameStateScore(Chessboard position, int availalbeMovesCount, int depth)
        {
            if (depth == MaxDepth)
            {
                return InvertScore(Evaluator.EvaluatePosition(position));
            }

            if (ThreefoldRepetitionEnforcable(position) || position.FiftyMoveCounter >= 100)
            {
                return 0;
            }

            if (availalbeMovesCount != 0)
            {
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

        #endregion
    }
}