namespace butnotquite.Core.Helpers
{
    using Defaults;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class IllegalMovesFilter
    {
        private static Chessboard position;
        private static List<Move> availableMoves;

        internal static List<Move> RemoveIllegalMoves(Chessboard currentPosition, List<Move> currentAvailableMoves)
        {
            position = currentPosition;
            availableMoves = currentAvailableMoves;

            RemoveIllegalMovesInCheck();
            RemoveMovesDueToPin();

            return availableMoves;
        }

        #region Checks

        private static void RemoveIllegalMovesInCheck()
        {
            if (!position.KingInCheck)
            {
                return;
            }

            HashSet<int> lineOfFire = GetLineOfFire();
            List<Move> kingMoves = availableMoves.Where(m => m.Direction == Direction.SingleSquare).ToList();

            availableMoves.RemoveAll(m => !lineOfFire.Contains(m.ToSquare));
            availableMoves.AddRange(kingMoves);
        }

        private static HashSet<int> GetLineOfFire()
        {
            int kingPosition = GetKingPosition();
            int attackerPosition = position.OpponentActivity
                .FirstOrDefault(kvp => kvp.Value.Contains(kingPosition))
                .Key
                .Position;
            HashSet<int> checkSquares = new HashSet<int>();

            if (position.Board[attackerPosition].OccupiedBy.Type == PieceType.Knight)
            {
                checkSquares.Add(attackerPosition);

                return checkSquares; // Can't block a knight's check
            }

            int kingRow = kingPosition / 8;
            int kingCol = kingPosition % 8;
            int attackerRow = attackerPosition / 8;
            int attackerCol = attackerPosition % 8;
            int increment = 0;

            if (kingRow == attackerRow)
            {
                increment = 1;
            }
            else if (kingCol == attackerCol)
            {
                increment = 8;
            }
            else
            {
                int positionDifference = Math.Abs(kingPosition - attackerPosition);
                increment = (positionDifference % 9 == 0) ? 9 : 7;
            }

            int startSquare = Math.Min(kingPosition, attackerPosition);
            int endSquare = Math.Max(kingPosition, attackerPosition);

            for (int checkSquare = startSquare + increment; checkSquare < endSquare; checkSquare += increment)
            {
                checkSquares.Add(checkSquare);
            }

            checkSquares.Add(attackerPosition);

            return checkSquares;
        }

        #endregion        

        #region Pins

        private static void RemoveMovesDueToPin()
        {
            IDictionary<int, List<Direction>> movesToRemove = new Dictionary<int, List<Direction>>();
            HashSet<int> coveredMoves = new HashSet<int>();

            for (int i = 0; i < availableMoves.Count; i++)
            {
                Move move = availableMoves[i];
                int fromSquare = move.FromSquare;

                if (coveredMoves.Contains(fromSquare))
                {
                    continue;
                }

                HashSet<Direction> illegalDirections = GetIllegalDirections(fromSquare);

                if (illegalDirections.Count != 0)
                {
                    if (!movesToRemove.ContainsKey(fromSquare))
                    {
                        movesToRemove.Add(fromSquare, new List<Direction>());
                    }

                    movesToRemove[fromSquare].AddRange(illegalDirections);
                }

                coveredMoves.Add(fromSquare);
            }

            availableMoves.RemoveAll(m =>
                movesToRemove
                .Any(
                    mtr => m.FromSquare == mtr.Key &&
                    mtr.Value.Contains(m.Direction)));
        }

        private static HashSet<Direction> GetIllegalDirections(int fromSquare)
        {
            HashSet<Direction> pinDirections = new HashSet<Direction>();
            List<int> attackerPositions = position.OpponentActivity.Where(kvp =>
               (kvp.Key.Type == PieceType.Queen ||
               kvp.Key.Type == PieceType.Rook ||
               kvp.Key.Type == PieceType.Bishop)
               && kvp.Value.Contains(fromSquare))
                .Select(kvp => kvp.Key.Position)
                .ToList();

            if (attackerPositions.Count == 0)
            {
                return pinDirections;
            }

            bool[] possiblePins = GetPossiblePins(fromSquare, attackerPositions);

            if (possiblePins.All(pinIsPossible => !pinIsPossible))
            {
                return pinDirections;
            }

            int kingPosition = GetKingPosition();
            int kingCol = kingPosition % 8;
            int pieceCol = fromSquare % 8;
            PieceType pieceType = position.Board[fromSquare].OccupiedBy.Type;

            for (int i = 0; i < possiblePins.Length; i++)
            {
                if (possiblePins[i])
                {
                    switch (i)
                    {
                        case 0: // First diagonal 
                            if (PinExists(fromSquare, Direction.DownLeftUpRight))
                            {
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);

                                if (kingCol != pieceCol)
                                {
                                    pinDirections.Add(Direction.Vertical);
                                }
                            }

                            break;
                        case 1: // Second diagonal
                            if (PinExists(fromSquare, Direction.DownRightUpLeft))
                            {
                                pinDirections.Add(Direction.DownRightUpLeft);
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);

                                if (kingCol != pieceCol)
                                {
                                    pinDirections.Add(Direction.Vertical);

                                    return pinDirections;
                                }
                            }

                            break;
                        case 2: // Vertical, same rank, so piece can't move up or down
                            if (PinExists(fromSquare, Direction.Vertical))
                            {
                                pinDirections.Add(Direction.Vertical);
                                pinDirections.Add(Direction.EnPassant);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.DownRightUpLeft);
                            }

                            break;
                        case 3: // Horizontal, same file, so piece can't move left or right 
                            if (PinExists(fromSquare, Direction.Horizontal))
                            {
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.DownRightUpLeft);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            return pinDirections;
        }

        private static bool[] GetPossiblePins(int fromSquare, List<int> attackersPositions)
        {
            int kingPosition = GetKingPosition();
            int kingRow = kingPosition / 8;
            int kingCol = kingPosition % 8;
            int pieceRow = fromSquare / 8;
            int pieceCol = fromSquare % 8;
            int squareDifferenceKingPiece = Math.Abs(fromSquare - kingPosition);
            bool[] possiblePins = new bool[4];

            for (int i = 0; i < attackersPositions.Count; i++)
            {
                int attackerPosition = attackersPositions[i];
                int squareDifferenceKingAttacker = Math.Abs(attackerPosition - kingPosition);
                int attackerRow = attackerPosition / 8;
                int attackerCol = attackerPosition % 8;               

                if (kingCol == pieceCol && pieceCol == attackerCol)
                {
                    possiblePins[3] = true;
                }
                else if (kingRow == pieceRow && pieceRow == attackerRow)
                {
                    possiblePins[2] = true;
                }
                else if ((squareDifferenceKingPiece % 7 == 0) && (squareDifferenceKingAttacker % 7 == 0))
                {
                    possiblePins[1] = true;
                }
                else if ((squareDifferenceKingPiece % 9 == 0) && (squareDifferenceKingAttacker % 9 == 0))
                {
                    possiblePins[0] = true;
                }
            }

            return possiblePins;
        }

        private static bool PinExists(int fromSquare, Direction potentialIllegalDirection)
        {
            int kingPosition = GetKingPosition();
            int squareDifference = Math.Abs(fromSquare - kingPosition);
            int minSquare = Math.Min(fromSquare, kingPosition);
            int maxSquare = Math.Max(fromSquare, kingPosition);
            int increment = 0;

            switch (potentialIllegalDirection)
            {
                case Direction.Vertical:
                    increment = 1;
                    break;
                case Direction.Horizontal:
                    increment = 8;
                    break;
                case Direction.DownLeftUpRight:
                case Direction.DownRightUpLeft:
                    increment = (squareDifference % 7 == 0) ? 7 : 9;
                    break;
                default:
                    return true; // Something went wrong; returns that a pin exists
            }

            minSquare += increment; // Otherwise we start at the piece

            for (int i = minSquare; i < maxSquare; i += increment)
            {
                if (position.Board[i].OccupiedBy.Type != PieceType.None) // There is a piece between the current piece and the king; no pin
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Helpers

        private static int GetKingPosition()
        {
            return (position.SideToMove == Color.White) ?
                position.WhiteKingPosition :
                position.BlackKingPosition;
        }

        #endregion
    }
}