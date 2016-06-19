namespace butnotquite.Tests
{
    using Defaults;
    using Models;
    using Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChessboardTests
    {
        [TestMethod]
        public void Chessboard_ShouldRecordTheCorrectGameState()
        {
            Chessboard positionOne = Utils.LoadPositionFromFenString("r3k2r/3q4/8/8/2Q1N3/4PP2/5KR1/8 b q - 2 42");

            Assert.AreEqual(42, positionOne.MoveCounter);
            Assert.AreEqual(Color.Black, positionOne.SideToMove);
            Assert.AreEqual(Color.White, positionOne.OppositeColor);
            Assert.AreEqual(false, positionOne.WhiteCanCastle);
            Assert.AreEqual(true, positionOne.BlackCanCastle);
            Assert.AreEqual(53, positionOne.WhiteKingPosition);
            Assert.AreEqual(4, positionOne.BlackKingPosition);
            Assert.AreEqual(-1, positionOne.EnPassantSquare);
            Assert.AreEqual(2, positionOne.FiftyMoveCounter);

            Chessboard positionTwo = Utils.LoadPositionFromFenString("r4rk1/1b2bppp/ppq1p3/2pp3n/5P2/1P1BP3/PBPPQ1PP/R4RK1 w - - 0 19");

            Assert.AreEqual(19, positionTwo.MoveCounter);
            Assert.AreEqual(Color.White, positionTwo.SideToMove);
            Assert.AreEqual(Color.Black, positionTwo.OppositeColor);
            Assert.AreEqual(false, positionTwo.WhiteCanCastle);
            Assert.AreEqual(false, positionTwo.BlackCanCastle);
            Assert.AreEqual(62, positionTwo.WhiteKingPosition);
            Assert.AreEqual(6, positionTwo.BlackKingPosition);
            Assert.AreEqual(-1, positionTwo.EnPassantSquare);
            Assert.AreEqual(0, positionTwo.FiftyMoveCounter);

            Chessboard positionThree = Utils.LoadPositionFromFenString("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w KQkq c6 0 2");

            Assert.AreEqual(2, positionThree.MoveCounter);
            Assert.AreEqual(Color.White, positionThree.SideToMove);
            Assert.AreEqual(Color.Black, positionThree.OppositeColor);
            Assert.AreEqual(true, positionThree.WhiteCanCastle);
            Assert.AreEqual(true, positionThree.BlackCanCastle);
            Assert.AreEqual(60, positionThree.WhiteKingPosition);
            Assert.AreEqual(4, positionThree.BlackKingPosition);
            Assert.AreEqual(18, positionThree.EnPassantSquare);
            Assert.AreEqual(0, positionThree.FiftyMoveCounter);
        }
    }
}
