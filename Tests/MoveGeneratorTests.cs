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
    public class MoveGeneratorTests
    {
        #region All Pieces

        [TestMethod]
        public void AllPiecesMovement_ShouldList218MovesForWhiteAndZeroMovesForBlack_1()
        {
            Chessboard positionWhite = Utils.LoadPositionFromFenString("R6R/3Q4/1Q4Q1/4Q3/2Q4Q/Q4Q2/pp1Q4/kBNN1KB1 w - - 0 1");
            Chessboard positionBlack = Utils.LoadPositionFromFenString("R6R/3Q4/1Q4Q1/4Q3/2Q4Q/Q4Q2/pp1Q4/kBNN1KB1 b - - 0 1");
            List<Move> availableMovesWhite = MoveGenerator.GetAvailableMoves(positionWhite);
            List<Move> availableMovesBlack = MoveGenerator.GetAvailableMoves(positionBlack);
            
            Assert.IsTrue(availableMovesWhite.Count == 218);            
            Assert.IsTrue(availableMovesBlack.Count == 0);
        }

        [TestMethod]
        public void AllPiecesMovement_ShouldList218MovesForWhiteAndZeroMovesForBlack_2()
        {
            Chessboard positionWhite = Utils.LoadPositionFromFenString("3Q4/1Q4Q1/4Q3/2Q4R/Q4Q2/3Q4/1Q4Rp/1K1BBNNk w - - 0 1");
            Chessboard positionBlack = Utils.LoadPositionFromFenString("3Q4/1Q4Q1/4Q3/2Q4R/Q4Q2/3Q4/1Q4Rp/1K1BBNNk b - - 0 1");
            List<Move> availableMovesWhite = MoveGenerator.GetAvailableMoves(positionWhite);
            List<Move> availableMovesBlack = MoveGenerator.GetAvailableMoves(positionBlack);

            Assert.IsTrue(availableMovesWhite.Count == 218);
            Assert.IsTrue(availableMovesBlack.Count == 0);
        }

        #endregion

        #region King

        [TestMethod]
        public void KingMovement_ShouldIncludeFourGenericKingMoves()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/6rp/k4p2/4r3/1P2P3/1K6/5P2/3R3R b - - 2 38");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> kingTestMoves = new List<Move>()
            {
                new Move(16, 8, Direction.SingleSquare),
                new Move(16, 9, Direction.SingleSquare),
                new Move(16, 17, Direction.SingleSquare),
                new Move(16, 25, Direction.SingleSquare)
            };

            foreach (Move move in kingTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction
                    && m.KingFromSquare == move.KingFromSquare
                    && m.KingToSquare == move.KingToSquare
                    && m.RookFromSquare == move.RookFromSquare
                    && m.RookToSquare == move.RookToSquare));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 16
                    && m.ToSquare == 7
                    && m.Direction == Direction.SingleSquare));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 16
                    && m.ToSquare == 15
                    && m.Direction == Direction.SingleSquare));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 16
                    && m.ToSquare == 23
                    && m.Direction == Direction.SingleSquare));
        }

        [TestMethod]
        public void KingMovement_ShouldIncludeKingsideButNotQueensideCastlingForWhite()
        {
            Chessboard position = Utils.LoadPositionFromFenString("5k1r/4qp1p/8/8/4P3/8/3Q1P2/3RK2R w K - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> kingTestMoves = new List<Move>()
            {
                new Move(60, 62, 63, 61),
                new Move(60, 61, Direction.SingleSquare),
                new Move(60, 52, Direction.SingleSquare)
            };

            foreach (Move move in kingTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction
                    && m.KingFromSquare == move.KingFromSquare
                    && m.KingToSquare == move.KingToSquare
                    && m.RookFromSquare == move.RookFromSquare
                    && m.RookToSquare == move.RookToSquare));
            }
        }

        [TestMethod]
        public void KingMovement_ShouldIncludeKingsideButNotQueensideCastlingForBlack()
        {
            Chessboard position = Utils.LoadPositionFromFenString("4k2r/4qp1p/8/8/4P3/8/3Q1P2/3RK2R b Kk - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> kingTestMoves = new List<Move>()
            {
                new Move(4, 6, 7, 5),
                new Move(4, 5, Direction.SingleSquare)
            };

            foreach (Move move in kingTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction
                    && m.KingFromSquare == move.KingFromSquare
                    && m.KingToSquare == move.KingToSquare
                    && m.RookFromSquare == move.RookFromSquare
                    && m.RookToSquare == move.RookToSquare));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 4
                && m.ToSquare == 3));
        }

        [TestMethod]
        public void KingMovement_ShouldIncludeQueensideButNotKingsideCastlingForWhite()
        {
            Chessboard position = Utils.LoadPositionFromFenString("5k1r/4qp1p/8/8/4P3/8/3Q1P2/R3K1R1 w Qq - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> kingTestMoves = new List<Move>()
            {
                new Move(60, 58, 56, 59),
                new Move(60, 61, Direction.SingleSquare),
                new Move(60, 52, Direction.SingleSquare),
                new Move(60, 59, Direction.SingleSquare)
            };

            foreach (Move move in kingTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction
                    && m.KingFromSquare == move.KingFromSquare
                    && m.KingToSquare == move.KingToSquare
                    && m.RookFromSquare == move.RookFromSquare
                    && m.RookToSquare == move.RookToSquare));
            }
        }

        [TestMethod]
        public void KingMovement_ShouldIncludeQueensideButNotKingsideCastlingForBlack()
        {
            Chessboard position = Utils.LoadPositionFromFenString("r3k2r/4qp1p/8/8/4P3/8/4QP2/1R2K1R1 b Qkq - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> kingTestMoves = new List<Move>()
            {
                new Move(4, 2, 0, 3),
                new Move(4, 5, Direction.SingleSquare),
                new Move(4, 3, Direction.SingleSquare),
                new Move(4, 11, Direction.SingleSquare)
            };

            foreach (Move move in kingTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction
                    && m.KingFromSquare == move.KingFromSquare
                    && m.KingToSquare == move.KingToSquare
                    && m.RookFromSquare == move.RookFromSquare
                    && m.RookToSquare == move.RookToSquare));
            }

            Assert.IsTrue(!availableMoves.Any(m => // Short castling
                m.KingFromSquare == 4
                && m.KingToSquare == 6
                && m.RookFromSquare == 7
                && m.RookToSquare == 5));
        }

        [TestMethod]
        public void KingMovement_ShouldNotHaveAnyLegalMoves()
        {
            Chessboard position = Utils.LoadPositionFromFenString("r3k1Nr/4qp1p/6N1/8/4P3/8/4QP2/3RK1R1 b Qkq - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.IsTrue(!availableMoves.Any(m => // Short castling
                m.KingFromSquare == 4
                && m.KingToSquare == 6
                && m.RookFromSquare == 7
                && m.RookToSquare == 5));
            Assert.IsTrue(!availableMoves.Any(m => // Long castling
                m.KingFromSquare == 4
                && m.KingToSquare == 2
                && m.RookFromSquare == 0
                && m.RookToSquare == 3));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 4
                && m.ToSquare == 5));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 4
                && m.ToSquare == 3));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 4
                && m.ToSquare == 11));
        }

        [TestMethod]
        public void KingMovement_ShouldNotBeAbleToApproachEnemyKing()
        {
            Chessboard position = Utils.LoadPositionFromFenString("7k/8/6K1/7P/8/8/8/8 b - - 8 49");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 6));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 8));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 14));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 15));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 16));
        }

        #endregion

        #region Common Movement Patterns

        [TestMethod]
        public void QueenRookBishop_ShouldListCorrectMovesInDownRightDirection()
        {
            Chessboard position = Utils.LoadPositionFromFenString("Q6N/8/4qk2/P7/8/5P2/6K1/2r5 w - - 3 41");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> queenTestMoves = new List<Move>()
            {
                new Move(0, 1, Direction.Horizontal),
                new Move(0, 2, Direction.Horizontal),
                new Move(0, 3, Direction.Horizontal),
                new Move(0, 4, Direction.Horizontal),
                new Move(0, 5, Direction.Horizontal),
                new Move(0, 6, Direction.Horizontal),
                new Move(0, 8, Direction.Vertical),
                new Move(0, 16, Direction.Vertical),
                new Move(0, 16, Direction.Vertical),
                new Move(0, 9, Direction.DownRightUpLeft),
                new Move(0, 18, Direction.DownRightUpLeft),
                new Move(0, 27, Direction.DownRightUpLeft),
                new Move(0, 36, Direction.DownRightUpLeft)                
            };

            foreach (Move move in queenTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 0
               && m.ToSquare == 7));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 0
               && m.ToSquare == 24));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 0
               && m.ToSquare == 45));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 0
                && m.ToSquare == -1));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 0
                && m.ToSquare == -7));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 0
                && m.ToSquare == -8));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 0
                && m.ToSquare == -9));
        }

        [TestMethod]
        public void QueenRookBishop_ShouldListCorrectMovesInDownLeftDirection()
        {
            Chessboard position = Utils.LoadPositionFromFenString("1N5Q/8/3kq3/8/8/2P2P2/6K1/2r5 w - - 3 41");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> queenTestMoves = new List<Move>()
            {
                new Move(7, 6, Direction.Horizontal),
                new Move(7, 5, Direction.Horizontal),
                new Move(7, 4, Direction.Horizontal),
                new Move(7, 3, Direction.Horizontal),
                new Move(7, 2, Direction.Horizontal),
                new Move(7, 15, Direction.Vertical),
                new Move(7, 23, Direction.Vertical),
                new Move(7, 31, Direction.Vertical),
                new Move(7, 39, Direction.Vertical),
                new Move(7, 47, Direction.Vertical),
                new Move(7, 55, Direction.Vertical),
                new Move(7, 63, Direction.Vertical),
                new Move(7, 14, Direction.DownLeftUpRight),
                new Move(7, 21, Direction.DownLeftUpRight),
                new Move(7, 28, Direction.DownLeftUpRight),
                new Move(7, 35, Direction.DownLeftUpRight)
            };

            foreach (Move move in queenTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 7
               && m.ToSquare == 0));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 7
               && m.ToSquare == 1));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == 8));
            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 7
              && m.ToSquare == 13));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 7
               && m.ToSquare == 71));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == -2));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 7
                && m.ToSquare == -1));
        }

        [TestMethod]
        public void QueenRookBishop_ShouldListCorrectMovesInUpRightDirection()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/8/3kq3/8/3p4/5P2/N5K1/Q3r3 w - - 3 41");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> queenTestMoves = new List<Move>()
            {
                new Move(56, 49, Direction.DownLeftUpRight),
                new Move(56, 42, Direction.DownLeftUpRight),
                new Move(56, 35, Direction.DownLeftUpRight),
                new Move(56, 57, Direction.Horizontal),
                new Move(56, 58, Direction.Horizontal),
                new Move(56, 59, Direction.Horizontal),
                new Move(56, 60, Direction.Horizontal)
            };

            foreach (Move move in queenTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 56
              && m.ToSquare == 28));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 56
               && m.ToSquare == 48));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 56
                && m.ToSquare == 50));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 56
                && m.ToSquare == 47));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 56
               && m.ToSquare == 55));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 56
               && m.ToSquare == 61));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 56
                && m.ToSquare == 63));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 56
                && m.ToSquare == 64));
        }

        [TestMethod]
        public void QueenRookBishop_ShouldListCorrectMovesInUpLeftDirection()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/8/3k4/8/3p1P2/6K1/Nq6/r6Q w - - 5 48");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> queenTestMoves = new List<Move>()
            {
                new Move(63, 54, Direction.DownRightUpLeft),
                new Move(63, 45, Direction.DownRightUpLeft),
                new Move(63, 36, Direction.DownRightUpLeft),
                new Move(63, 27, Direction.DownRightUpLeft),
                new Move(63, 18, Direction.DownRightUpLeft),
                new Move(63, 9, Direction.DownRightUpLeft),
                new Move(63, 0, Direction.DownRightUpLeft),
                new Move(63, 62, Direction.Horizontal),
                new Move(63, 61, Direction.Horizontal),
                new Move(63, 60, Direction.Horizontal),
                new Move(63, 59, Direction.Horizontal),
                new Move(63, 58, Direction.Horizontal),
                new Move(63, 57, Direction.Horizontal),
                new Move(63, 56, Direction.Horizontal),
                new Move(63, 55, Direction.Vertical),
                new Move(63, 47, Direction.Vertical),
                new Move(63, 39, Direction.Vertical),
                new Move(63, 31, Direction.Vertical),
                new Move(63, 23, Direction.Vertical),
                new Move(63, 15, Direction.Vertical),
                new Move(63, 7, Direction.Vertical),
            };

            foreach (Move move in queenTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 63
              && m.ToSquare == -9));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 63
               && m.ToSquare == -1));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 63
               && m.ToSquare == 53));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 63
                && m.ToSquare == 64));  
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 63
               && m.ToSquare == 70));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 63
               && m.ToSquare == 71));
            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 63
              && m.ToSquare == 72));
        }

        [TestMethod]
        public void QueenRookBishop_ShouldListCorrectMovesInAllDirections()
        {
            Chessboard position = Utils.LoadPositionFromFenString("4k2r/2qpb3/4n1p1/3pRb2/1r2QP2/2NP1P2/1PP1B3/4K3 w k - 0 31");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> queenTestMoves = new List<Move>()
            {
                new Move(36, 35, Direction.Horizontal),
                new Move(36, 34, Direction.Horizontal),
                new Move(36, 33, Direction.Horizontal),
                new Move(36, 27, Direction.DownRightUpLeft),
                new Move(36, 29, Direction.DownLeftUpRight),
                new Move(36, 44, Direction.Vertical)
            };

            foreach (Move move in queenTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 36
              && m.ToSquare == 18));
            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 36
              && m.ToSquare == 22));
            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 36
              && m.ToSquare == 28));
            Assert.IsTrue(!availableMoves.Any(m =>
              m.FromSquare == 36
              && m.ToSquare == 32));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 36
               && m.ToSquare == 37));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 36
               && m.ToSquare == 43));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 36
               && m.ToSquare == 45));
            Assert.IsTrue(!availableMoves.Any(m =>
               m.FromSquare == 36
               && m.ToSquare == 52));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 36
                && m.ToSquare == 53));
        }

        #endregion

        #region Knight

        [TestMethod]
        public void Knight_ShouldListCorrectMovesFromBoardEdges()
        {
            Chessboard position = Utils.LoadPositionFromFenString("n6n/2p2P2/1p4p1/8/3k1K2/1P6/2p2P2/n6n b - - 0 1");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> knightTestMoves = new List<Move>()
            {
                new Move(7, 13, Direction.L),
                new Move(56, 41, Direction.L),
                new Move(63, 53, Direction.L),
                new Move(63, 46, Direction.L)
            };

            foreach (Move move in knightTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }


            // Top-left knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == -10));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == -17));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == -6));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == -15));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == 6));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 0
                    && m.ToSquare == 15));

            // Top-right knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == -3));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == -10));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == 1));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == -8));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == 17));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 7
                    && m.ToSquare == 24));

            // Bottom-left knight
            Assert.IsTrue(!availableMoves.Any(m =>
                   m.FromSquare == 56
                   && m.ToSquare == 46));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 56
                    && m.ToSquare == 39));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 56
                    && m.ToSquare == 66));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 56
                    && m.ToSquare == 73));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 56
                    && m.ToSquare == 62));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 56
                    && m.ToSquare == 71));

            // Bottom-right knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 57));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 48));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 73));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 80));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 69));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 63
                    && m.ToSquare == 78));
        }

        [TestMethod]
        public void Knight_ShouldListCorrectMovesFromBoardEdgesWithOffset1()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/1n3Pn1/1p4p1/2p3K1/3k4/1P6/1np2Pn1/8 b - - 0 1");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> knightTestMoves = new List<Move>()
            {
                new Move(9, 3, Direction.L),
                new Move(9, 19, Direction.L),
                new Move(9, 24, Direction.L),
                new Move(14, 4, Direction.L),
                new Move(14, 20, Direction.L),
                new Move(14, 29, Direction.L),
                new Move(14, 31, Direction.L),
                new Move(49, 32, Direction.L),
                new Move(49, 34, Direction.L),
                new Move(49, 43, Direction.L),
                new Move(49, 59, Direction.L),
                new Move(54, 39, Direction.L),
                new Move(54, 37, Direction.L),
                new Move(54, 44, Direction.L),
                new Move(54, 60, Direction.L)
            };

            foreach (Move move in knightTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }

            // Top-left knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 9
                    && m.ToSquare == -1));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 9
                    && m.ToSquare == -8));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 9
                    && m.ToSquare == -6));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 9
                    && m.ToSquare == 26));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 9
                    && m.ToSquare == 15));

            // Top-right knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 14
                    && m.ToSquare == -3));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 14
                    && m.ToSquare == 8));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 14
                    && m.ToSquare == -1));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 14
                    && m.ToSquare == 24));

            // Bottom-left knight
            Assert.IsTrue(!availableMoves.Any(m =>
                   m.FromSquare == 49
                   && m.ToSquare == 39));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 49
                    && m.ToSquare == 66));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 49
                    && m.ToSquare == 55));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 49
                    && m.ToSquare == 64));

            // Bottom-right knight
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 54
                    && m.ToSquare == 48));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 54
                    && m.ToSquare == 64));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 54
                    && m.ToSquare == 71));
            Assert.IsTrue(!availableMoves.Any(m =>
                    m.FromSquare == 54
                    && m.ToSquare == 69));
        }

        #endregion

        #region Pawn

        [TestMethod]
        public void PawnMovement_ShouldContainSixOrdinaryPawnMoves()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/8/3k4/8/7p/NnP1p1p1/PP3P2/5K2 w - - 0 42");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(48, 41, Direction.DownLeftUpRight),
                new Move(42, 34, Direction.Vertical),
                new Move(53, 45, Direction.Vertical),
                new Move(53, 37, Direction.Vertical),
                new Move(53, 44, Direction.DownRightUpLeft),
                new Move(53, 46, Direction.DownLeftUpRight)
            };

            foreach (Move move in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == move.FromSquare
                && m.ToSquare == move.ToSquare
                && m.Direction == move.Direction));
            }
        }

        [TestMethod]
        public void PawnMovement_ShouldContain16PawnMovesIncludingPromotions()
        {
            Chessboard position = Utils.LoadPositionFromFenString("1q3n1r/P5P1/3k4/8/8/NnP1p1p1/PP3P11/5K2 w - - 0 50");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(48, 41, Direction.DownLeftUpRight),
                new Move(42, 34, Direction.Vertical),
                new Move(53, 45, Direction.Vertical),
                new Move(53, 37, Direction.Vertical),
                new Move(53, 44, Direction.DownRightUpLeft),
                new Move(53, 46, Direction.DownLeftUpRight),
                new Move(8, 0, Direction.Vertical, new Piece(Color.White, PieceType.Queen, 0)),
                new Move(8, 0, Direction.Vertical, new Piece(Color.White, PieceType.Knight, 0)),
                new Move(8, 1, Direction.DownLeftUpRight, new Piece(Color.White, PieceType.Queen, 1)),
                new Move(8, 1, Direction.DownLeftUpRight, new Piece(Color.White, PieceType.Knight, 1)),
                new Move(14, 5, Direction.DownRightUpLeft, new Piece(Color.White, PieceType.Queen, 5)),
                new Move(14, 5, Direction.DownRightUpLeft, new Piece(Color.White, PieceType.Knight, 5)),
                new Move(14, 6, Direction.Vertical, new Piece(Color.White, PieceType.Queen, 6)),
                new Move(14, 6, Direction.Vertical, new Piece(Color.White, PieceType.Knight, 6)),
                new Move(14, 7, Direction.DownLeftUpRight, new Piece(Color.White, PieceType.Queen, 7)),
                new Move(14, 7, Direction.DownLeftUpRight, new Piece(Color.White, PieceType.Knight, 7))
            };

            foreach (Move testMove in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                   m.FromSquare == testMove.FromSquare
                   && m.ToSquare == testMove.ToSquare
                   && m.Direction == testMove.Direction
                   && m.PromotionPiece.Equals(testMove.PromotionPiece)
                   && m.KingFromSquare == testMove.KingFromSquare
                   && m.KingToSquare == testMove.KingToSquare
                   && m.RookFromSquare == testMove.RookFromSquare
                   && m.RookToSquare == testMove.RookToSquare));                
            }
        }

        [TestMethod]
        public void PawnMovement_ShouldContainSixPawnMovesIncludingEnPassant()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/6p1/3k1P1P/8/4pP2/NnP3p1/PP6/5K2 b - f3 0 42");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            List<Move> pawnTestMoves = new List<Move>()
            {
                new Move(36, 45, Direction.EnPassant),
                new Move(36, 44, Direction.Vertical),
                new Move(46, 54, Direction.Vertical),
                new Move(14, 21, Direction.DownLeftUpRight),
                new Move(14, 22, Direction.Vertical),
                new Move(14, 23, Direction.DownRightUpLeft)
            };

            foreach (Move move in pawnTestMoves)
            {
                Assert.IsTrue(availableMoves.Any(m =>
                    m.FromSquare == move.FromSquare
                    && m.ToSquare == move.ToSquare
                    && m.Direction == move.Direction));
            }
        }

        #endregion

        #region Pins

        [TestMethod]
        public void Pins_ShouldIncludeTheCorrectMovesOfPinnedPieces()
        {
            Chessboard position = Utils.LoadPositionFromFenString("k7/1b2r3/8/7p/2b3q1/2pPNBp1/r2QKP1r/3N3r w - - 3 41");
            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
                        
            // Illegal moves
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 43
                && m.ToSquare == 35));
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 43
                && m.ToSquare == 34));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 43
                && m.ToSquare == 36));
            Assert.IsTrue(!availableMoves.Any(m => m.FromSquare == 44));  
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 45
                && m.ToSquare == 54));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 45
                && m.ToSquare == 63));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 45
                && m.ToSquare == 18));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 45
                && m.ToSquare == 9));
            Assert.IsTrue(!availableMoves.Any(m => m.FromSquare == 53));
            Assert.IsTrue(!availableMoves.Any(m => m.FromSquare == 52));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 51
                && m.ToSquare == 42));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 51
                && m.ToSquare == 60));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 51
                && m.ToSquare == 58));   
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 44));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 53));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 65));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 74));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 69));
            Assert.IsTrue(!availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 76));

            // Legal moves        
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 42));
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 59
                && m.ToSquare == 49));
            Assert.IsTrue(availableMoves.Any(m =>
               m.FromSquare == 51
               && m.ToSquare == 50));
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 51
                && m.ToSquare == 49));
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 51
                && m.ToSquare == 48)); 
            Assert.IsTrue(availableMoves.Any(m =>
                m.FromSquare == 45
                && m.ToSquare == 38));
        }

        [TestMethod]
        public void Pins_ShouldCorrectlyListAllAvailableMovesUnderCloseCheck()
        {
            Chessboard position = Utils.LoadPositionFromFenString("1rbq1bnr/pp1kpppp/2p5/P1PP4/Q7/4P3/5PPP/RNB1KBNR w KQ - 0 15");

            position.MakeMove(new Move(32, 18, Direction.DownLeftUpRight));

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.IsTrue(availableMoves.Count == 1);
            Assert.IsTrue(availableMoves.Any(m => 
                m.FromSquare == 9 && 
                m.ToSquare == 18 && 
                m.Direction == Direction.DownRightUpLeft));
        }

        [TestMethod]
        public void Pins_ShouldCorrectlyListAllAvailableMovesAndCaptures()
        {
            Chessboard position = Utils.LoadPositionFromFenString("q2k1r2/b7/6n1/3B4/4K3/r7/8/8 w - - 15 48");

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.IsTrue(availableMoves.Count == 3);
            Assert.IsTrue(availableMoves.All(m =>
                m.FromSquare == 27 && m.Direction == Direction.DownRightUpLeft));
        }

        [TestMethod]
        public void Pins_ShouldCorrectlyListAllAvailableMovesWhilePinned()
        {
            Chessboard position = Utils.LoadPositionFromFenString("q2k1r2/b7/6n1/3B4/4K3/r7/8/8 w - - 15 48");

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);

            Assert.IsTrue(availableMoves.Count == 3);
            Assert.IsTrue(availableMoves.All(m =>
                m.FromSquare == 27 && m.Direction == Direction.DownRightUpLeft));
        }

        [TestMethod]
        public void Pins_ShouldCorrectlyIdentifyThatThereAreNoPins()
        {
            Chessboard position = Utils.LoadPositionFromFenString("8/4k3/5pp1/p1p2n2/1rb2PB1/B7/6PP/4K1NR b K - 8 81");

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            
            Assert.IsTrue(availableMoves.Any(m => m.FromSquare == 33));
        }

        #endregion

        #region In Check

        [TestMethod]
        public void InCheck_ShouldCorrectlyListAllLegalMovesWhileInCheck()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/5q1n/8/2Q2r2/1NPP1R2/3p1K2/8/8 b - - 15 58");

            position.MakeMove(new Move(13, 9, Direction.Horizontal));

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            HashSet<int> kingIllegalSquares = new HashSet<int>() { 36, 52, 54, 63 };
            HashSet<int> kingLegalSquares = new HashSet<int>() { 38, 44, 46, 53 };

            //Assert.IsTrue(availableMoves.Count == 8);
            Assert.IsTrue(availableMoves.Any(
                m => m.FromSquare == 26 && m.ToSquare == 18));
            Assert.IsTrue(availableMoves.Any(
                m => m.FromSquare == 26 && m.ToSquare == 27));
            Assert.IsTrue(availableMoves.Any(
                m => m.FromSquare == 33 && m.ToSquare == 18));
            Assert.IsTrue(availableMoves.Any(
                m => m.FromSquare == 33 && m.ToSquare == 27));
            Assert.IsTrue(availableMoves.Any(
                m => m.FromSquare == 35 && m.ToSquare == 27));

            foreach (var illegalSquare in kingIllegalSquares)
            {
                Assert.IsTrue(availableMoves
                    .Where(m => m.Direction == Direction.SingleSquare)
                    .All(m => m.FromSquare == 45 && m.ToSquare != illegalSquare));
            }

            foreach (var legalSquare in kingLegalSquares)
            {
                Assert.IsTrue(availableMoves
                    .Where(m => m.Direction == Direction.SingleSquare)
                    .Any(m => m.FromSquare == 45 && m.ToSquare == legalSquare));
            }
        }

        [TestMethod]
        public void InCheck_ShouldCorrectlyListAllLegalMovesWhileInCheckByKnight()
        {
            Chessboard position = Utils.LoadPositionFromFenString("3k4/5q1n/P7/2Q1r3/2PN1P2/3p1K2/8/8 b - - 15 58");

            position.MakeMove(new Move(15, 30, Direction.L));

            List<Move> availableMoves = MoveGenerator.GetAvailableMoves(position);
            HashSet<int> kingIllegalSquares = new HashSet<int>() { 36, 37, 44, 52, 60 };
            HashSet<int> kingLegalSquares = new HashSet<int>() { 38, 46, 53, 54 };

            foreach (var illegalSquare in kingIllegalSquares)
            {
                Assert.IsTrue(availableMoves
                    .Where(m => m.Direction == Direction.SingleSquare)
                    .All(m => m.FromSquare == 45 && m.ToSquare != illegalSquare));
            }

            foreach (var legalSquare in kingLegalSquares)
            {
                Assert.IsTrue(availableMoves
                    .Where(m => m.Direction == Direction.SingleSquare)
                    .Any(m => m.FromSquare == 45 && m.ToSquare == legalSquare));
            }
        }

        #endregion
    }
}