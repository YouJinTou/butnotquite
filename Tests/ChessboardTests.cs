namespace Engine.Tests
{
    using Engine.Core;
    using Engine.Defaults;
    using Engine.Models;
    using Engine.Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

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

            Chessboard positionFour = Utils.LoadPositionFromFenString("3k4/1q6/8/2Q1r3/2PN1P2/3p1K2/8/8 w - - 38 72");

            Assert.AreEqual(72, positionFour.MoveCounter);
            Assert.AreEqual(Color.White, positionFour.SideToMove);
            Assert.AreEqual(Color.Black, positionFour.OppositeColor);
            Assert.AreEqual(false, positionFour.WhiteCanCastle);
            Assert.AreEqual(false, positionFour.BlackCanCastle);
            Assert.AreEqual(45, positionFour.WhiteKingPosition);
            Assert.AreEqual(3, positionFour.BlackKingPosition);
            Assert.AreEqual(-1, positionFour.EnPassantSquare);
            Assert.AreEqual(38, positionFour.FiftyMoveCounter);
        }

        [TestMethod]
        public void Chessboard_ShouldRecordCorrectValuesAfterMakeMove()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/4q3/8/2Q1r3/2PN1P2/3p1K2/8/8 b - - 38 72");

            position.MakeMove(new Move(12, 9, Direction.Horizontal));

            Assert.AreEqual(position.Board[12].OccupiedBy.Type, PieceType.None);
            Assert.AreEqual(position.Board[12].OccupiedBy.Position, -1);
            Assert.AreEqual(position.Board[9].OccupiedBy.Type, PieceType.Queen);
            Assert.AreEqual(position.Board[9].OccupiedBy.Position, 9);
        }

        [TestMethod]
        public void Chessboard_ShouldRecordCorrectValuesAfterUndoMove()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/4q3/8/2Q1r3/2PN1P2/3p1K2/8/8 b - - 38 72");
            Move queenMove = new Move(12, 9, Direction.Horizontal);

            Assert.AreEqual(position.Board[9].OccupiedBy.Type, PieceType.None);
            Assert.AreEqual(position.Board[9].OccupiedBy.Color, Color.None);
            Assert.AreEqual(position.Board[9].OccupiedBy.Position, -1);
            Assert.AreEqual(position.Board[12].OccupiedBy.Type, PieceType.Queen);
            Assert.AreEqual(position.Board[12].OccupiedBy.Color, Color.Black);
            Assert.AreEqual(position.Board[12].OccupiedBy.Position, 12);

            position.MakeMove(queenMove);

            Assert.AreEqual(position.Board[12].OccupiedBy.Type, PieceType.None);
            Assert.AreEqual(position.Board[12].OccupiedBy.Type, PieceType.None);
            Assert.AreEqual(position.Board[12].OccupiedBy.Position, -1);
            Assert.AreEqual(position.Board[9].OccupiedBy.Type, PieceType.Queen);
            Assert.AreEqual(position.Board[9].OccupiedBy.Color, Color.Black);
            Assert.AreEqual(position.Board[9].OccupiedBy.Position, 9);

            position.UndoMove(queenMove);

            Assert.AreEqual(position.Board[9].OccupiedBy.Type, PieceType.None);
            Assert.AreEqual(position.Board[9].OccupiedBy.Color, Color.None);
            Assert.AreEqual(position.Board[9].OccupiedBy.Position, -1);
            Assert.AreEqual(position.Board[12].OccupiedBy.Type, PieceType.Queen);
            Assert.AreEqual(position.Board[12].OccupiedBy.Color, Color.Black);
            Assert.AreEqual(position.Board[12].OccupiedBy.Position, 12);            
        }

        [TestMethod]
        public void Chessboard_ShouldCorrectlyIdentifyWhenKingInCheck()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/4q3/8/2Q1r3/2PN1P2/3p1K2/8/8 b - - 38 72");

            position.MakeMove(new Move(12, 9, Direction.Horizontal));

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.AreEqual(true, position.WhiteInCheck);
            Assert.AreEqual(false, position.BlackInCheck);
        }

        [TestMethod]
        public void Chessboard_ShouldCorrectlyIdentifyWhenKingInCheckByKnight()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/5q1n/P7/2Q1r3/2PN1P2/3p1K2/8/8 b - - 15 58");

            position.MakeMove(new Move(15, 30, Direction.L));

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.AreEqual(true, position.WhiteInCheck);
            Assert.AreEqual(false, position.BlackInCheck);
        }
    }
}
