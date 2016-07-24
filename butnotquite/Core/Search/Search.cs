namespace butnotquite.Core.Search
{
    using butnotquite.Core.Search.Models;
    using butnotquite.Models;
    using butnotquite.Defaults;
    using butnotquite.Core.Search.Zobrist;

    using System.Collections.Generic;
    using System.Linq;

    internal static class Search
    {
        private const int MaxDepth = 7;

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

            availableMoves = SortMoves(availableMoves);

            for (int moveIndex = 0; moveIndex < availableMoves.Count; moveIndex++)
            {
                Move currentMove = availableMoves[moveIndex];

                position.MakeMove(currentMove);

                int gameStateScore = GetGameStateScore(position, availableMoves.Count, false);
                int uneventfulScore = -1;
                int score = (gameStateScore == uneventfulScore) ? uneventfulScore : gameStateScore;
                long zobristKey = ZobristHasher.GetZobristHash(position);

                position.GameHistory.Push(zobristKey);

                if (score == uneventfulScore)
                {
                    if (!position.TranspositionTable.ContainsKey(zobristKey))
                    {
                        score = DoAlphaBetaPruning(depth + 1, alpha, beta, position);
                    }
                    else
                    {
                        TableEntry entry = position.TranspositionTable[zobristKey];

                        if (entry.Depth >= depth)
                        {
                            score = entry.Score;
                        }
                        else
                        {
                            score = DoAlphaBetaPruning(depth + 1, alpha, beta, position);
                        }
                    }
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

            return alpha;
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

            if (position.SideToMove == maximizingSide)
            {
                if (position.KingInCheck)
                {
                    return int.MinValue + 1;
                }
            }
            else
            {
                if (position.KingInCheck)
                {
                    return int.MaxValue - 1;
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