namespace butnotquite.Tests
{
    using Core;
    using Defaults;
    using Models;
    using Utils;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;
    [TestClass]
    public class PieceMovementTests
    {
        #region Pawns

        [TestMethod]
        public void PawnMovement_ResultShouldContainSixOrdinaryPawnMoves()
        {
            Chessboard board = Utils.LoadPositionFromFenString("8/8/3k4/8/7p/NnP1p1p1/PP3P2/5K2 w - - 0 42");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(board);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(48, 41),
                new Move(42, 34),
                new Move(53, 45),
                new Move(53, 37),
                new Move(53, 44),
                new Move(53, 46)
            };

            foreach (Move move in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == move.FromSquare
                && m.ToSquare == move.ToSquare));
            }
        }

        [TestMethod]
        public void PawnMovement_ResultShouldContain15PawnMovesIncludingPromotions()
        {
            Chessboard board = Utils.LoadPositionFromFenString("1q3n1r/P5P1/3k4/8/8/NnP1p1p1/PP3P11/5K2 w - - 0 50 ");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(board);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(48, 41),
                new Move(42, 34),
                new Move(53, 45),
                new Move(53, 37),
                new Move(53, 44),
                new Move(53, 46),
                new Move(8, 0, new Piece(Color.White, PieceType.Queen, 0)),
                new Move(8, 0, new Piece(Color.White, PieceType.Knight, 0)),
                new Move(8, 1, new Piece(Color.White, PieceType.Queen, 1)),
                new Move(8, 1, new Piece(Color.White, PieceType.Knight, 1)),
                new Move(14, 5, new Piece(Color.White, PieceType.Queen, 5)),
                new Move(14, 5, new Piece(Color.White, PieceType.Knight, 5)),
                new Move(14, 6, new Piece(Color.White, PieceType.Queen, 6)),
                new Move(14, 6, new Piece(Color.White, PieceType.Knight, 6)),
                new Move(14, 7, new Piece(Color.White, PieceType.Queen, 7)),
                new Move(14, 7, new Piece(Color.White, PieceType.Knight, 7))
            };

            foreach (Move testMove in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                   m.FromSquare == testMove.FromSquare
                   && m.ToSquare == testMove.ToSquare
                   && m.PromotionPiece.Equals(testMove.PromotionPiece)
                   && m.KingFromSquare == testMove.KingFromSquare
                   && m.KingToSquare == testMove.KingToSquare
                   && m.RookFromSquare == testMove.RookFromSquare
                   && m.RookToSquare == testMove.RookToSquare));                
            }
        }

        [TestMethod]
        public void PawnMovement_ResultShouldContainThreePawnMovesIncludingEnPassant()
        {
            Chessboard board = Utils.LoadPositionFromFenString("8/8/3k4/8/4pP2/NnP3p1/PP6/5K2 b - f3 0 42");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(board);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(36, 45),
                new Move(36, 44),
                new Move(46, 54)
            };

            foreach (Move move in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare));
            }
        }

        #endregion
    }
}
