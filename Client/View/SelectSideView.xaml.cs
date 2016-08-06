namespace Client.View
{
    using Engine.Defaults;

    using System.Windows;
    using System.Windows.Controls;

    public partial class SelectSideView : Window
    {
        public SelectSideView()
        {
            InitializeComponent();
        }
        
        private void side_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Color playerColor = (button.Content.ToString() == "White") ? Color.White : Color.Black;
            int depth = 4;

            foreach (var child in this.mainContainer.Children)
            {
                RadioButton childAsRadio = child as RadioButton;

                if (childAsRadio != null && (bool)childAsRadio.IsChecked)
                {
                    int difficulty = int.Parse(childAsRadio.Content as string);

                    depth = (difficulty == 1) ? 4 : 5;
                }
            }

            GameView gameView = new GameView(playerColor, depth);

            this.Close();
            gameView.Show();
        }
    }
}
