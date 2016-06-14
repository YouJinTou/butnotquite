namespace butnotquite.Objects
{
    using Defaults;

    using System;
    using System.Collections.Generic;

    internal sealed class Chessboard
    {
        internal Square[] Squares;
        internal short Evaluation;
        internal bool KingInCheck;
        internal byte FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;
        internal byte RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal bool WhiteCastled;
        internal bool BlackCastled;
        internal bool Stalemate;
        internal bool EnpassantPossible;

        internal Chessboard()
        {
            this.Squares = new Square[64];

            for (byte i = 0; i < Squares.Length; i++)
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
            byte blackTerritoryLimit = 15;

            for (byte i = 0; i <= blackTerritoryLimit; i++)
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
            byte blackTerritoryEnd = 16;
            byte emptyTerritoryLimit = 47;

            for (byte i = blackTerritoryEnd; i <= emptyTerritoryLimit; i++)
            {
                this.Squares[i].OccupiedBy = new Piece();
            }
        }

        private void SetWhitePieces()
        {
            byte emptyTerritoryEnd = 48;

            for (byte i = emptyTerritoryEnd; i <= 63; i++)
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
            List<Move> availableMoves = new List<Move>(200);
             
            for (byte i = 0; i < 64; i++)
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
                    case PieceType.King:
                        //availableMoves.AddRange(GetKingMoves(i));
                        break;
                    case PieceType.Queen:
                        availableMoves.AddRange(GetQueenMoves(i));
                        break;
                    case PieceType.Rook:
                        //availableMoves.AddRange(GetRookMoves(i));
                        break;
                    case PieceType.Bishop:
                        //availableMoves.AddRange(GetBishopMoves(i));
                        break;
                    case PieceType.Knight:
                        //availableMoves.AddRange(GetKnightMoves(i));
                        break;
                    case PieceType.Pawn:
                        //availableMoves.AddRange(GetPawnMoves(i));
                        break;
                    default:
                        return null;
                }

            }

            return availableMoves;
        }

        private IEnumerable<Move> GetMovesUp(byte fromSquare)
        {
            List<Move> movesUp = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            byte topBorder = 0;

            for (byte location = (byte)(fromSquare - 8); location >= topBorder; location -= 8)
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

        private IEnumerable<Move> GetMovesDown(byte fromSquare)
        {
            List<Move> movesDown = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            byte bottomBorder = 63;

            for (byte location = (byte)(fromSquare + 8); location <= bottomBorder; location += 8)
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

        private IEnumerable<Move> GetMovesLeft(byte fromSquare)
        {
            List<Move> movesLeft = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int leftBorder = fromSquare - (fromSquare % 8);

            for (byte location = (byte)(fromSquare - 1); location >= leftBorder; location--)
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

        private IEnumerable<Move> GetMovesRight(byte fromSquare)
        {
            List<Move> movesRight = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;
            int rightBorder = fromSquare + (7 - fromSquare % 8);

            for (byte location = (byte)(fromSquare + 1); location <= rightBorder; location++)
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

        private IEnumerable<Move> GetMovesUpRightDiagonal(byte fromSquare)
        {
            List<Move> movesUpRight = new List<Move>(7);
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            byte location = (byte)(fromSquare - 7); // No need to check the current square
            int prevRankMinusFile = this.GetRankMinusFile(fromSquare);
            int currentRankMinusFile = this.GetRankMinusFile(location);

            while (prevRankMinusFile == currentRankMinusFile)
            {
                Square square = this.Squares[location];

                if (square.OccupiedBy.Type == PieceType.None)
                {
                    movesUpRight.Add(new Move(fromSquare, location));

                    prevRankMinusFile = currentRankMinusFile;
                    location -= 7;
                    currentRankMinusFile = this.GetRankMinusFile(location);

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

                prevRankMinusFile = currentRankMinusFile;
                location -= 7;
                currentRankMinusFile = this.GetRankMinusFile(location);
            }            

            return movesUpRight;
        }

        private IEnumerable<Move> GetKingMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Move> GetQueenMoves(byte fromSquare)
        {
            List<Move> queenMoves = new List<Move>(40);

            queenMoves.AddRange(GetMovesUp(fromSquare));
            queenMoves.AddRange(GetMovesDown(fromSquare));
            queenMoves.AddRange(GetMovesLeft(fromSquare));
            queenMoves.AddRange(GetMovesRight(fromSquare));
            queenMoves.AddRange(GetMovesUpRightDiagonal(fromSquare));

            return queenMoves;
        }

        private IEnumerable<Move> GetRookMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Move> GetBishopMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Move> GetKnightMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Move> GetPawnMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utils

        private int GetRankMinusFile(int fromSquare)
        {
            int row = 7 - (fromSquare / 8);
            int col = fromSquare % 8;
            int rankMinusFile = row - col;

            return rankMinusFile;
        }

        #endregion
    }
}