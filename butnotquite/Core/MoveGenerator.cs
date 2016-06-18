namespace butnotquite.Core
{
    using Defaults;
    using Models;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal static class MoveGenerator
    {
        private static readonly HashSet<int> InitialWhitePawnSquares = new HashSet<int> { 48, 49, 50, 51, 52, 53, 54, 55 };
        private static readonly HashSet<int> InitialBlackPawnSquares = new HashSet<int> { 8, 9, 10, 11, 12, 13, 14, 15 };

        private static Square[] board;
        private static Color sideToMove;
        private static Color oppositeColor;
        private static int whiteKingPosition;
        private static int blackKingPosition;
        private static bool whiteCanCastle;
        private static bool blackCanCastle;
        private static IDictionary<Piece, HashSet<int>> opponentControl;
        private static Move lastMove;

        #region Move Generation

        internal static List<Move> GetAvailableMoves(Chessboard chessboardPosition)
        {
            board = chessboardPosition.Board;
            sideToMove = chessboardPosition.SideToMove;
            oppositeColor = chessboardPosition.OppositeColor;
            whiteKingPosition = chessboardPosition.WhiteKingPosition;
            blackKingPosition = chessboardPosition.BlackKingPosition;
            whiteCanCastle = chessboardPosition.WhiteCanCastle;
            blackCanCastle = chessboardPosition.BlackCanCastle;
            opponentControl = chessboardPosition.OpponentControl;
            lastMove = chessboardPosition.LastMove;

            List<Move> availableMoves = new List<Move>(300);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < 64; i++)
            {
                Square square = board[i];

                if (square.OccupiedBy.Type == PieceType.None || square.OccupiedBy.Color == oppositeColor)
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

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            GetOpponentMoves(availableMoves);

            return availableMoves;
        }

        #region Common Movement Patterns

        private static List<Move> GetMovesUp(int fromSquare)
        {
            List<Move> movesUp = new List<Move>(11);
            int topBorder = 0;

            for (int location = fromSquare - 8; location >= topBorder; location -= 8)
            {
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUp.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesUp.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDown.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesDown.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesLeft.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesLeft.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesRight.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesRight.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

                    location -= 7;

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownLeft.Add(new Move(fromSquare, location));

                    location += 7;

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesDownLeft.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpLeft.Add(new Move(fromSquare, location));

                    location -= 9;

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesUpLeft.Add(new Move(fromSquare, location));

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
                Square square = board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownRight.Add(new Move(fromSquare, location));

                    location += 9;

                    continue;
                }

                if (square.OccupiedBy.Color == sideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    movesDownRight.Add(new Move(fromSquare, location));

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

            if (CanCastleShort())
            {
                int kingFromSquare = fromSquare;
                int kingDestination = (sideToMove == Color.White) ? 62 : 6;
                int rookFromSquare = (fromSquare == 60) ? 63 : 7;
                int rookDestination = (kingDestination == 62) ? 61 : 5;

                kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
            }

            if (CanCastleLong())
            {
                int kingFromSquare = fromSquare;
                int kingDestination = (sideToMove == Color.White) ? 62 : 6;
                int rookFromSquare = (fromSquare == 60) ? 63 : 7;
                int rookDestination = (kingDestination == 62) ? 61 : 5;

                kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
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

            for (int i = 0; i < kingMovePositions.Length; i++)
            {
                int squareValue = kingMovePositions[i];

                if (squareValue < 0 || squareValue > 63)
                {
                    continue;
                }

                Square square = board[squareValue];

                if (square.OccupiedBy.Color == sideToMove)
                {
                    continue;
                }

                if (opponentControl.Any(kvp => kvp.Value.Contains(squareValue)))
                {
                    continue;
                }

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    kingMoves.Add(new Move(fromSquare, squareValue));

                    continue;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    kingMoves.Add(new Move(fromSquare, squareValue));
                }
            }

            return kingMoves;
        }

        private static List<Move> GetQueenMoves(int fromSquare)
        {
            List<Move> queenMoves = new List<Move>(35);
            HashSet<string> illegalDirections = GetIllegalDirections(fromSquare);

            if (!illegalDirections.Contains("vertical"))
            {
                queenMoves.AddRange(GetMovesUp(fromSquare));
                queenMoves.AddRange(GetMovesDown(fromSquare));
            }

            if (!illegalDirections.Contains("horizontal"))
            {
                queenMoves.AddRange(GetMovesLeft(fromSquare));
                queenMoves.AddRange(GetMovesRight(fromSquare));
            }

            if (!illegalDirections.Contains("down-left-up-right"))
            {
                queenMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
                queenMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            }

            if (!illegalDirections.Contains("down-right-up-left"))
            {
                queenMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));
                queenMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));
            }

            return queenMoves;
        }

        private static List<Move> GetRookMoves(int fromSquare)
        {
            List<Move> rookMoves = new List<Move>(20);
            HashSet<string> illegalDirections = GetIllegalDirections(fromSquare);

            if (!illegalDirections.Contains("vertical"))
            {
                rookMoves.AddRange(GetMovesUp(fromSquare));
                rookMoves.AddRange(GetMovesDown(fromSquare));
            }

            if (!illegalDirections.Contains("horizontal"))
            {
                rookMoves.AddRange(GetMovesLeft(fromSquare));
                rookMoves.AddRange(GetMovesRight(fromSquare));
            }

            return rookMoves;
        }

        private static List<Move> GetBishopMoves(int fromSquare)
        {
            List<Move> bishopMoves = new List<Move>(20);
            HashSet<string> illegalDirections = GetIllegalDirections(fromSquare);

            if (!illegalDirections.Contains("down-left-up-right"))
            {
                bishopMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
                bishopMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            }

            if (!illegalDirections.Contains("down-right-up-left"))
            {
                bishopMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));
                bishopMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));
            }

            return bishopMoves;
        }

        private static List<Move> GetKnightMoves(int fromSquare)
        {
            List<Move> knightMoves = new List<Move>(12);

            if (GetIllegalDirections(fromSquare).Any())
            {
                return knightMoves;
            }

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

                Square square = board[destination];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    knightMoves.Add(new Move(fromSquare, destination));

                    continue;
                }

                if (square.OccupiedBy.Color == oppositeColor)
                {
                    knightMoves.Add(new Move(fromSquare, destination));
                }
            }

            return knightMoves;
        }

        private static List<Move> GetPawnMoves(int fromSquare)
        {
            List<Move> pawnMoves = new List<Move>(7);
            HashSet<string> illegalDirections = GetIllegalDirections(fromSquare);
            HashSet<int> initialPawnSquares = (sideToMove == Color.White) ? InitialWhitePawnSquares : InitialBlackPawnSquares;
            int direction = (sideToMove == Color.White) ? -1 : 1;
            int oneForward = fromSquare + (8 * direction);
            int twoForward = fromSquare + (16 * direction);
            int captureLeft = fromSquare + (9 * direction);
            int captureRight = fromSquare + (7 * direction);
            int currentCol = fromSquare % 8;

            if (board[oneForward].OccupiedBy.Type == PieceType.None 
                && !illegalDirections.Contains("vertical"))
            {
                if (IsPawnPromotion(oneForward))
                {
                    pawnMoves.Add(new Move(fromSquare, oneForward, new Piece(sideToMove, PieceType.Queen, oneForward)));
                    pawnMoves.Add(new Move(fromSquare, oneForward, new Piece(sideToMove, PieceType.Knight, oneForward)));
                }
                else
                {
                    pawnMoves.Add(new Move(fromSquare, oneForward));
                }

                if (initialPawnSquares.Contains(fromSquare)
                    && board[twoForward].OccupiedBy.Type == PieceType.None)
                {
                    pawnMoves.Add(new Move(fromSquare, twoForward));
                }
            }

            int destLeftCol = captureLeft % 8;
            int captureLeftDif = Math.Abs(destLeftCol - currentCol);

            if ((captureLeft > -1 && captureLeft < 64)
                && captureLeftDif == 1
                && board[captureLeft].OccupiedBy.Color == oppositeColor
                && !illegalDirections.Contains("down-left-up-right"))
            {
                if (IsPawnPromotion(captureLeft))
                {
                    pawnMoves.Add(new Move(fromSquare, captureLeft, new Piece(sideToMove, PieceType.Queen, captureLeft)));
                    pawnMoves.Add(new Move(fromSquare, captureLeft, new Piece(sideToMove, PieceType.Knight, captureLeft)));
                }
                else
                {
                    pawnMoves.Add(new Move(fromSquare, captureLeft));
                }
            }

            int destRightCol = captureRight % 8;
            int captureRightDif = Math.Abs(destRightCol - currentCol);

            if ((captureRight > -1 && captureRight < 64)
                && captureRightDif == 1
                && board[captureRight].OccupiedBy.Color == oppositeColor
                && !illegalDirections.Contains("down-right-up-left"))
            {
                if (IsPawnPromotion(captureRight))
                {
                    pawnMoves.Add(new Move(fromSquare, captureRight, new Piece(sideToMove, PieceType.Queen, captureRight)));
                    pawnMoves.Add(new Move(fromSquare, captureRight, new Piece(sideToMove, PieceType.Knight, captureRight)));
                }
                else
                {
                    pawnMoves.Add(new Move(fromSquare, captureRight));
                }
            }

            if (!(illegalDirections.Contains("down-right-up-left") 
                && illegalDirections.Contains("down-left-up-right")))
            {
                int enPassantSquare = GetEnPassantSquare();

                if (enPassantSquare > -1)
                {
                    pawnMoves.Add(new Move(fromSquare, enPassantSquare));
                }
            }

            return pawnMoves;
        }

        #endregion

        #region Special Cases

        private static bool CanCastleShort()
        {
            if (sideToMove == Color.White)
            {
                if (whiteCanCastle || KingIsInCheck() || whiteKingPosition != 60)
                {
                    return false;
                }

                int[] whiteShortCastleSquares = new int[2] { 61, 62 };

                for (int i = 0; i < whiteShortCastleSquares.Length; i++)
                {
                    if (board[whiteShortCastleSquares[i]].OccupiedBy.Type != PieceType.None
                        || opponentControl.Any(kvp => kvp.Value.Contains(whiteShortCastleSquares[i])))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (blackCanCastle || KingIsInCheck() || blackKingPosition != 4)
            {
                return false;
            }

            int[] blackShortCastleSquares = new int[2] { 5, 6 };

            for (int i = 0; i < blackShortCastleSquares.Length; i++)
            {
                if (board[blackShortCastleSquares[i]].OccupiedBy.Type != PieceType.None
                    || opponentControl.Any(kvp => kvp.Value.Contains(blackShortCastleSquares[i])))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanCastleLong()
        {
            if (sideToMove == Color.White)
            {
                if (whiteCanCastle || KingIsInCheck() || whiteKingPosition != 60)
                {
                    return false;
                }

                List<int> whiteLongCastleSquares = new List<int>(3) { 57, 58, 59 };

                for (int i = 0; i < whiteLongCastleSquares.Count; i++)
                {
                    if (board[whiteLongCastleSquares[i]].OccupiedBy.Type != PieceType.None)
                    {
                        return false;
                    }

                    whiteLongCastleSquares.Remove(57); // We can still castle if this square is being controlled

                    if (opponentControl.Any(kvp => kvp.Value.Contains(whiteLongCastleSquares[i])))
                    {
                        return false;
                    }

                    whiteLongCastleSquares.Add(57);
                }

                return true;
            }

            if (blackCanCastle || KingIsInCheck() || blackKingPosition != 4)
            {
                return false;
            }

            List<int> blackLongCastleSquares = new List<int>(3) { 1, 2, 3 };

            for (int i = 0; i < blackLongCastleSquares.Count; i++)
            {
                if (board[blackLongCastleSquares[i]].OccupiedBy.Type != PieceType.None)
                {
                    return false;
                }

                blackLongCastleSquares.Remove(1);

                if (opponentControl.Any(kvp => kvp.Value.Contains(blackLongCastleSquares[i])))
                {
                    return false;
                }

                blackLongCastleSquares.Add(1);
            }

            return true;
        }

        private static int GetEnPassantSquare()
        {
            int fromSquare = lastMove.FromSquare;
            int toSquare = lastMove.ToSquare;
            Piece lastMovedPiece = board[toSquare].OccupiedBy;
            HashSet<int> initialPawnSquares = (sideToMove == Color.White) ? InitialBlackPawnSquares : InitialWhitePawnSquares;

            if (lastMovedPiece.Type == PieceType.Pawn)
            {
                if (initialPawnSquares.Contains(fromSquare))
                {
                    int moveDirection = (lastMovedPiece.Color == Color.White) ? -1 : 1;
                    int twoForward = fromSquare + (16 * moveDirection);

                    if (lastMove.ToSquare == twoForward)
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
                            if (board[twoForwardOneLeft].OccupiedBy.Type == PieceType.Pawn)
                            {
                                enPassantPossible = true;
                            }
                        }

                        if (rightDif == 1)
                        {
                            if (board[twoForwardOneRight].OccupiedBy.Type == PieceType.Pawn)
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
            return (sideToMove == Color.White) ?
                opponentControl.Any(kvp => kvp.Value.Contains(whiteKingPosition)) :
                opponentControl.Any(kvp => kvp.Value.Contains(blackKingPosition));
        }

        private static int GetKingPosition()
        {
            return (sideToMove == Color.White) ?
                whiteKingPosition :
                blackKingPosition;
        }

        private static void GetOpponentMoves(IList<Move> availableMoves)
        {
            opponentControl.Clear();

            for (int i = 0; i < availableMoves.Count; i++)
            {
                Square currentSquare = board[availableMoves[i].FromSquare];

                if (!opponentControl.ContainsKey(currentSquare.OccupiedBy))
                {
                    opponentControl[currentSquare.OccupiedBy] = new HashSet<int>();
                }

                opponentControl[currentSquare.OccupiedBy].Add(availableMoves[i].ToSquare);
            }
        }

        private static HashSet<string> GetIllegalDirections(int fromSquare)
        {
            HashSet<string> pinDirections = new HashSet<string>();

            if (!opponentControl.Any(kvp =>
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
            bool upLeftDownRight = (Math.Abs(fromSquare - kingPosition) % 9 == 0);
            bool[] possiblePins = new bool[4] { verticalPinPosible, horizontalPinPossible, downLeftUpRightPinPossible, upLeftDownRight };

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
                            if (PinExists("vertical", fromSquare, kingPosition))
                            {
                                pinDirections.Add("vertical");
                            }
                            break;
                        case 1: // Horizontal, same file, so piece can't move left or right
                            if (PinExists("horizontal", fromSquare, kingPosition))
                            {
                                pinDirections.Add("horizontal");
                            }
                            break;
                        case 2: // First diagonal
                            if (PinExists("down-left-up-right", fromSquare, kingPosition))
                            {
                                pinDirections.Add("down-left-up-right");
                            }
                            break;
                        case 3: // Second diagonal
                            if (PinExists("down-right-up-left", fromSquare, kingPosition))
                            {
                                pinDirections.Add("down-right-up-left");
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return pinDirections;
        }

        private static bool PinExists(string potentialIllegalDirection, int fromSquare, int kingPosition)
        {
            int difference = Math.Abs(fromSquare - kingPosition);
            int minSquare = Math.Min(fromSquare, kingPosition);
            int maxSquare = Math.Max(fromSquare, kingPosition);
            int increment = 0;

            switch (potentialIllegalDirection)
            {
                case "vertical":
                    increment = 1;
                    break;
                case "horizontal":
                    increment = 8;
                    break;
                case "down-left-up-right":
                case "down-right-up-left":
                    increment = (difference % 7 == 0) ? 7 : 9;
                    break;
                default:
                    return true; // Something went wrong, returns that a pin exists
            }

            for (int i = minSquare; i < maxSquare; i += increment)
            {
                if (board[i].OccupiedBy.Type != PieceType.None) // There is a piece between the current piece and the king; no pin
                {
                    return false;
                }
            }

            return true;
        }

        private static void ResetSquare(int squareNumber)
        {
            Square square = board[squareNumber];

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
