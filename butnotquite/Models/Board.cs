namespace butnotquite.Models
{
    using Defaults;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Chessboard
    {
        internal Square[] Squares;
        internal int Evaluation;
        internal bool KingInCheck;
        internal int FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;
        internal int RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal bool WhiteCastled;
        internal bool BlackCastled;
        internal bool Stalemate;
        internal bool EnpassantPossible;

        internal Chessboard()
        {
            this.Squares = new Square[64];

            for (int i = 0; i < Squares.Length; i++)
            {
                this.Squares[i] = new Square();
            }
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
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.Pawn);

                    continue;
                }

                if (i == 0 || i == 7)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.Rook);

                    continue;
                }

                if (i == 1 || i == 6)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.Knight);

                    continue;
                }

                if (i == 2 || i == 5)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.Bishop);

                    continue;
                }

                if (i == 3)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.Queen);

                    continue;
                }

                if (i == 4)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.Black, PieceType.King);
                }
            }
        }

        private void FillEmptySquares()
        {
            int blackTerritoryEnd = 16;
            int emptyTerritoryLimit = 47;

            for (int i = blackTerritoryEnd; i <= emptyTerritoryLimit; i++)
            {
                this.Squares[i].OccupiedBy = new Piece();
            }
        }

        private void SetWhitePieces()
        {
            int emptyTerritoryEnd = 48;

            for (int i = emptyTerritoryEnd; i <= 63; i++)
            {
                if (i <= 55)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.Pawn);

                    continue;
                }

                if (i == 56 || i == 63)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.Rook);

                    continue;
                }

                if (i == 57 || i == 62)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.Knight);

                    continue;
                }

                if (i == 58 || i == 61)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.Bishop);

                    continue;
                }

                if (i == 59)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.Queen);

                    continue;
                }

                if (i == 60)
                {
                    this.Squares[i].OccupiedBy = new Piece(Color.White, PieceType.King);
                }
            }
        }

        #endregion

        #region Move Generation

        internal IEnumerable<Move> GetAvailableMoves(Color sideToMove)
        {
            List<Move> availableMoves = new List<Move>(217);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            for (int i = 0; i < 64; i++)
            {
                Square square = this.Squares[i];

                if (square.OccupiedBy.Color != sideToMove || square.OccupiedBy.Type == PieceType.None)
                {
                    continue;
                }

                if (this.KingInCheck)
                {
                    // TODO

                    continue;
                }

                switch (square.OccupiedBy.Type)
                {
                    case PieceType.None:
                        break;
                    case PieceType.Pawn:
                        //availableMoves.AddRange(GetPawnMoves(i));
                        break;
                    case PieceType.Knight:
                        //availableMoves.AddRange(GetKnightMoves(i));
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
                        //availableMoves.AddRange(GetKingMoves(i));
                        break;
                    default:
                        break;
                }
            }

            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            return availableMoves;
        }

        #region Directions

        private IEnumerable<Move> GetMovesUp(int fromSquare)
        {
            List<Move> movesUp = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int topBorder = 0;

            for (int location = fromSquare - 8; location >= topBorder; location -= 8)
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUp.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int bottomBorder = 63;

            for (int location = (fromSquare + 8); location <= bottomBorder; location += 8)
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDown.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int leftBorder = fromSquare - (fromSquare % 8);

            for (int location = (fromSquare - 1); location >= leftBorder; location--)
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesLeft.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int rightBorder = fromSquare + (7 - fromSquare % 8);

            for (int location = (fromSquare + 1); location <= rightBorder; location++)
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesRight.Add(new Move(fromSquare, location));

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            int location = fromSquare - 7;

            while (!northEastBordersAdjusted.Contains(location))
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

                    location -= 7;

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            int location = fromSquare + 7;

            while (!southWestBordersAdjusted.Contains(location))
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownLeft.Add(new Move(fromSquare, location));

                    location += 7;

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            int location = fromSquare - 9;

            while (!northWestBordersAdjusted.Contains(location))
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpLeft.Add(new Move(fromSquare, location));

                    location -= 9;

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
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
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            int location = fromSquare + 9;

            while (!southEastBordersAdjusted.Contains(location))
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesDownRight.Add(new Move(fromSquare, location));

                    location += 9;

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
                {
                    movesDownRight.Add(new Move(fromSquare, location));

                    break;
                }

                location += 9;
            }

            return movesDownRight;
        }

        private IEnumerable<Move> GetLMoves(int fromSquare)
        {
            List<Move> lMoves = new List<Move>();
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            // Long is three, short is one
            int difUpLeftShort = -10;
            int difUpLeftLong = -17;
            int difUpRightShort = -6;
            int difUpRightLong = -15;
            int difDownRightShort = +10;
            int difDownRightLong = +17;
            int difDownLeftShort = +6;
            int difDownLeftLong = +15; 
            int[] jumpPositions = new int[] 
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

            int fromRow = fromSquare / 8;

            for (int i = 0; i < jumpPositions.Length; i++)
            {
                int destination = fromSquare + jumpPositions[i];

                if (destination < 0 || destination > 63)
                {
                    continue;
                }

                int destinationRow = destination / 8;

                if (fromRow == destinationRow) // Jumping over the board
                {
                    continue;
                }

                Square square = this.Squares[i];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    lMoves.Add(new Move(fromSquare, i));

                    continue;
                }

                if (square.OccupiedBy.Color == currentSideColor)
                {
                    break;
                }

                if (square.OccupiedBy.Color != currentSideColor)
                {
                    movesRight.Add(new Move(fromSquare, i));

                    break;
                }
            }

            return lMoves;
        }

        #endregion

        private IEnumerable<Move> GetKingMoves(int fromSquare)
        {
            throw new NotImplementedException();
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
            List<Move> knigtMoves = new List<Move>(8);

            knigtMoves.AddRange(GetLMoves(fromSquare));

            return knigtMoves;
        }

        private IEnumerable<Move> GetPawnMoves(int fromSquare)
        {
            throw new NotImplementedException();
        }

        #endregion        
    }
}