namespace butnotquite.Tests
{
    using Defaults;
    using Models;
    using Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public void FenParser_ShouldSetupStartingPositionCorrectly()
        {
            Chessboard position = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            HashSet<int> emptySquareIndices = new HashSet<int>();

            for (int i = 16; i <= 47; i++)
            {
                emptySquareIndices.Add(i);
            }

            foreach (int emptySquareIndex in emptySquareIndices)
            {
                Assert.AreEqual(new Piece(Color.None, PieceType.None, emptySquareIndex).Type, position.Board[emptySquareIndex].OccupiedBy.Type);
            }

            Assert.AreEqual(new Piece(Color.Black, PieceType.Rook, 0).Type, position.Board[0].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Knight, 1).Type, position.Board[1].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Bishop, 2).Type, position.Board[2].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Queen, 3).Type, position.Board[3].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.King, 4).Type, position.Board[4].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Bishop, 5).Type, position.Board[5].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Knight, 6).Type, position.Board[6].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Rook, 7).Type, position.Board[7].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 8).Type, position.Board[8].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 9).Type, position.Board[9].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 10).Type, position.Board[10].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 11).Type, position.Board[11].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 12).Type, position.Board[12].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 13).Type, position.Board[13].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 14).Type, position.Board[14].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 15).Type, position.Board[15].OccupiedBy.Type);

            Assert.AreEqual(new Piece(Color.White, PieceType.Rook, 56).Type, position.Board[56].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Knight, 57).Type, position.Board[57].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Bishop, 58).Type, position.Board[58].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Queen, 59).Type, position.Board[59].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.King, 60).Type, position.Board[60].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Bishop, 61).Type, position.Board[61].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Knight, 62).Type, position.Board[62].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Rook, 63).Type, position.Board[63].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 48).Type, position.Board[48].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 49).Type, position.Board[49].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 50).Type, position.Board[50].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 51).Type, position.Board[51].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 52).Type, position.Board[52].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 53).Type, position.Board[54].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 54).Type, position.Board[54].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 55).Type, position.Board[55].OccupiedBy.Type);                      
        }

        [TestMethod]
        public void FenParser_ShouldSetupSpecificPositionCorrectly()
        {
            Chessboard position = Utils.LoadPositionFromFenString("r3k2r/4qp1p/8/8/4P3/8/4QP2/R3K1R1 b Qkq - 0 31");
            HashSet<int> emptySquareIndices = new HashSet<int>()
            {
                1, 2, 3, 5, 6, 8, 9, 10, 11, 14, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,28, 29, 30, 31, 32, 33, 34,
                35, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 54, 55, 57, 58, 59, 61, 63
            };

            foreach (int emptySquareIndex in emptySquareIndices)
            {
                Assert.AreEqual(new Piece(Color.None, PieceType.None, emptySquareIndex).Type, position.Board[emptySquareIndex].OccupiedBy.Type);
            }

            Assert.AreEqual(new Piece(Color.Black, PieceType.Rook, 0).Type, position.Board[0].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.King, 4).Type, position.Board[4].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Rook, 7).Type, position.Board[7].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Queen, 12).Type, position.Board[12].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 13).Type, position.Board[13].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.Black, PieceType.Pawn, 15).Type, position.Board[15].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 36).Type, position.Board[36].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Queen, 52).Type, position.Board[52].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Pawn, 53).Type, position.Board[53].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Rook, 56).Type, position.Board[56].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.King, 60).Type, position.Board[60].OccupiedBy.Type);
            Assert.AreEqual(new Piece(Color.White, PieceType.Rook, 62).Type, position.Board[62].OccupiedBy.Type);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "Invalid FEN string.")]
        public void FenParser_ShouldFailWithArgumentException()
        {
            Chessboard position = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        }
    }
}
