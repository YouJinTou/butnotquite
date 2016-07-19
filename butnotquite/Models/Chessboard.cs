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
        internal IDictionary<long, Piece[]> MoveSquares;

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
        internal int EnPassantSquare;

        internal Chessboard(bool fromFen)
        {
            if (!fromFen)
            {
                this.InitializeStartingPosition();
            }

            this.SideToMove = Color.White;
            this.OppositeColor = Color.Black;

            this.WhiteCanCastle = true;
            this.BlackCanCastle = true;

            this.OpponentActivity = new Dictionary<Piece, HashSet<int>>(30);
            this.MoveSquares = new Dictionary<long, Piece[]>();
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
            if (this.MakeCastle(move))
            {
                return;
            }

            Piece movingPiece = Utils.MakeDeepCopy(this.Board[move.FromSquare].OccupiedBy);

            Piece[] fromToPieces = new Piece[]
            {
                Utils.MakeDeepCopy(this.Board[move.FromSquare].OccupiedBy),
                Utils.MakeDeepCopy(this.Board[move.ToSquare].OccupiedBy)
            };
            this.MoveSquares.Add(move.Id, fromToPieces);

            bool isPawn = (movingPiece.Type == PieceType.Pawn);
            bool isCapture = (this.Board[move.ToSquare].OccupiedBy.Type != PieceType.None);
            this.FiftyMoveCounter = (isPawn || isCapture) ? 0 : (this.SideToMove == Color.Black) ?
                (this.FiftyMoveCounter + 1) : this.FiftyMoveCounter;
            movingPiece.Position = move.ToSquare;

            this.ResetSquare(move.FromSquare);
            this.Board[move.ToSquare].OccupiedBy = (move.PromotionPiece.Position == -1) ? movingPiece : move.PromotionPiece;

            if (move.Direction == Direction.EnPassant)
            {
                int squareAboveEnPassantSquare = (this.SideToMove == Color.White) ? move.ToSquare - 8 : move.ToSquare + 8;

                this.ResetSquare(squareAboveEnPassantSquare);
            }

            this.SwapSides();
            this.UpdateGameStateInfo(move, "make");
        }

        internal void UndoMove(Move move)
        {
            if (this.UndoCastle(move))
            {
                return;
            }

            this.SwapSides();

            Piece movingPiece = (move.PromotionPiece.Position == -1) ?
                this.Board[move.ToSquare].OccupiedBy :
                new Piece(this.SideToMove, PieceType.Pawn, move.FromSquare);
            movingPiece.Position = move.FromSquare;

            this.Board[move.ToSquare].OccupiedBy = this.MoveSquares[move.Id][1];
            this.Board[move.FromSquare].OccupiedBy = movingPiece;

            if (move.Direction == Direction.EnPassant)
            {
                int squareAboveEnPassantSquare = (this.SideToMove == Color.White) ? move.ToSquare - 8 : move.ToSquare + 8;

                this.Board[squareAboveEnPassantSquare].OccupiedBy = new Piece(this.SideToMove, PieceType.Pawn, squareAboveEnPassantSquare);
            }

            this.MoveSquares.Remove(move.Id);

            this.UpdateGameStateInfo(move, "undo");
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

        private bool MakeCastle(Move move)
        {
            if (move.Direction != Direction.Castle)
            {
                return false;
            }

            Piece king = Utils.MakeDeepCopy(this.Board[move.KingFromSquare].OccupiedBy);
            Piece rook = Utils.MakeDeepCopy(this.Board[move.RookFromSquare].OccupiedBy);

            this.ResetSquare(move.KingFromSquare);
            this.Board[move.KingToSquare].OccupiedBy = king;
            this.ResetSquare(move.RookFromSquare);
            this.Board[move.RookToSquare].OccupiedBy = rook;

            king.Position = move.KingToSquare;
            rook.Position = move.RookToSquare;

            this.UpdateGameStateInfo(move, "make");
            this.SwapSides();

            return true;
        }

        private bool UndoCastle(Move move)
        {
            if (move.Direction != Direction.Castle)
            {
                return false;
            }

            this.SwapSides();

            Piece king = Utils.MakeDeepCopy(this.Board[move.KingToSquare].OccupiedBy);
            Piece rook = Utils.MakeDeepCopy(this.Board[move.RookToSquare].OccupiedBy);

            this.ResetSquare(move.KingToSquare);
            this.Board[move.KingFromSquare].OccupiedBy = king;
            this.ResetSquare(move.RookToSquare);
            this.Board[move.RookFromSquare].OccupiedBy = rook;

            king.Position = move.KingFromSquare;
            rook.Position = move.RookFromSquare;

            this.UpdateGameStateInfo(move, "undo");

            return true;
        }

        private void UpdateGameStateInfo(Move move, string moveType)
        {
            this.LastMove = move;

            this.SetEnPassantSquare(moveType);

            if (this.SideToMove == Color.Black)
            {
                int increment = (moveType == "make") ? 1 : -1;
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

        private void SetEnPassantSquare(string moveType)
        {
            Move move = this.LastMove;

            if (moveType == "make" && move.Direction == Direction.Vertical)
            {
                if (this.Board[move.ToSquare].OccupiedBy.Type == PieceType.Pawn)
                {
                    int moveDifference = Math.Abs(move.FromSquare - move.ToSquare);

                    if (moveDifference == 16)
                    {
                        this.EnPassantSquare = (this.SideToMove == Color.White) ? move.FromSquare + 8 : move.FromSquare - 8;
                    }
                }
            }
            else if (moveType == "unmake" && move.Direction == Direction.EnPassant)
            {
                this.EnPassantSquare = move.ToSquare;
            }
            else
            {
                this.EnPassantSquare = -1;
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