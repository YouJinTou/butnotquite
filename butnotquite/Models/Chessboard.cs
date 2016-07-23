namespace butnotquite.Models
{
    using Core.Search.Zobrist;
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
        internal IDictionary<long, int> FiftyMoveMarkers;

        internal Stack<long> GameHistory;
        internal IDictionary<long, int> TranspositionTable;

        internal bool WhiteInCheck;
        internal bool BlackInCheck;
        internal bool KingInCheck;
        internal bool WhiteCanCastle;
        internal bool BlackCanCastle;

        internal int RepetitionCounter;
        internal int FiftyMoveCounter;

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
            this.EnPassantSquare = -1;
            this.GameHistory = new Stack<long>(200);
            this.TranspositionTable = new Dictionary<long, int>();
            this.OpponentActivity = new Dictionary<Piece, HashSet<int>>(30);
            this.MoveSquares = new Dictionary<long, Piece[]>();
            this.FiftyMoveMarkers = new Dictionary<long, int>();
        }

        #region Properties

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

        #endregion

        #region Board Initialization

        private void InitializeStartingPosition()
        {
            Chessboard tempPosition = Utils.LoadPositionFromFenString("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

            this.Board = tempPosition.Board;
        }

        #endregion

        #region Movement

        internal void MakeMove(Move move)
        {
            if (this.MakeCastle(move))
            {
                this.GameHistory.Push(ZobristHasher.GetZobristHash(this));

                return;
            }

            Piece movingPiece = Utils.MakeDeepCopy(this.Board[move.FromSquare].OccupiedBy);
            Piece[] fromToPieces = new Piece[]
            {
                Utils.MakeDeepCopy(this.Board[move.FromSquare].OccupiedBy),
                Utils.MakeDeepCopy(this.Board[move.ToSquare].OccupiedBy)
            };

            this.MoveSquares.Add(move.Id, fromToPieces);

            this.ResetSquare(move.FromSquare);
            this.Board[move.ToSquare].OccupiedBy = (move.PromotionPiece.Position == -1) ? movingPiece : move.PromotionPiece;
            movingPiece.Position = move.ToSquare;

            if (move.Direction == Direction.EnPassant)
            {
                int pawnSquareToReset = (this.SideToMove == Color.White) ? move.ToSquare + 8 : move.ToSquare - 8;

                this.ResetSquare(pawnSquareToReset);
            }

            this.UpdateGameStateInfo(move, "make", movingPiece);
            this.SwapSides();
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

            this.Board[move.KingFromSquare].InitialPieceLeft = true;
            this.Board[move.RookFromSquare].InitialPieceLeft = true;

            king.Position = move.KingToSquare;
            rook.Position = move.RookToSquare;

            if (this.SideToMove == Color.White)
            {
                this.WhiteCanCastle = false;
            }
            else
            {
                this.BlackCanCastle = false;
            }

            this.UpdateGameStateInfo(move, "make");
            this.SwapSides();

            return true;
        }

        internal void UndoMove(Move move)
        {
            if (this.UndoCastle(move))
            {
                this.GameHistory.Pop();

                return;
            }

            this.SwapSides();

            Piece movingPiece = Utils.MakeDeepCopy(this.MoveSquares[move.Id][0]);
            //(move.PromotionPiece.Position == -1) ?
            //this.Board[move.ToSquare].OccupiedBy :
            //new Piece(this.SideToMove, PieceType.Pawn, move.FromSquare);

            this.Board[move.ToSquare].OccupiedBy = this.MoveSquares[move.Id][1];
            this.Board[move.FromSquare].OccupiedBy = movingPiece;
            movingPiece.Position = move.FromSquare;

            if (move.Direction == Direction.EnPassant)
            {
                int pawnSquareToRestore = (this.SideToMove == Color.White) ? move.ToSquare + 8 : move.ToSquare - 8;

                this.Board[pawnSquareToRestore].OccupiedBy = new Piece(this.OppositeColor, PieceType.Pawn, pawnSquareToRestore);
            }

            this.MoveSquares.Remove(move.Id);

            this.UpdateGameStateInfo(move, "undo", movingPiece);

            this.GameHistory.Pop();
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

            this.Board[move.KingFromSquare].InitialPieceLeft = false;
            this.Board[move.RookFromSquare].InitialPieceLeft = false;

            king.Position = move.KingFromSquare;
            rook.Position = move.RookFromSquare;

            if (this.SideToMove == Color.White)
            {
                this.WhiteCanCastle = true;
            }
            else
            {
                this.BlackCanCastle = true;
            }

            this.UpdateGameStateInfo(move, "undo");

            return true;
        }

        #endregion

        #region Utils

        private void UpdateGameStateInfo(Move move, string moveType, Piece movingPiece = null)
        {
            bool makeMove = (moveType == "make");
            this.LastMove = move;

            this.SetEnPassantSquare(moveType);

            if (makeMove)
            {
                this.UpdateGameStateInfoOnMakeMove(move, movingPiece);
            }
            else
            {
                this.UpdateGameStateInfoOnUndoMove(move, movingPiece);
            }

            if (movingPiece != null && move.Direction != Direction.Castle)
            {
                if (movingPiece.Type == PieceType.Rook &&
                    (move.FromSquare == 0 || move.FromSquare == 7 || move.FromSquare == 56 || move.FromSquare == 63))
                {
                    if (movingPiece.Color == this.SideToMove)
                    {
                        this.Board[move.FromSquare].InitialPieceLeft = makeMove;
                    }
                }
                else if (movingPiece.Type == PieceType.King &&
                    (move.FromSquare == 4 || move.FromSquare == 60))
                {
                    if (movingPiece.Color == this.SideToMove)
                    {
                        this.Board[move.FromSquare].InitialPieceLeft = makeMove;
                    }
                }
            }
        }

        private void SetEnPassantSquare(string moveType)
        {
            Move move = this.LastMove;

            if (moveType == "make" && move.Direction == Direction.Vertical)
            {
                if (this.Board[move.ToSquare].OccupiedBy.Type == PieceType.Pawn)
                {
                    int squareDifference = Math.Abs(move.FromSquare - move.ToSquare);

                    if (squareDifference == 16)
                    {
                        this.EnPassantSquare = (this.SideToMove == Color.White) ? move.FromSquare + 8 : move.FromSquare - 8;
                    }
                }
            }
            else if (moveType == "undo" && move.Direction == Direction.EnPassant)
            {
                this.EnPassantSquare = move.ToSquare;
            }
            else
            {
                this.EnPassantSquare = -1;
            }
        }

        private void UpdateGameStateInfoOnMakeMove(Move move, Piece movingPiece)
        {
            if (this.SideToMove == Color.Black)
            {
                this.MoveCounter++;
            }

            if (movingPiece != null)
            {
                this.FiftyMoveMarkers.Add(move.Id, this.FiftyMoveCounter);

                bool isPawn = (movingPiece.Type == PieceType.Pawn);
                bool isCapture = (this.MoveSquares[move.Id][1].Type != PieceType.None);
                this.FiftyMoveCounter = (isPawn || isCapture) ? 0 : (this.FiftyMoveCounter + 1);
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

        private void UpdateGameStateInfoOnUndoMove(Move move, Piece movingPiece)
        {
            if (this.SideToMove == Color.Black)
            {
                this.MoveCounter--;
            }

            if (movingPiece != null)
            {
                this.FiftyMoveCounter = this.FiftyMoveMarkers[move.Id];

                this.FiftyMoveMarkers.Remove(move.Id);
            }
        }

        private void ResetSquare(int squareNumber)
        {
            Square square = this.Board[squareNumber];

            square.OccupiedBy.Type = PieceType.None;
            square.OccupiedBy.Color = Color.None;
            square.OccupiedBy.Value = 0;
            square.OccupiedBy.Position = -1;
        }

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
            char letter = '_';

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
                    return '_';
            }

            return (piece.Color == Color.White) ? char.ToUpper(letter) : letter;
        }

        #endregion
    }
}