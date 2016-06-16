namespace butnotquite.Models
{
    using Defaults;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    internal sealed class Chessboard
    {
        private readonly HashSet<int> InitialPawnSquares = new HashSet<int> { 8, 9, 10, 11, 12, 13, 14, 15, 48, 49, 50, 51, 52, 53, 54, 55 };

        internal Square[] Board;
        internal Color SideToMove;
        internal Color OppositeColor;
        internal int Evaluation;

        internal HashSet<int> OpponentControl;

        internal int WhiteKingPosition;
        internal int BlackKingPosition;
        internal bool WhiteCastled;
        internal bool BlackCastled;

        internal int RepetitionCounter;
        internal bool RepetitionEnforcable;        
        internal int FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;
        internal bool Stalemate;

        private Move lastMove;

        internal Chessboard()
        {
            this.Board = new Square[64];

            for (int i = 0; i < Board.Length; i++)
            {
                this.Board[i] = new Square();
            }

            this.SideToMove = Color.White;
            this.OppositeColor = Color.Black;

            this.WhiteKingPosition = 60;
            this.BlackKingPosition = 4;

            this.OpponentControl = new HashSet<int>();
        }

        #region Board Initialization

        internal void InitializeStartingPosition()
        {
            this.SetBlackPieces();
            this.FillEmptySquares();
            this.SetWhitePieces();
        }

        private void SetBlackPieces()
        {
            int blackTerritoryLimit = 15;

            for (int i = 0; i <= blackTerritoryLimit; i++)
            {
                if (i >= 8)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.Pawn);

                    continue;
                }

                if (i == 0 || i == 7)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.Rook);

                    continue;
                }

                if (i == 1 || i == 6)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.Knight);

                    continue;
                }

                if (i == 2 || i == 5)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.Bishop);

                    continue;
                }

                if (i == 3)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.Queen);

                    continue;
                }

                if (i == 4)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.Black, PieceType.King);
                }
            }
        }

        private void FillEmptySquares()
        {
            int blackTerritoryEnd = 16;
            int emptyTerritoryLimit = 47;

            for (int i = blackTerritoryEnd; i <= emptyTerritoryLimit; i++)
            {
                this.Board[i].OccupiedBy = new Piece();
            }
        }

        private void SetWhitePieces()
        {
            int emptyTerritoryEnd = 48;

            for (int i = emptyTerritoryEnd; i <= 63; i++)
            {
                if (i <= 55)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Pawn);

                    continue;
                }

                if (i == 56 || i == 63)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Rook);

                    continue;
                }

                if (i == 57 || i == 62)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Knight);

                    continue;
                }

                if (i == 58 || i == 61)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Bishop);

                    continue;
                }

                if (i == 59)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Queen);

                    continue;
                }

                if (i == 60)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.King);
                }
            }
        }

        #endregion

        #region Move Generation

        internal void MakeMove(int fromSquare, int toSquare)
        {
            Piece movingPiece = MakeDeepCopy(this.Board[fromSquare].OccupiedBy);

            this.Board[fromSquare].OccupiedBy.Type = PieceType.None;
            this.Board[toSquare].OccupiedBy = movingPiece;

            this.lastMove = new Move(fromSquare, toSquare);

            this.SideToMove = (this.SideToMove == Color.White) ? Color.Black : Color.White;
            this.OppositeColor = (this.SideToMove == Color.White) ? Color.Black : Color.White;
        }

        internal IEnumerable<Move> GetAvailableMoves()
        {
            List<Move> availableMoves = new List<Move>(217);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < 64; i++)
            {
                Square square = this.Board[i];

                if (square.OccupiedBy.Type == PieceType.None || square.OccupiedBy.Color == this.OppositeColor)
                {
                    continue;
                }

                if (this.KingIsInCheck())
                {
                    // TODO

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

            this.OpponentControl.Clear();

            for (int i = 0; i < availableMoves.Count; i++)
            {
                this.OpponentControl.Add(availableMoves[i].ToSquare);
            }

            return availableMoves;
        }

        #region Common Movement Patterns

        private IEnumerable<Move> GetMovesUp(int fromSquare)
        {
            List<Move> movesUp = new List<Move>(7);
            int topBorder = 0;

            for (int location = fromSquare - 8; location >= topBorder; location -= 8)
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUp.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesUp.Add(new Move(fromSquare, location));

                    break;
                }
            }

            return movesUp;
        }

        private IEnumerable<Move> GetMovesDown(int fromSquare)
        {
            List<Move> movesDown = new List<Move>(7);
            int bottomBorder = 63;

            for (int location = (fromSquare + 8); location <= bottomBorder; location += 8)
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDown.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesDown.Add(new Move(fromSquare, location));

                    break;
                }
            }

            return movesDown;
        }

        private IEnumerable<Move> GetMovesLeft(int fromSquare)
        {
            List<Move> movesLeft = new List<Move>(7);
            int leftBorder = fromSquare - (fromSquare % 8);

            for (int location = (fromSquare - 1); location >= leftBorder; location--)
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesLeft.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesLeft.Add(new Move(fromSquare, location));

                    break;
                }
            }

            return movesLeft;
        }

        private IEnumerable<Move> GetMovesRight(int fromSquare)
        {
            List<Move> movesRight = new List<Move>(7);
            int rightBorder = fromSquare + (7 - fromSquare % 8);

            for (int location = (fromSquare + 1); location <= rightBorder; location++)
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesRight.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesRight.Add(new Move(fromSquare, location));

                    break;
                }
            }

            return movesRight;
        }

        private IEnumerable<Move> GetMovesUpRightDiagonal(int fromSquare)
        {
            HashSet<int> northEastBordersAdjusted = new HashSet<int>() { -7, -6, -5, -4, -3, -2, -1, 0, 8, 16, 24, 32, 40, 48, 57 };
            List<Move> movesUpRight = new List<Move>(7);

            int location = fromSquare - 7;

            while (!northEastBordersAdjusted.Contains(location))
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

                    location -= 7;

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

                    break;
                }

                location -= 7;
            }

            return movesUpRight;
        }

        private IEnumerable<Move> GetMovesDownLeftDiagonal(int fromSquare)
        {
            HashSet<int> southWestBordersAdjusted = new HashSet<int>() { 70, 69, 68, 67, 66, 65, 64, 63, 55, 47, 39, 31, 23, 15, 7 };
            List<Move> movesDownLeft = new List<Move>(7);
            int location = fromSquare + 7;

            while (!southWestBordersAdjusted.Contains(location))
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownLeft.Add(new Move(fromSquare, location));

                    location += 7;

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesDownLeft.Add(new Move(fromSquare, location));

                    break;
                }

                location += 7;
            }

            return movesDownLeft;
        }

        private IEnumerable<Move> GetMovesUpLeftDiagonal(int fromSquare)
        {
            HashSet<int> northWestBordersAdjusted = new HashSet<int>() { 47, 39, 31, 23, 15, 7, -1, -9, -8, -7, -6, -5, -4, -3, -2 };
            List<Move> movesUpLeft = new List<Move>(7);
            int location = fromSquare - 9;

            while (!northWestBordersAdjusted.Contains(location))
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpLeft.Add(new Move(fromSquare, location));

                    location -= 9;

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    movesUpLeft.Add(new Move(fromSquare, location));

                    break;
                }

                location -= 9;
            }

            return movesUpLeft;
        }

        private IEnumerable<Move> GetMovesDownRightDiagonal(int fromSquare)
        {
            HashSet<int> southEastBordersAdjusted = new HashSet<int>() { 16, 24, 32, 40, 48, 56, 64, 72, 71, 70, 69, 68, 67, 66, 65 };
            List<Move> movesDownRight = new List<Move>(7);
            int location = fromSquare + 9;

            while (!southEastBordersAdjusted.Contains(location))
            {
                Square square = this.Board[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownRight.Add(new Move(fromSquare, location));

                    location += 9;

                    continue;
                }

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    break;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
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

        private IEnumerable<Move> GetKingMoves(int fromSquare)
        {
            List<Move> kingMoves = new List<Move>();            

            if (this.CanCastleShort())
            {
                int kingFromSquare = fromSquare;
                int kingDestination = (this.SideToMove == Color.White) ? 62 : 6;
                int rookFromSquare = (fromSquare == 60) ? 63 : 7;
                int rookDestination = (kingDestination == 62) ? 61 : 5;

                kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
            }

            if (this.CanCastleLong())
            {
                int kingFromSquare = fromSquare;
                int kingDestination = (this.SideToMove == Color.White) ? 62 : 6;
                int rookFromSquare = (fromSquare == 60) ? 63 : 7;
                int rookDestination = (kingDestination == 62) ? 61 : 5;

                kingMoves.Add(new Move(kingFromSquare, kingDestination, rookFromSquare, rookDestination));
            }

            int currentKingPosition = this.GetKingPosition();
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

                Square square = this.Board[squareValue];

                if (square.OccupiedBy.Color == this.SideToMove)
                {
                    continue;
                }

                if (this.OpponentControl.Contains(squareValue))
                {
                    continue;
                }

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    kingMoves.Add(new Move(fromSquare, squareValue));

                    continue;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    kingMoves.Add(new Move(fromSquare, squareValue));
                }
            }

            return kingMoves;
        }

        private IEnumerable<Move> GetQueenMoves(int fromSquare)
        {
            List<Move> queenMoves = new List<Move>(24);

            queenMoves.AddRange(GetMovesUp(fromSquare));
            queenMoves.AddRange(GetMovesDown(fromSquare));
            queenMoves.AddRange(GetMovesLeft(fromSquare));
            queenMoves.AddRange(GetMovesRight(fromSquare));
            queenMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));
            queenMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));

            return queenMoves;
        }

        private IEnumerable<Move> GetRookMoves(int fromSquare)
        {
            List<Move> rookMoves = new List<Move>(14);

            rookMoves.AddRange(GetMovesUp(fromSquare));
            rookMoves.AddRange(GetMovesDown(fromSquare));
            rookMoves.AddRange(GetMovesLeft(fromSquare));
            rookMoves.AddRange(GetMovesRight(fromSquare));

            return rookMoves;
        }

        private IEnumerable<Move> GetBishopMoves(int fromSquare)
        {
            List<Move> bishopMoves = new List<Move>(13);

            bishopMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesDownLeftDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesUpLeftDiagonal(fromSquare));
            bishopMoves.AddRange(GetMovesDownRightDiagonal(fromSquare));

            return bishopMoves;
        }

        private IEnumerable<Move> GetKnightMoves(int fromSquare)
        {
            List<Move> knightMoves = new List<Move>();
            // Long is three, short is one
            int difUpLeftShort = -10;
            int difUpLeftLong = -17;
            int difUpRightShort = -6;
            int difUpRightLong = -15;
            int difDownRightShort = +10;
            int difDownRightLong = +17;
            int difDownLeftShort = +6;
            int difDownLeftLong = +15;
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

                Square square = this.Board[destination];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    knightMoves.Add(new Move(fromSquare, destination));

                    continue;
                }

                if (square.OccupiedBy.Color == this.OppositeColor)
                {
                    knightMoves.Add(new Move(fromSquare, destination));
                }
            }

            return knightMoves;
        }

        private IEnumerable<Move> GetPawnMoves(int fromSquare)
        {
            List<Move> pawnMoves = new List<Move>(4);
            int direction = (this.SideToMove == Color.White) ? -1 : 1;
            int oneForward = fromSquare + (8 * direction);
            int twoForward = fromSquare + (16 * direction);
            int captureLeft = fromSquare + (9 * direction);
            int captureRight = fromSquare + (7 * direction);

            if (this.Board[oneForward].OccupiedBy.Type == PieceType.None)
            {
                pawnMoves.Add(new Move(fromSquare, oneForward));

                if (this.IsPawnPromotion(oneForward))
                {
                    pawnMoves.Add(new Move(fromSquare, oneForward, new Piece(this.SideToMove, PieceType.Queen)));
                    pawnMoves.Add(new Move(fromSquare, oneForward, new Piece(this.SideToMove, PieceType.Knight)));
                }
            }

            if (this.InitialPawnSquares.Contains(fromSquare)
                && this.Board[fromSquare].OccupiedBy.Color == this.SideToMove)
            {
                pawnMoves.Add(new Move(fromSquare, twoForward));
            }

            if (this.Board[captureLeft].OccupiedBy.Color == this.OppositeColor)
            {
                pawnMoves.Add(new Move(fromSquare, captureLeft));

                if (this.IsPawnPromotion(captureLeft))
                {
                    pawnMoves.Add(new Move(fromSquare, captureLeft, new Piece(this.SideToMove, PieceType.Queen)));
                    pawnMoves.Add(new Move(fromSquare, captureLeft, new Piece(this.SideToMove, PieceType.Knight)));
                }
            }

            if (this.Board[captureRight].OccupiedBy.Color == this.OppositeColor)
            {
                pawnMoves.Add(new Move(fromSquare, captureRight));

                if (this.IsPawnPromotion(captureRight))
                {
                    pawnMoves.Add(new Move(fromSquare, captureRight, new Piece(this.SideToMove, PieceType.Queen)));
                    pawnMoves.Add(new Move(fromSquare, captureRight, new Piece(this.SideToMove, PieceType.Knight)));
                }
            }

            int enPassantSquare = this.GetEnPassantSquare();

            if (enPassantSquare > -1)
            {
                pawnMoves.Add(new Move(fromSquare, enPassantSquare));
            }

            return pawnMoves;
        }

        #endregion

        #region Special Cases

        private bool CanCastleShort()
        {
            if (this.SideToMove == Color.White)
            {
                if (this.WhiteCastled || this.KingIsInCheck() || this.WhiteKingPosition != 60)
                {
                    return false;
                }

                int[] whiteShortCastleSquares = new int[2] { 61, 62 };

                for (int i = 0; i < whiteShortCastleSquares.Length; i++)
                {
                    if (this.Board[whiteShortCastleSquares[i]].OccupiedBy.Type != PieceType.None
                        || this.OpponentControl.Contains(whiteShortCastleSquares[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (this.BlackCastled || this.KingIsInCheck() || this.BlackKingPosition != 4)
            {
                return false;
            }

            int[] blackShortCastleSquares = new int[2] { 5, 6 };

            for (int i = 0; i < blackShortCastleSquares.Length; i++)
            {
                if (this.Board[blackShortCastleSquares[i]].OccupiedBy.Type != PieceType.None
                    || this.OpponentControl.Contains(blackShortCastleSquares[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CanCastleLong()
        {
            if (this.SideToMove == Color.White)
            {
                if (this.WhiteCastled || this.KingIsInCheck() || this.WhiteKingPosition != 60)
                {
                    return false;
                }

                int[] whiteLongCastleSquares = new int[3] { 57, 58, 59 };

                this.OpponentControl.Remove(57); // We can still castle if this square is being controlled

                for (int i = 0; i < whiteLongCastleSquares.Length; i++)
                {
                    if (this.Board[whiteLongCastleSquares[i]].OccupiedBy.Type != PieceType.None
                        || this.OpponentControl.Contains(whiteLongCastleSquares[i]))
                    {
                        return false;
                    }
                }

                this.OpponentControl.Add(57);

                return true;
            }

            if (this.BlackCastled || this.KingIsInCheck() || this.BlackKingPosition != 4)
            {
                return false;
            }

            this.OpponentControl.Remove(1);

            int[] blackLongCastleSquares = new int[3] { 1, 2, 3 };

            for (int i = 0; i < blackLongCastleSquares.Length; i++)
            {
                if (this.Board[blackLongCastleSquares[i]].OccupiedBy.Type != PieceType.None
                    || this.OpponentControl.Contains(blackLongCastleSquares[i]))
                {
                    return false;
                }
            }

            this.OpponentControl.Add(1);

            return true;
        }

        private int GetEnPassantSquare()
        {
            int fromSquare = this.lastMove.FromSquare;
            int toSquare = this.lastMove.ToSquare;
            Piece lastMovedPiece = this.Board[toSquare].OccupiedBy;

            if (lastMovedPiece.Type == PieceType.Pawn)
            {
                if (this.InitialPawnSquares.Contains(fromSquare))
                {
                    int moveDirection = (lastMovedPiece.Color == Color.White) ? -1 : 1;
                    int twoForward = fromSquare + (16 * moveDirection);

                    if (this.lastMove.ToSquare == twoForward)
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
                            if (this.Board[twoForwardOneLeft].OccupiedBy.Type == PieceType.Pawn)
                            {
                                enPassantPossible = true;
                            }
                        }

                        if (rightDif == 1)
                        {
                            if (this.Board[twoForwardOneRight].OccupiedBy.Type == PieceType.Pawn)
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

        private bool IsPawnPromotion(int toSquare)
        {
            return (toSquare < 8 || toSquare > 55);
        }

        #endregion

        #endregion

        #region Utils

        private bool KingIsInCheck()
        {
            return (this.SideToMove == Color.White) ?
                this.OpponentControl.Contains(this.WhiteKingPosition) :
                this.OpponentControl.Contains(this.BlackKingPosition);
        }
        
        private int GetKingPosition()
        {
            return (this.SideToMove == Color.White) ? 
                this.WhiteKingPosition : 
                this.BlackKingPosition;
        }

        private static T MakeDeepCopy<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(ms, obj);

                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        #endregion
    }
}