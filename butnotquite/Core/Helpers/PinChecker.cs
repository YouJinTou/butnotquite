namespace butnotquite.Core.Helpers
{
    using Defaults;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class PinChecker
    {
        private static Chessboard position;

        internal static void RemoveIllegalMoves(Chessboard currentPosition, List<Move> availableMoves)
        {
            IDictionary<int, List<Direction>> movesToRemove = new Dictionary<int, List<Direction>>();
            HashSet<int> coveredMoves = new HashSet<int>();
            position = currentPosition;

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
            List<int> attackers = position.OpponentActivity.Where(kvp =>
               (kvp.Key.Type == PieceType.Queen ||
               kvp.Key.Type == PieceType.Rook ||
               kvp.Key.Type == PieceType.Bishop)
               && kvp.Value.Contains(fromSquare))
                .Select(kvp => kvp.Key.Position)
                .ToList();

            if (attackers.Count == 0)
            {
                return pinDirections;
            }

            bool[] possiblePins = GetPossiblePins(fromSquare, attackers);

            if (possiblePins.All(pinIsPossible => !pinIsPossible))
            {
                return pinDirections;
            }

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
                                pinDirections.Add(Direction.Vertical);
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);
                            }
                            break;
                        case 1: // Second diagonal
                            if (PinExists(fromSquare, Direction.DownRightUpLeft))
                            {
                                pinDirections.Add(Direction.DownRightUpLeft);
                                pinDirections.Add(Direction.Vertical);
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);

                                return pinDirections;
                            }
                            break;
                        case 2: // Vertical, same rank, so piece can't move up or down
                            if (PinExists(fromSquare, Direction.Vertical))
                            {
                                pinDirections.Add(Direction.Vertical);
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.DownRightUpLeft);
                                pinDirections.Add(Direction.EnPassant);
                                pinDirections.Add(Direction.L); // If the knight is pinned, it simply can't move
                            }
                            break;
                        case 3: // Horizontal, same file, so piece can't move left or right 
                            if (PinExists(fromSquare, Direction.Horizontal))
                            {
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.DownRightUpLeft);
                                pinDirections.Add(Direction.L);
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
            bool verticalPinPosible = (kingRow == pieceRow);
            bool horizontalPinPossible = (kingCol == pieceCol);
            bool downRightUpLeftPinPossible = (Math.Abs(fromSquare - kingPosition) % 7 == 0);
            bool downLeftUpRightPinPossible = (Math.Abs(fromSquare - kingPosition) % 9 == 0);

            bool[] tempPossiblePins = new bool[4]
            {
                downLeftUpRightPinPossible,
                downRightUpLeftPinPossible,
                verticalPinPosible,
                horizontalPinPossible
            };
            bool[] actualPossiblePins = new bool[4];

            if (tempPossiblePins.All(pin => !pin))
            {
                return actualPossiblePins;
            }

            for (int i = 0; i < attackersPositions.Count; i++)
            {
                int attackerPosition = attackersPositions[i];
                int squareDifference = Math.Abs(attackerPosition - kingPosition);
                int attackerRow = attackerPosition / 8;
                int attackerCol = attackerPosition % 8;

                for (int direction = 0; direction < tempPossiblePins.Length; direction++)
                {
                    switch (direction)
                    {
                        case 0: // Down-left-up-right
                            tempPossiblePins[direction] = (squareDifference % 9 == 0) ? true : false;
                            break;
                        case 1: // Down-right-up-left
                            tempPossiblePins[direction] = (squareDifference % 7 == 0) ? true : false;
                            break;
                        case 2: // Vertical
                            tempPossiblePins[direction] = (pieceRow == attackerRow) ? true : false;
                            break;
                        case 3: // Horizontal
                            tempPossiblePins[direction] = (pieceCol == attackerCol) ? true : false;
                            break;
                    }
                }

                for (int j = 0; j < actualPossiblePins.Length; j++)
                {
                    if (tempPossiblePins[j])
                    {
                        actualPossiblePins[j] = true;
                    }
                }
            }

            if (actualPossiblePins[0] || actualPossiblePins[1]) // The only direction you can go in when pinned diagonally is towards the pinning piece
            {
                actualPossiblePins[2] = true;
                actualPossiblePins[3] = true;
            }

            if (position.Board[fromSquare].OccupiedBy.Type == PieceType.Pawn && // If pinned horizontally, pawns can't move diagonally
                actualPossiblePins[3])
            {
                actualPossiblePins[0] = true;
                actualPossiblePins[1] = true;
            }

            return actualPossiblePins;
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
                    return true; // Something went wrong, returns that a pin exists
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
