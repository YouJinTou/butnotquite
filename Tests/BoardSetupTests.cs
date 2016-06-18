namespace butnotquite.Tests
{
    using Defaults;
    using Models;
    using Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class BoardSetupTests
    {
        [TestMethod]
        public void ParseFen_ShouldSetupStartingPosition()
        {
            Chessboard board = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            // Black
            for (int i = 0; i <= 15; i++)
            {
                if (i >= 8)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 0 || i == 7)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Rook, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 1 || i == 6)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Knight, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 2 || i == 5)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Bishop, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 3)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.Queen, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 4)
                {
                    Assert.AreEqual(new Piece(Color.Black, PieceType.King, i).Type, board.Board[i].OccupiedBy.Type);
                }
            }

            // Middle
            Piece emptyPiece = new Piece();

            for (int i = 16; i <= 47; i++)
            {
                Assert.AreEqual(emptyPiece.Type, board.Board[i].OccupiedBy.Type);
            }

            // White
            for (int i = 48; i <= 63; i++)
            {
                if (i <= 55)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 56 || i == 63)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Rook, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 57 || i == 62)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Knight, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 58 || i == 61)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Bishop, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 59)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.Queen, i).Type, board.Board[i].OccupiedBy.Type);

                    continue;
                }

                if (i == 60)
                {
                    Assert.AreEqual(new Piece(Color.White, PieceType.King, i).Type, board.Board[i].OccupiedBy.Type) ;
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid FEN string.")]
        public void ParseFen_ShouldFailWithArgumentException()
        {
            Chessboard board = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }

        [TestMethod]
        public void ParseFen_ShouldRecordTheCorrectGameState()
        {
            Chessboard boardOne = Utils.LoadPositionFromFenString("r3k2r/3q4/8/8/2Q1N3/4PP2/5KR1/8 b q - 2 42");

            Assert.AreEqual(42, boardOne.MoveCounter);
            Assert.AreEqual(Color.Black, boardOne.SideToMove);
            Assert.AreEqual(Color.White, boardOne.OppositeColor);
            Assert.AreEqual(false, boardOne.WhiteCanCastle);
            Assert.AreEqual(true, boardOne.BlackCanCastle);
            Assert.AreEqual(53, boardOne.WhiteKingPosition);
            Assert.AreEqual(4, boardOne.BlackKingPosition);
            Assert.AreEqual(-1, boardOne.EnPassantSquare);
            Assert.AreEqual(2, boardOne.FiftyMoveCounter);

            Chessboard boardTwo = Utils.LoadPositionFromFenString("r4rk1/1b2bppp/ppq1p3/2pp3n/5P2/1P1BP3/PBPPQ1PP/R4RK1 w - - 0 19");

            Assert.AreEqual(19, boardTwo.MoveCounter);
            Assert.AreEqual(Color.White, boardTwo.SideToMove);
            Assert.AreEqual(Color.Black, boardTwo.OppositeColor);
            Assert.AreEqual(false, boardTwo.WhiteCanCastle);
            Assert.AreEqual(false, boardTwo.BlackCanCastle);
            Assert.AreEqual(62, boardTwo.WhiteKingPosition);
            Assert.AreEqual(6, boardTwo.BlackKingPosition);
            Assert.AreEqual(-1, boardTwo.EnPassantSquare);
            Assert.AreEqual(0, boardTwo.FiftyMoveCounter);

            Chessboard boardThree = Utils.LoadPositionFromFenString("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2");

            Assert.AreEqual(2, boardThree.MoveCounter);
            Assert.AreEqual(Color.White, boardThree.SideToMove);
            Assert.AreEqual(Color.Black, boardThree.OppositeColor);
            Assert.AreEqual(true, boardThree.WhiteCanCastle);
            Assert.AreEqual(true, boardThree.BlackCanCastle);
            Assert.AreEqual(60, boardThree.WhiteKingPosition);
            Assert.AreEqual(4, boardThree.BlackKingPosition);
            Assert.AreEqual(18, boardThree.EnPassantSquare);
            Assert.AreEqual(0, boardThree.FiftyMoveCounter);
        }
    }
}
