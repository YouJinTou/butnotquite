namespace butnotquite.Objects
{
    using butnotquite.Defaults;

    using System.Collections.Generic;
    using System;

    internal sealed class Chessboard
    {
        internal Square[] Squares;
        internal short Evaluation;
        internal bool KingInCheck;
        internal byte FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;
        internal byte RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal bool WhiteCanCastle;
        internal bool BlackCanCastle;
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

        internal IEnumerable<byte> GetAvailableMoves(Color sideToMove)
        {
            List<byte> availableMoves = new List<byte>(200);
             
            for (byte i = 0; i < 64; i++)
            {
                Piece piece = this.Squares[i].OccupiedBy;

                if (piece.Color != sideToMove || piece.Type == PieceType.None)
                {
                    continue;
                }

                if (this.KingInCheck)
                {
                    // TODO

                    continue;
                }

                switch (piece.Type)
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

        private IEnumerable<byte> GetMovesUp(byte fromSquare)
        {
            List<byte> movesUp = new List<byte>();
            Color currentSideColor = this.Squares[fromSquare].OccupiedBy.Color;

            for (byte i = (byte)(fromSquare - 8); i >= 0; i -= 8)
             {
                Piece piece = this.Squares[i].OccupiedBy;

                if (piece.Type == PieceType.None)
                {
                    movesUp.Add(i);

                    continue;
                }

                if (piece.Color == currentSideColor)
                {
                    break;
                }                

                if (piece.Color != currentSideColor)
                {
                    movesUp.Add(i);

                    break;
                }
            }

            return movesUp;
        }

        private IEnumerable<byte> GetKingMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<byte> GetQueenMoves(byte fromSquare)
        {
            List<byte> queenMoves = new List<byte>(40);

            queenMoves.AddRange(GetMovesUp(fromSquare));

            return queenMoves;
        }

        private IEnumerable<byte> GetRookMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<byte> GetBishopMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<byte> GetKnightMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<byte> GetPawnMoves(byte fromSquare)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
