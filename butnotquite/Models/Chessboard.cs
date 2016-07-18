namespace butnotquite.Models
{
    using Defaults;
    using Utils;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class Chessboard
    {
        internal Square[] Board;
        internal int MoveCounter;
        internal Color SideToMove;
        internal Color OppositeColor;
        internal int Evaluation;
        internal Move MaximizingSideBestMove;

        internal IDictionary<Piece, HashSet<int>> OpponentActivity;
        internal IDictionary<long, Piece> DestinationSquares;

        internal bool WhiteInCheck;
        internal bool BlackInCheck;
        internal bool KingInCheck;
        internal bool WhiteCanCastle;
        internal bool BlackCanCastle;

        internal int RepetitionCounter;
        internal bool RepetitionEnforcable;
        internal int FiftyMoveCounter;
        internal bool FiftyMoveEnforcable;

        internal Move LastMove;
        internal int EnPassantSquare; // To be implemented

        internal Chessboard(bool fromFen)
        {
            if (!fromFen)
            {
                this.InitializeStartingPosition();
            }

            this.SideToMove = Color.White;
            this.OppositeColor = Color.Black;

            this.OpponentActivity = new Dictionary<Piece, HashSet<int>>(30);
            this.DestinationSquares = new Dictionary<long, Piece>();
        }

        internal int WhiteKingPosition
        {
            get
            {
                return this.Board
                .FirstOrDefault(s => s.OccupiedBy.Type == PieceType.King && s.OccupiedBy.Color == Color.White)
                .OccupiedBy
                .Position;
            }
        }
        internal int BlackKingPosition
        {
            get
            {
                return this.Board
                .FirstOrDefault(s => s.OccupiedBy.Type == PieceType.King && s.OccupiedBy.Color == Color.Black)
                .OccupiedBy
                .Position;
            }
        }


        internal void MakeMove(Move move)
        {
            Piece movingPiece = Utils.MakeDeepCopy(this.Board[move.FromSquare].OccupiedBy);

            this.DestinationSquares.Add(move.Id, Utils.MakeDeepCopy(this.Board[move.ToSquare].OccupiedBy));

            bool isPawn = (movingPiece.Type == PieceType.Pawn);
            bool isCapture = (this.Board[move.ToSquare].OccupiedBy.Type != PieceType.None);
            this.FiftyMoveCounter = (isPawn || isCapture) ? 0 : (this.SideToMove == Color.Black) ?
                (this.FiftyMoveCounter + 1) : this.FiftyMoveCounter;
            movingPiece.Position = move.ToSquare;

            this.UpdateGameStateInfo(move, movingPiece, "make");

            this.ResetSquare(move.FromSquare);
            this.Board[move.ToSquare].OccupiedBy = movingPiece;

            this.SwapSides();
        }

        internal void UndoMove(Move move)
        {
            Piece movingPiece = Utils.MakeDeepCopy(this.Board[move.ToSquare].OccupiedBy);

            movingPiece.Position = move.FromSquare;

            this.UpdateGameStateInfo(move, movingPiece, "undo");

            this.Board[move.ToSquare].OccupiedBy = this.DestinationSquares[move.Id];
            this.Board[move.FromSquare].OccupiedBy = movingPiece;

            this.DestinationSquares.Remove(move.Id);

            this.SwapSides();
        }

        private void UpdateGameStateInfo(Move move, Piece movingPiece, string moveType)
        {
            int increment = (moveType == "make") ? 1 : -1;   
            this.LastMove = move;

            if (this.SideToMove == Color.Black)
            {
                this.MoveCounter = this.MoveCounter + increment;
            }
            
            if (this.WhiteInCheck)
            {
                this.WhiteInCheck = false;
            }
            else if (this.BlackInCheck)
            {
                this.BlackInCheck = false;
            }

            this.KingInCheck = (this.WhiteInCheck ^ this.BlackInCheck);
        }

        #region Board Initialization

        private void InitializeStartingPosition()
        {
            Chessboard tempPosition = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            this.Board = tempPosition.Board;
        }

        #endregion

        #region Utils

        internal void SwapSides()
        {
            this.SideToMove = (this.SideToMove == Color.White) ? Color.Black : Color.White;
            this.OppositeColor = (this.SideToMove == Color.White) ? Color.Black : Color.White;
        }

        internal void PrintBoard()
        {
            Console.Clear();

            for (int square = 0; square < this.Board.Length; square++)
            {
                int col = square % 8;

                if (col == 0)
                {
                    Console.WriteLine();
                }

                if (this.LastMove.FromSquare == square || this.LastMove.ToSquare == square)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.Write(this.GetLetterFromPiece(this.Board[square].OccupiedBy).ToString().PadRight(3));
            }
        }

        

        private char GetLetterFromPiece(Piece piece)
        {
            char letter = '#';

            switch (piece.Type)
            {
                case PieceType.King:
                    letter = 'k';
                    break;
                case PieceType.Queen:
                    letter = 'q';
                    break;
                case PieceType.Rook:
                    letter = 'r';
                    break;
                case PieceType.Bishop:
                    letter = 'b';
                    break;
                case PieceType.Knight:
                    letter = 'n';
                    break;
                case PieceType.Pawn:
                    letter = 'p';
                    break;
                default:
                    return '#';
            }

            return (piece.Color == Color.White) ? char.ToUpper(letter) : letter;
        }

        private void ResetSquare(int squareNumber)
        {
            Square square = this.Board[squareNumber];

            square.OccupiedBy.Type = PieceType.None;
            square.OccupiedBy.Color = Color.None;
            square.OccupiedBy.Value = 0;
            square.OccupiedBy.Position = -1;
            square.OccupiedBy.Moves = null;
        }

        #endregion
    }
}