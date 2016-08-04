namespace Client
{
    using butnotquite.Models;
    using butnotquite.Defaults;
    using ViewModel;

    using System.Collections.ObjectModel;
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.InitializeGame();        
        }

        private void InitializeGame()
        {
            Chessboard initialChessboard = new Chessboard(false);
            ObservableCollection<PieceViewModel> pieces = new ObservableCollection<PieceViewModel>();

            for (int square = 0; square < initialChessboard.Board.Length; square++)
            {
                int x = square % 8;
                int y = square / 8;
                Point position = new Point(x, y);
                PieceViewModel pieceModel = new PieceViewModel()
                {
                    PieceType = initialChessboard.Board[square].OccupiedBy.Type,
                    Color = initialChessboard.Board[square].OccupiedBy.Color,
                    Position = position
                };

                if (pieceModel.PieceType == PieceType.None)
                {
                    continue;
                }

                pieces.Add(pieceModel);
            }

            this.ChessboardUI.ItemsSource = pieces;
        }
    }
}
