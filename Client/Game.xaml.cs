namespace Client
{
    using butnotquite.Models;
    using butnotquite.Defaults;
    using ViewModel;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window
    {
        private Chessboard chessboard;
        private ObservableCollection<PieceViewModel> pieces;
        private IDictionary<int, int[]> colBorders; 
        private IDictionary<int, int[]> rowBorders; 
        private bool squareSelected;
        private int fromSquare;
        private int toSquare;

        public MainWindow()
        {
            InitializeComponent();
            this.BindPieces();
            this.InitializeSquareBorders();
        }

        private void BindPieces()
        {
            this.chessboard = new Chessboard(false);
            this.pieces = new ObservableCollection<PieceViewModel>();
            
            for (int square = 0; square < this.chessboard.Board.Length; square++)
            {
                int x = square % 8;
                int y = square / 8;
                Point position = new Point(x, y);
                PieceViewModel pieceModel = new PieceViewModel()
                {
                    PieceType = this.chessboard.Board[square].OccupiedBy.Type,
                    Color = this.chessboard.Board[square].OccupiedBy.Color,
                    Position = position
                };

                this.pieces.Add(pieceModel);
            }

            this.ChessboardUI.ItemsSource = this.pieces;
        }

        private void InitializeSquareBorders()
        {
            this.colBorders = new Dictionary<int, int[]>();
            this.rowBorders = new Dictionary<int, int[]>();
            int rowFrom = 0;
            int rowTo = 70;
            int colFrom = 10;
            int colTo = 80;

            for (int row = 0; row < 8; row++)
            {
                if (!this.rowBorders.ContainsKey(row))
                {
                    this.rowBorders.Add(row, new int[] { rowFrom, rowTo });
                }

                rowFrom = rowTo;
                rowTo += 70;

                for (int col = 0; col < 8; col++)
                {
                    if (!this.colBorders.ContainsKey(col))
                    {
                        this.colBorders.Add(col, new int[] { colFrom, colTo });
                    }

                    colFrom = colTo;
                    colTo += 70;
                }
            }
        }

        private void emptySquare_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int s = 5;
        }

        private void square_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            int row = this.rowBorders.FirstOrDefault(kvp => kvp.Value[0] <= position.Y && kvp.Value[1] >= position.Y).Key;
            int col = this.colBorders.FirstOrDefault(kvp => kvp.Value[0] <= position.X && kvp.Value[1] >= position.X).Key;
            int squareIndex = (row * 8) + col;
            
            if (!this.squareSelected)
            {
                if (this.chessboard.Board[squareIndex].OccupiedBy.Type == PieceType.None)
                {
                    return;
                }

                this.fromSquare = squareIndex;
                this.squareSelected = true;
            }
            else
            {
                if (squareIndex == this.fromSquare)
                {
                    this.squareSelected = false;

                    return;
                }

                this.toSquare = squareIndex;
                Move newMove = new Move(fromSquare, toSquare, Direction.SingleSquare);
                this.squareSelected = false;

                this.chessboard.MakeMove(newMove);
                this.UpdateGameState();
            }
        }

        private void UpdateGameState()
        {
            int fromRow = this.fromSquare / 8;
            int toRow = this.toSquare / 8;
            int fromCol = this.fromSquare % 8;
            int toCol = this.toSquare % 8;
            Point fromPosition = new Point(fromCol, fromRow);
            Point toPosition = new Point(toCol, toRow);
            PieceViewModel movingPiece = this.pieces.FirstOrDefault(piece => 
                piece.Position.X == fromPosition.X && 
                piece.Position.Y == fromPosition.Y);

            movingPiece.Position = toPosition;
        }
    }
}