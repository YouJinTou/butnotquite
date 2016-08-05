namespace butnotquite.Core
{
    using butnotquite.Defaults;
    using butnotquite.Models;

    internal static class Evaluator
    {
        #region Piece Tables

        private static readonly short[] PawnTable = new short[]
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
            5,  5, 10, 27, 27, 10,  5,  5,
            0,  0,  0, 25, 25,  0,  0,  0,
            5, -5,-10,  0,  0,-10, -5,  5,
            5, 10, 10,-25,-25, 10, 10,  5,
            0,  0,  0,  0,  0,  0,  0,  0
        };
        private static readonly short[] KnightTable = new short[]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-20,-30,-30,-20,-40,-50,
        };
        private static readonly short[] BishopTable = new short[]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-30,-10,-10,-40,-10,-20,
        };
        private static readonly short[] KingTable = new short[]
        {
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -10, -20, -20, -20, -20, -20, -20, -10,
            20,  20,   0,   0,   0,   0,  20,  20,
            20,  30,  10,   0,   0,  10,  30,  20
        };
        private static readonly short[] KingTableEndGame = new short[]
        {
            -50,-40,-30,-20,-20,-30,-40,-50,
            -30,-20,-10,  0,  0,-10,-20,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-30,  0,  0,  0,  0,-30,-30,
            -50,-30,-30,-30,-30,-30,-30,-50
        };

        #endregion

        private static Chessboard position;

        internal static int EvaluatePosition(Chessboard currentPosition)
        {
            position = currentPosition;
            int evaluation = 0;
            evaluation += EvaluateMaterial();
            evaluation += EvaluateActivity();

            return evaluation;
        }

        #region Evaluation Methods

        private static int EvaluateMaterial()
        {
            int sideToMoveScore = 0;
            int oppositeSideScore = 0;

            for (int square = 0; square < position.Board.Length; square++)
            {
                Piece currentPiece = position.Board[square].OccupiedBy;

                if (currentPiece.Type == PieceType.None)
                {
                    continue;
                }

                if (currentPiece.Color == Color.White)
                {
                    sideToMoveScore += currentPiece.Value;
                }
                else
                {
                    oppositeSideScore -= currentPiece.Value;
                }
            }

            return (sideToMoveScore + oppositeSideScore);
        }

        private static int EvaluateActivity()
        {
            int sideToMoveScore = position.PieceActivity;
            int oppositeSideScore = -position.PieceActivity;

            for (int square = 0; square < position.Board.Length; square++)
            {
                Piece currentPiece = position.Board[square].OccupiedBy;

                if (currentPiece.Type == PieceType.None)
                {
                    continue;
                }

                if (currentPiece.Color == Color.White)
                {
                    sideToMoveScore += GetTableValue(currentPiece, square);
                }
                else
                {
                    oppositeSideScore -= GetTableValue(currentPiece, square);
                }
            }

            return (sideToMoveScore + oppositeSideScore);
        }

        #endregion

        #region Helpers

        private static int GetTableValue(Piece piece, int square)
        {
            if (piece.Color == Color.Black)
            {
                square = 63 - square;
            }

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return PawnTable[square];
                case PieceType.Knight:
                    return KnightTable[square];
                case PieceType.Bishop:
                    return BishopTable[square];
                case PieceType.King:
                    return KingTable[square];
                default:
                    return 0;
            }
        }

        #endregion
    }
}
