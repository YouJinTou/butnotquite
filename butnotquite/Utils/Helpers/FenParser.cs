namespace butnotquite.Utils.Helpers
{
    using Defaults;
    using Models;

    using System;
    using System.Linq;

    internal static class FenParser
    {
        internal static Chessboard LoadPositionFromFenString(string fenString)
        {
            string[] boardState = fenString.Split('/');

            if (boardState.Length != 8)
            {
                throw new ArgumentException();
            }

            string[] gameState = boardState[7].Split(' ');
            string[] position = new string[8];

            Array.Copy(boardState, position, 7);
            position[7] = gameState[0];
            gameState[0] = null;

            Chessboard chessPosition = new Chessboard(true);
            chessPosition.Board = new Square[64];
            int squareIndex = 0;

            for (int row = 0; row < position.Length; row++)
            {
                string currentRow = position[row];

                for (int col = 0; col < currentRow.Length; col++)
                {
                    char currentSquare = currentRow[col];

                    if (Char.IsDigit(currentSquare))
                    {
                        int toSquare = squareIndex + (int)Char.GetNumericValue(currentSquare);

                        for (int i = squareIndex; i < toSquare; i++) // Create empty squares
                        {
                            chessPosition.Board[i].OccupiedBy = new Piece();

                            squareIndex++;
                        }

                        continue;
                    }

                    chessPosition.Board[squareIndex].OccupiedBy = GetPieceByLetter(currentSquare, squareIndex);

                    squareIndex++;
                }
            }

            chessPosition.SideToMove = (gameState[1] == "w") ? Color.White : Color.Black;
            chessPosition.OppositeColor = (chessPosition.SideToMove == Color.White) ? Color.Black : Color.White;
            chessPosition.WhiteCanCastle = (gameState[2].IndexOfAny(new char[] { 'K', 'Q' }) > -1) ? true : false;
            chessPosition.Board[63].InitialPieceLeft = !gameState[2].Contains("K");
            chessPosition.Board[56].InitialPieceLeft = !gameState[2].Contains("Q");
            chessPosition.BlackCanCastle = (gameState[2].IndexOfAny(new char[] { 'k', 'q' }) > -1) ? true : false;
            chessPosition.Board[7].InitialPieceLeft = !gameState[2].Contains("k");
            chessPosition.Board[0].InitialPieceLeft = !gameState[2].Contains("q");
            chessPosition.FiftyMoveCounter = int.Parse(gameState[4]);
            chessPosition.MoveCounter = int.Parse(gameState[5]);
            chessPosition.EnPassantSquare = GetEnPassantSquare(gameState[3]);
            int sourceSquare = (chessPosition.SideToMove == Color.White) ? 
                (chessPosition.EnPassantSquare - 8) : 
                (chessPosition.EnPassantSquare + 8);
            int targetSquare = (chessPosition.SideToMove == Color.White) ?
                (chessPosition.EnPassantSquare + 8) :
                (chessPosition.EnPassantSquare - 8);
            chessPosition.LastMove = (chessPosition.EnPassantSquare > -1) ? new Move(sourceSquare, targetSquare, Direction.EnPassant) : new Move();   

            return chessPosition;
        }

        private static Piece GetPieceByLetter(char fenLetter, int position)
        {
            char fenLetterAsLower = Char.ToLower(fenLetter);

            switch (fenLetterAsLower)
            {
                case 'k':
                    return new Piece(GetPieceColor(fenLetter), PieceType.King, position);
                case 'q':
                    return new Piece(GetPieceColor(fenLetter), PieceType.Queen, position);
                case 'r':
                    return new Piece(GetPieceColor(fenLetter), PieceType.Rook, position);
                case 'b':
                    return new Piece(GetPieceColor(fenLetter), PieceType.Bishop, position);
                case 'n':
                    return new Piece(GetPieceColor(fenLetter), PieceType.Knight, position);
                case 'p':
                    return new Piece(GetPieceColor(fenLetter), PieceType.Pawn, position);
                default:
                    return new Piece();
            }
        }

        private static Color GetPieceColor(char fenLetter)
        {
            return (Char.IsUpper(fenLetter) ? Color.White : Color.Black);
        }

        private static int GetEnPassantSquare(string location)
        {
            if (location == "-")
            {
                return -1;
            }

            int colAsNumber;
            char file = location[0];
            int rank = (int)Char.GetNumericValue(location[1]);

            switch (file)
            {
                case 'a':
                    colAsNumber = 0;
                    break;
                case 'b':
                    colAsNumber = 1;
                    break;
                case 'c':
                    colAsNumber = 2;
                    break;
                case 'd':
                    colAsNumber = 3;
                    break;
                case 'e':
                    colAsNumber = 4;
                    break;
                case 'f':
                    colAsNumber = 5;
                    break;
                case 'g':
                    colAsNumber = 6;
                    break;
                case 'h':
                    colAsNumber = 7;
                    break;
                default:
                    return -1;
            }

            int enPassantSquare = (8 - rank) * 8 + colAsNumber;

            return enPassantSquare;
        }
    }}
