namespace butnotquite.Core.Search.Helpers
{
    using butnotquite.Models;
    using Defaults;   

    using System.Collections.Generic;
    using System.Linq;

    internal static class MoveSorter
    {
        private static Chessboard position;
        private static IDictionary<int, Move[]> killerMoves;

        internal static List<Move> SortMoves(Chessboard currentPosition, List<Move> availableMoves, IDictionary<int, Move[]> currentKillerMoves, int depth)
        {
            List<Move> sortedMoves = new List<Move>();
            position = currentPosition;
            killerMoves = currentKillerMoves;
            Move pVMove = GetPrincipalVariationMove(depth);

            sortedMoves.AddRange(SortByMvvLva(availableMoves));
            sortedMoves.AddRange(SortByKillerMoves(availableMoves, depth));

            if (pVMove.Id != 0)
            {
                if (availableMoves.Any(m => m.FromSquare == pVMove.FromSquare && m.ToSquare == pVMove.ToSquare))
                {
                    sortedMoves.Add(pVMove);
                }
            }

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

        private static Move GetPrincipalVariationMove(int depth)
        {
            if (position.PrincipalVariation.ContainsKey(depth))
            {
                return position.PrincipalVariation[depth];
            }

            return new Move();
        }
    }
}
