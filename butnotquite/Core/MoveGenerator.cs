namespace butnotquite.Core
{
    using Defaults;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class MoveGenerator
    {
        private static readonly HashSet<int> InitialWhitePawnSquares = new HashSet<int> { 48, 49, 50, 51, 52, 53, 54, 55 };
        private static readonly HashSet<int> InitialBlackPawnSquares = new HashSet<int> { 8, 9, 10, 11, 12, 13, 14, 15 };

        private static Chessboard position;
        private static bool GettingOpponentActivity; // Used when generating the opponent's moves for the current position

        #region Move Generation

        internal static List<Move> GetAvailableMoves(Chessboard chessboardPosition)
        {
            List<Move> availableMoves = new List<Move>(300);
            position = chessboardPosition;

            if (!GettingOpponentActivity)
            {
                GettingOpponentActivity = true;

                GetCurrentOpponentActivity();
            }

            for (int i = 0; i < 64; i++)
            {
                Square square = position.Board[i];

                if (square.OccupiedBy.Type == PieceType.None || square.OccupiedBy.Color == position.OppositeColor)
                {
                    continue;
                }

                switch (square.OccupiedBy.Type)
                {
                    case PieceType.None:
                        break;
                    case PieceType.Pawn:
                        availableMoves.AddRange(GetPawnMoves(i));
                        break;
                    case PieceType.Knight:
                        availableMoves.AddRange(GetKnightMoves(i));
                        break;
                    case PieceType.Bishop:
                        availableMoves.AddRange(GetBishopMoves(i));
                        break;
                    case PieceType.Rook:
                        availableMoves.AddRange(GetRookMoves(i));
                        break;
                    case PieceType.Queen:
                        availableMoves.AddRange(GetQueenMoves(i));
                        break;
                    case PieceType.King:
                        availableMoves.AddRange(GetKingMoves(i));
                        break;
                    default:
                        break;
                }
            }

            if (!GettingOpponentActivity)
            {
                RemoveIllegalMovesPinwise(availableMoves);
            }

            return availableMoves;
        }

        #region Common Movement Patterns

        private static List<Move> GetMovesUp(int fromSquare)
        {
            List<Move> movesUp = new List<Move>(11);
            int topBorder = 0;

            for (int location = fromSquare - 8; location >= topBorder; location -= 8)
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUp.Add(new Move(fromSquare, location, Direction.Vertical));

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesUp.Add(new Move(fromSquare, location, Direction.Vertical));

                    break;
                }
            }

            return movesUp;
        }

        private static List<Move> GetMovesDown(int fromSquare)
        {
            List<Move> movesDown = new List<Move>(11);
            int bottomBorder = 63;

            for (int location = (fromSquare + 8); location <= bottomBorder; location += 8)
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDown.Add(new Move(fromSquare, location, Direction.Vertical));

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesDown.Add(new Move(fromSquare, location, Direction.Vertical));

                    break;
                }
            }

            return movesDown;
        }

        private static List<Move> GetMovesLeft(int fromSquare)
        {
            List<Move> movesLeft = new List<Move>(11);
            int leftBorder = fromSquare - (fromSquare % 8);

            for (int location = (fromSquare - 1); location >= leftBorder; location--)
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesLeft.Add(new Move(fromSquare, location, Direction.Horizontal));

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesLeft.Add(new Move(fromSquare, location, Direction.Horizontal));

                    break;
                }
            }

            return movesLeft;
        }

        private static List<Move> GetMovesRight(int fromSquare)
        {
            List<Move> movesRight = new List<Move>(11);
            int rightBorder = fromSquare + (7 - fromSquare % 8);

            for (int location = (fromSquare + 1); location <= rightBorder; location++)
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesRight.Add(new Move(fromSquare, location, Direction.Horizontal));

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesRight.Add(new Move(fromSquare, location, Direction.Horizontal));

                    break;
                }
            }

            return movesRight;
        }

        private static List<Move> GetMovesUpRightDiagonal(int fromSquare)
        {
            HashSet<int> northEastBordersAdjusted = new HashSet<int>() { -7, -6, -5, -4, -3, -2, -1, 0, 8, 16, 24, 32, 40, 48, 57 };
            List<Move> movesUpRight = new List<Move>(11);

            int location = fromSquare - 7;

            while (!northEastBordersAdjusted.Contains(location))
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpRight.Add(new Move(fromSquare, location, Direction.DownLeftUpRight));

                    location -= 7;

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesUpRight.Add(new Move(fromSquare, location, Direction.DownLeftUpRight));

                    break;
                }

                location -= 7;
            }

            return movesUpRight;
        }

        private static List<Move> GetMovesDownLeftDiagonal(int fromSquare)
        {
            HashSet<int> southWestBordersAdjusted = new HashSet<int>() { 70, 69, 68, 67, 66, 65, 64, 63, 55, 47, 39, 31, 23, 15, 7 };
            List<Move> movesDownLeft = new List<Move>(11);
            int location = fromSquare + 7;

            while (!southWestBordersAdjusted.Contains(location))
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownLeft.Add(new Move(fromSquare, location, Direction.DownLeftUpRight));

                    location += 7;

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesDownLeft.Add(new Move(fromSquare, location, Direction.DownLeftUpRight));

                    break;
                }

                location += 7;
            }

            return movesDownLeft;
        }

        private static List<Move> GetMovesUpLeftDiagonal(int fromSquare)
        {
            HashSet<int> northWestBordersAdjusted = new HashSet<int>() { 47, 39, 31, 23, 15, 7, -1, -9, -8, -7, -6, -5, -4, -3, -2 };
            List<Move> movesUpLeft = new List<Move>(11);
            int location = fromSquare - 9;

            while (!northWestBordersAdjusted.Contains(location))
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpLeft.Add(new Move(fromSquare, location, Direction.DownRightUpLeft));

                    location -= 9;

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesUpLeft.Add(new Move(fromSquare, location, Direction.DownRightUpLeft));

                    break;
                }

                location -= 9;
            }

            return movesUpLeft;
        }

        private static List<Move> GetMovesDownRightDiagonal(int fromSquare)
        {
            HashSet<int> southEastBordersAdjusted = new HashSet<int>() { 16, 24, 32, 40, 48, 56, 64, 72, 71, 70, 69, 68, 67, 66, 65 };
            List<Move> movesDownRight = new List<Move>(11);
            int location = fromSquare + 9;

            while (!southEastBordersAdjusted.Contains(location))
            {
                Square square = position.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownRight.Add(new Move(fromSquare, location, Direction.DownRightUpLeft));

                    location += 9;

                    continue;
                }

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    movesDownRight.Add(new Move(fromSquare, location, Direction.DownRightUpLeft));

                    break;
                }

                location += 9;
            }

            return movesDownRight;
        }

        #endregion

        #region Pieces

        private static List<Move> GetKingMoves(int fromSquare)
        {
            List<Move> kingMoves = new List<Move>();

            if (!GettingOpponentActivity)
            {
                if (CanCastleShort())
                {
                    int kingFromSquare = fromSquare;
                    int kingDestination = (position.SideToMove == Color.White) ? 62 : 6;
                    int rookFromSquare = (position.SideToMove == Color.White) ? 63 : 7;
                    int rookDestination = (position.SideToMove == Color.White) ? 61 : 5;

                    kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
                }

                if (CanCastleLong())
                {
                    int kingFromSquare = fromSquare;
                    int kingDestination = (position.SideToMove == Color.White) ? 58 : 2;
                    int rookFromSquare = (position.SideToMove == Color.White) ? 56 : 0;
                    int rookDestination = (position.SideToMove == Color.White) ? 59 : 3;

                    kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
                }
            }            

            int currentKingPosition = GetKingPosition();
            int upLeft = currentKingPosition - 9;
            int up = currentKingPosition - 8;
            int upRight = currentKingPosition - 7;
            int right = currentKingPosition + 1;
            int downRight = currentKingPosition + 9;
            int down = currentKingPosition + 8;
            int downLeft = currentKingPosition + 7;
            int left = currentKingPosition - 1;
            int[] kingMovePositions = new int[8]
            {
                upLeft, up, upRight, right, downRight, down, downLeft, left
            };
            int currentKingCol = currentKingPosition % 8;

            for (int i = 0; i < kingMovePositions.Length; i++)
            {
                int toSquare = kingMovePositions[i];
                int toSquareCol = toSquare % 8;
                int fromToColDif = Math.Abs(currentKingCol - toSquareCol);

                if (toSquare < 0 || toSquare > 63 || fromToColDif > 1)
                {
                    continue;
                }

                Square square = position.Board[toSquare];

                if (square.OccupiedBy.Color == position.SideToMove)
                {
                    continue;
                }

                if (position.OpponentActivity.Any(kvp => kvp.Value.Contains(toSquare)))
                {
                    continue;
                }

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    kingMoves.Add(new Move(fromSquare, toSquare, Direction.SingleSquare));

                    continue;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    kingMoves.Add(new Move(fromSquare, toSquare, Direction.SingleSquare));
                }
            }

            return kingMoves;
        }

        private static List<Move> GetQueenMoves(int fromSquare)
        {
            List<Move> queenMoves = new List<Move>(35);

            queenMoves.AddRange(GetMovesUp(fromSquare));
            queenMoves.AddRange(GetMovesDown(fromSquare));
            queenMoves.AddRange(GetMovesLeft(fromSquare));
            queenMoves.AddRange(GetMovesRight(fromSquare));
            queenMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));

            return queenMoves;
        }

        private static List<Move> GetRookMoves(int fromSquare)
        {
            List<Move> rookMoves = new List<Move>(20);

            rookMoves.AddRange(GetMovesUp(fromSquare));
            rookMoves.AddRange(GetMovesDown(fromSquare));
            rookMoves.AddRange(GetMovesLeft(fromSquare));
            rookMoves.AddRange(GetMovesRight(fromSquare));

            return rookMoves;
        }

        private static List<Move> GetBishopMoves(int fromSquare)
        {
            List<Move> bishopMoves = new List<Move>(20);

            bishopMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));

            return bishopMoves;
        }

        private static List<Move> GetKnightMoves(int fromSquare)
        {
            List<Move> knightMoves = new List<Move>(12);
            // Long is three, short is one
            int difUpLeftShort = -10;
            int difUpLeftLong = -17;
            int difUpRightShort = -6;
            int difUpRightLong = -15;
            int difDownRightShort = 10;
            int difDownRightLong = 17;
            int difDownLeftShort = 6;
            int difDownLeftLong = 15;
            int[] jumpPositions = new int[8]
            {
                difUpLeftShort,
                difUpLeftLong,
                difUpRightShort,
                difUpRightLong,
                difDownRightShort,
                difDownRightLong,
                difDownLeftShort,
                difDownLeftLong
            };
            int fromCol = fromSquare % 8;

            for (int i = 0; i < jumpPositions.Length; i++)
            {
                int destination = fromSquare + jumpPositions[i];

                if (destination < 0 || destination > 63)
                {
                    continue;
                }

                int destinationCol = destination % 8;

                if (Math.Abs(destinationCol - fromCol) > 2) // Jumping over the board
                {
                    continue;
                }

                Square square = position.Board[destination];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    knightMoves.Add(new Move(fromSquare, destination, Direction.L));

                    continue;
                }

                if (square.OccupiedBy.Color == position.OppositeColor)
                {
                    knightMoves.Add(new Move(fromSquare, destination, Direction.L));
                }
            }

            return knightMoves;
        }

        private static List<Move> GetPawnMoves(int fromSquare)
        {
            List<Move> pawnMoves = new List<Move>(7);
            HashSet<int> initialPawnSquares = (position.SideToMove == Color.White) ? InitialWhitePawnSquares : InitialBlackPawnSquares;
            int direction = (position.SideToMove == Color.White) ? -1 : 1;
            int oneForward = fromSquare + (8 * direction);
            int twoForward = fromSquare + (16 * direction);
            int captureLeft = fromSquare + (9 * direction);
            int captureRight = fromSquare + (7 * direction);
            int currentCol = fromSquare % 8;

            if (position.Board[oneForward].OccupiedBy.Type == PieceType.None)
            {
                if (!GettingOpponentActivity) // When generating the opponent's moves, we don't need the check the squares in front of the pawns
                {
                    if (IsPawnPromotion(oneForward))
                    {
                        pawnMoves.Add(new Move(fromSquare, oneForward, Direction.Vertical, new Piece(position.SideToMove, PieceType.Queen, oneForward)));
                        pawnMoves.Add(new Move(fromSquare, oneForward, Direction.Vertical, new Piece(position.SideToMove, PieceType.Knight, oneForward)));
                    }
                    else
                    {
                        pawnMoves.Add(new Move(fromSquare, oneForward, Direction.Vertical));
                    }

                    if (initialPawnSquares.Contains(fromSquare)
                        && position.Board[twoForward].OccupiedBy.Type == PieceType.None)
                    {
                        pawnMoves.Add(new Move(fromSquare, twoForward, Direction.Vertical));
                    }
                }                
            }

            pawnMoves.AddRange(GenerateDiagonalPawnCaptures(fromSquare, captureLeft, Direction.DownRightUpLeft));
            pawnMoves.AddRange(GenerateDiagonalPawnCaptures(fromSquare, captureRight, Direction.DownLeftUpRight));

            int enPassantSquare = GetEnPassantSquare();

            if (enPassantSquare > -1)
            {
                pawnMoves.Add(new Move(fromSquare, enPassantSquare, Direction.EnPassant));
            }

            return pawnMoves;
        }
        
        #endregion

        #region Special Cases

        private static bool CanCastleShort()
        {
            if (position.SideToMove == Color.White)
            {
                if (!position.WhiteCanCastle || KingIsInCheck() || position.WhiteKingPosition != 60)
                {
                    return false;
                }

                int[] whiteShortCastleSquares = new int[2] { 61, 62 };

                for (int i = 0; i < whiteShortCastleSquares.Length; i++)
                {
                    if (position.Board[whiteShortCastleSquares[i]].OccupiedBy.Type != PieceType.None || 
                        position.OpponentActivity.Any(kvp => kvp.Value.Contains(whiteShortCastleSquares[i])))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (!position.BlackCanCastle || KingIsInCheck() || position.BlackKingPosition != 4)
            {
                return false;
            }

            int[] blackShortCastleSquares = new int[2] { 5, 6 };

            for (int i = 0; i < blackShortCastleSquares.Length; i++)
            {
                if (position.Board[blackShortCastleSquares[i]].OccupiedBy.Type != PieceType.None || 
                    position.OpponentActivity.Any(kvp => kvp.Value.Contains(blackShortCastleSquares[i])))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanCastleLong()
        {
            if (position.SideToMove == Color.White)
            {
                if (!position.WhiteCanCastle || KingIsInCheck() || position.WhiteKingPosition != 60)
                {
                    return false;
                }

                int[] whiteLongCastleSquares = new int[2] { 58, 59 };

                if (position.Board[57].OccupiedBy.Type != PieceType.None)
                {
                    return false;
                }

                for (int i = 0; i < whiteLongCastleSquares.Length; i++)
                {
                    if (position.Board[whiteLongCastleSquares[i]].OccupiedBy.Type != PieceType.None || 
                        position.OpponentActivity.Any(kvp => kvp.Value.Contains(whiteLongCastleSquares[i])))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (!position.BlackCanCastle || KingIsInCheck() || position.BlackKingPosition != 4)
            {
                return false;
            }

            int[] blackLongCastleSquares = new int[2] { 2, 3 };

            if (position.Board[1].OccupiedBy.Type != PieceType.None)
            {
                return false;
            }

            for (int i = 0; i < blackLongCastleSquares.Length; i++)
            {
                if (position.Board[blackLongCastleSquares[i]].OccupiedBy.Type != PieceType.None ||
                    position.OpponentActivity.Any(kvp => kvp.Value.Contains(blackLongCastleSquares[i])))
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetEnPassantSquare()
        {
            int fromSquare = position.LastMove.FromSquare;
            int toSquare = position.LastMove.ToSquare;
            Piece lastMovedPiece = position.Board[toSquare].OccupiedBy;
            HashSet<int> initialPawnSquares = (position.SideToMove == Color.White) ? InitialBlackPawnSquares : InitialWhitePawnSquares;

            if (lastMovedPiece.Type == PieceType.Pawn)
            {
                if (initialPawnSquares.Contains(fromSquare))
                {
                    int moveDirection = (lastMovedPiece.Color == Color.White) ? -1 : 1;
                    int twoForward = fromSquare + (16 * moveDirection);

                    if (position.LastMove.ToSquare == twoForward)
                    {
                        int currentCol = toSquare % 8;
                        int twoForwardOneLeft = twoForward - 1;
                        int twoForwardOneRight = twoForward + 1;
                        int twoForwardOneLeftCol = twoForwardOneLeft % 8;
                        int twoForwardOneRightCol = twoForwardOneRight % 8;
                        int leftDif = Math.Abs(twoForwardOneLeftCol - currentCol);
                        int rightDif = Math.Abs(twoForwardOneRightCol - currentCol);
                        bool enPassantPossible = false;

                        if (leftDif == 1) // A flank pawn hasn't been pushed
                        {
                            if (position.Board[twoForwardOneLeft].OccupiedBy.Type == PieceType.Pawn)
                            {
                                enPassantPossible = true;
                            }
                        }

                        if (rightDif == 1)
                        {
                            if (position.Board[twoForwardOneRight].OccupiedBy.Type == PieceType.Pawn)
                            {
                                enPassantPossible = true;
                            }
                        }

                        if (enPassantPossible)
                        {
                            int enPassantSquare = fromSquare + (8 * moveDirection);

                            return enPassantSquare;
                        }
                    }
                }
            }

            return -1;
        }

        private static bool IsPawnPromotion(int toSquare)
        {
            return (toSquare < 8 || toSquare > 55);
        }

        #endregion

        #region Utils

        private static bool KingIsInCheck()
        {
            return (position.SideToMove == Color.White) ?
                position.OpponentActivity.Any(kvp => kvp.Value.Contains(position.WhiteKingPosition)) :
                position.OpponentActivity.Any(kvp => kvp.Value.Contains(position.BlackKingPosition));
        }

        private static int GetKingPosition()
        {
            return (position.SideToMove == Color.White) ?
                position.WhiteKingPosition :
                position.BlackKingPosition;
        }

        private static List<Move> GenerateDiagonalPawnCaptures(int fromSquare, int diagonalCaptureIndex, Direction direction)
        {
            List<Move> pawnMoves = new List<Move>();
            int currentCol = fromSquare % 8;
            int captureCol = diagonalCaptureIndex % 8;
            int captureColDif = Math.Abs(captureCol - currentCol);

            if ((diagonalCaptureIndex > -1 && diagonalCaptureIndex < 64) && captureColDif == 1)  // Borders are in order
            {
                if (!GettingOpponentActivity)
                {
                    if (position.Board[diagonalCaptureIndex].OccupiedBy.Color == position.OppositeColor) // Generating our moves, so we should check if the square is occupied by an enemy piece
                    {
                        if (IsPawnPromotion(diagonalCaptureIndex))
                        {
                            pawnMoves.Add(new Move(fromSquare, diagonalCaptureIndex, direction, new Piece(position.SideToMove, PieceType.Queen, diagonalCaptureIndex)));
                            pawnMoves.Add(new Move(fromSquare, diagonalCaptureIndex, direction, new Piece(position.SideToMove, PieceType.Knight, diagonalCaptureIndex)));
                        }
                        else
                        {
                            pawnMoves.Add(new Move(fromSquare, diagonalCaptureIndex, direction));
                        }
                    }
                }
                else // Just get the square that this pawn controls
                {
                    pawnMoves.Add(new Move(fromSquare, diagonalCaptureIndex, Direction.DownRightUpLeft));
                }
            }

            return pawnMoves;
        }

        private static void GetCurrentOpponentActivity()
        {
            position.OpponentActivity.Clear();

            // Reverse the colors
            position.SideToMove = (position.SideToMove == Color.White) ? Color.Black : Color.White;
            position.OppositeColor = (position.SideToMove == Color.White) ? Color.Black : Color.White;

            List<Move> availableMoves = GetAvailableMoves(position);
            GettingOpponentActivity = false;

            for (int i = 0; i < availableMoves.Count; i++)
            {
                int fromSquare = availableMoves[i].FromSquare;

                if (fromSquare == -1) // Castling will not be included
                {
                    continue;
                }

                Square currentSquare = position.Board[fromSquare];

                if (!position.OpponentActivity.ContainsKey(currentSquare.OccupiedBy))
                {
                    position.OpponentActivity[currentSquare.OccupiedBy] = new HashSet<int>();
                }

                position.OpponentActivity[currentSquare.OccupiedBy].Add(availableMoves[i].ToSquare);
            }

            // Undo color reversion
            position.SideToMove = (position.SideToMove == Color.White) ? Color.Black : Color.White;
            position.OppositeColor = (position.SideToMove == Color.White) ? Color.Black : Color.White;
        }

        private static void RemoveIllegalMovesPinwise(List<Move> availableMoves)
        {
            for (int i = 0; i < availableMoves.Count; i++)
            {
                Move move = availableMoves[i];
                HashSet<Direction> illegalDirections = GetIllegalDirections(move.FromSquare);

                availableMoves.RemoveAll(m => m.FromSquare == move.FromSquare && illegalDirections.Contains(move.Direction));
            }
        }

        private static HashSet<Direction> GetIllegalDirections(int fromSquare)
        {
            HashSet<Direction> pinDirections = new HashSet<Direction>();

            if (!position.OpponentActivity.Any(kvp =>
            (
                kvp.Key.Type == PieceType.Queen ||
                kvp.Key.Type == PieceType.Rook ||
                kvp.Key.Type == PieceType.Bishop)
                && kvp.Value.Contains(fromSquare)))
            {
                return pinDirections;
            }

            int kingPosition = GetKingPosition();
            int kingRow = kingPosition / 8;
            int kingCol = kingPosition % 8;
            int pieceRow = fromSquare / 8;
            int pieceCol = fromSquare % 8;
            bool verticalPinPosible = (kingRow == pieceRow);
            bool horizontalPinPossible = (kingCol == pieceCol);
            bool downLeftUpRightPinPossible = (Math.Abs(fromSquare - kingPosition) % 7 == 0);
            bool downRightUpLeft = (Math.Abs(fromSquare - kingPosition) % 9 == 0);
            bool[] possiblePins = new bool[4] { verticalPinPosible, horizontalPinPossible, downLeftUpRightPinPossible, downRightUpLeft };

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
                        case 0: // Vertical, same rank, so piece can't move up or down
                            if (PinExists(Direction.Vertical, fromSquare, kingPosition))
                            {
                                pinDirections.Add(Direction.Vertical);
                                pinDirections.Add(Direction.L); // If the knight is pinned, it simply can't move
                            }
                            break;
                        case 1: // Horizontal, same file, so piece can't move left or right
                            if (PinExists(Direction.Horizontal, fromSquare, kingPosition))
                            {
                                pinDirections.Add(Direction.Horizontal);
                                pinDirections.Add(Direction.L);
                            }
                            break;
                        case 2: // First diagonal
                            if (PinExists(Direction.DownLeftUpRight, fromSquare, kingPosition))
                            {
                                pinDirections.Add(Direction.DownLeftUpRight);
                                pinDirections.Add(Direction.L);
                                pinDirections.Add(Direction.EnPassant);
                            }
                            break;
                        case 3: // Second diagonal
                            if (PinExists(Direction.DownRightUpLeft, fromSquare, kingPosition))
                            {
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

        private static bool PinExists(Direction potentialIllegalDirection, int fromSquare, int kingPosition)
        {
            int difference = Math.Abs(fromSquare - kingPosition);
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
                    increment = (difference % 7 == 0) ? 7 : 9;
                    break;
                default:
                    return true; // Something went wrong, returns that a pin exists
            }

            for (int i = minSquare; i < maxSquare; i += increment)
            {
                if (position.Board[i].OccupiedBy.Type != PieceType.None) // There is a piece between the current piece and the king; no pin
                {
                    return false;
                }
            }

            return true;
        }

        private static void ResetSquare(int squareNumber)
        {
            Square square = position.Board[squareNumber];

            square.OccupiedBy.Type = PieceType.None;
            square.OccupiedBy.Color = Color.None;
            square.OccupiedBy.Value = 0;
            square.OccupiedBy.Position = -1;
            square.OccupiedBy.Moves = null;
        }

        #endregion

        #endregion
    }
}