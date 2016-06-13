namespace butnotquite
{
    using butnotquite.Defaults;

    internal sealed class Chessboard
    {
        internal Square[] Board;

        internal Chessboard()
        {
            this.Board = new Square[64];

            for (byte i = 0; i < Board.Length; i++)
            {
                this.Board[i] = new Square();
            }
        }

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
            byte blackTerritoryEnd = 16;
            byte emptyTerritoryLimit = 47;

            for (byte i = blackTerritoryEnd; i <= emptyTerritoryLimit; i++)
            {
                this.Board[i].OccupiedBy = new Piece();
            }
        }

        private void SetWhitePieces()
        {
            byte emptyTerritoryEnd = 48;

            for (byte i = emptyTerritoryEnd; i <= 63; i++)
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

                if (i == 56 || i == 61)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Bishop);

                    continue;
                }

                if (i == 55)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.Queen);

                    continue;
                }

                if (i == 54)
                {
                    this.Board[i].OccupiedBy = new Piece(Color.White, PieceType.King);
                }
            }
        }
    }
}
