namespace butnotquite.Core.Zobrist
{
    using Defaults;
    using Models;

    using System.Collections.Generic;

    internal static class ZobristHasher
    {
        private static readonly Dictionary<KeyValuePair<Color, PieceType>, int> PieceCodes =
            new Dictionary<KeyValuePair<Color, PieceType>, int>
        {
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.Pawn), 0},
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.Pawn), 1},
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.Knight), 2 },
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.Knight), 3 },
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.Bishop), 4 },
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.Bishop), 5 },
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.Rook), 6},
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.Rook), 7},
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.Queen), 8},
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.Queen), 9},
            { new KeyValuePair<Color, PieceType>(Color.White, PieceType.King), 10},
            { new KeyValuePair<Color, PieceType>(Color.Black, PieceType.King), 11}
        };

        private static long[] randomValues;
        private static ZobristRandomizer randomizer;

        static ZobristHasher()
        {
            randomValues = new long[781]; // (12 x 64) + 8 (enPassant) + 4 (castling) + 1 (side to move)
            randomizer = new ZobristRandomizer();

            for (int i = 0; i < randomValues.Length; i++)
            {
                randomValues[i] = randomizer.NextLong();
            }
        }

        internal static long GetZobristHash(Chessboard position)
        {
            long hash = 0;

            for (int square = 0; square < position.Board.Length; square++)
            {
                Piece piece = position.Board[square].OccupiedBy;

                if (piece.Type == PieceType.None)
                {
                    continue;
                }

                int valueIndex = piece.Position * 12 + PieceCodes[new KeyValuePair<Color, PieceType>(piece.Color, piece.Type)];

                hash ^= randomValues[valueIndex];              
            }

            int enPassantCol = (position.EnPassantSquare > -1) ? (position.EnPassantSquare % 8) : -1;

            if (enPassantCol > -1)
            {
                int enPassantIndex = 768 + enPassantCol;

                hash ^= randomValues[enPassantIndex];
            }

            if (position.WhiteCanCastle)
            {
                if (!position.Board[63].InitialPieceLeft) // Short
                {
                    hash ^= randomValues[776];
                }

                if (!position.Board[56].InitialPieceLeft) // Long
                {
                    hash ^= randomValues[777];
                }
            }

            if (position.BlackCanCastle)
            {
                if (!position.Board[7].InitialPieceLeft) // Short
                {
                    hash ^= randomValues[778];
                }

                if (!position.Board[0].InitialPieceLeft) // Long
                {
                    hash ^= randomValues[779];
                }
            }

            if (position.SideToMove == Color.Black)
            {
                hash ^= randomValues[780];
            }

            return hash;
        }
    }
}