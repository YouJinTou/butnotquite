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

            availableMoves = SortMoves(availableMoves, depth);

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
                        TryAddTableEntry(zobristKey, score, depth);

                        return beta;
                    }

                    if (score > alpha)
                    {
                        alpha = score;

                        if (depth == 0)
                        {
                            position.MaximizingSideBestMove = currentMove;
                        }

                        TryAddTableEntry(zobristKey, score, depth);
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

        private static List<Move> SortMoves(List<Move> availableMoves, int depth)
        {
            List<Move> sortedMoves = new List<Move>();

            sortedMoves.AddRange(SortByMvvLva(availableMoves));
            sortedMoves.AddRange(SortByKillerMoves(availableMoves, depth));

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
                m => m.Direction != Direction.Castle && position.Board[m.ToSquare].OccupiedBy.Value != 0)
                .ToList();
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

        private static List<Move> SortByKillerMoves(List<Move> availableMoves, int depth)
        {
            List<Move> castleKillers = availableMoves.Where(m => m.Direction == Direction.Castle).ToList();
            List<Move> killers = availableMoves.Where(
                m => m.Direction != Direction.Castle && position.Board[m.ToSquare].OccupiedBy.Value == 0)
                .ToList();
            List<Move> sortedKillers = new List<Move>();

            killers.AddRange(castleKillers);

            if (killerMoves.ContainsKey(depth))
            {
                Move[] killerMovesAtPly = killerMoves[depth];

                for (int i = 0; i < killerMovesAtPly.Length; i++)
                {
                    if (killers.Any(k => k.Id == killerMovesAtPly[i].Id))
                    {
                        sortedKillers.Add(killerMovesAtPly[i]);
                    }
                }
            }

            return sortedKillers;
        }

        #endregion

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

        private static void TryAddTableEntry(long zobristKey, int score, int depth)
        {
            if (!position.TranspositionTable.ContainsKey(zobristKey))
            {
                position.TranspositionTable.Add(zobristKey, new TableEntry(depth, score));
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