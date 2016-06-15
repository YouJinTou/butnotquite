using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace butnotquite.Tests
{
    using Defaults;
    using Models;

    [TestClass]
    public class BoardSetupTests
    {
        Chessboard board;

        [TestMethod]
        public void Board_PopulateAllSquares_StartingPosition()
        {
            this.board = new Chessboard();

            board.InitializeStartingPosition();

            // Black
            for (int i = 0; i <= 15; i++)
            {
                if (i >= 8)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 0 || i == 7)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Rook).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 1 || i == 6)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Knight).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 2 || i == 5)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Bishop).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 3)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Queen).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 4)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.King).Type, this.board.Board[i].OccupiedBy.Type);
                }
            }

            // Middle
            Piece emptyPiece = new Piece();

            for (int i = 16; i <= 47; i++)
            {
                Assert.AreEqual(emptyPiece.Type, this.board.Board[i].OccupiedBy.Type);
            }

            // White
            for (int i = 48; i <= 63; i++)
            {
                if (i <= 55)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Pawn).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 56 || i == 63)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Rook).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 57 || i == 62)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Knight).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 58 || i == 61)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Bishop).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 59)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Queen).Type, this.board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 60)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.King).Type, this.board.Board[i].OccupiedBy.Type) ;
                }
            }
        }
    }
}
