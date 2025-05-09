using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snakes
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 25;
        private int SnakeSpeedStart = 5;

        private readonly SolidColorBrush _snakeColor = Brushes.Green;
        private readonly SolidColorBrush _appleColor = Brushes.Red;
        private readonly SolidColorBrush _venomColor = Brushes.Blue;
        private readonly SolidColorBrush _starColor = Brushes.Yellow;


        private Rectangle _snakeHead;
        private Point _applePosition;
        private Point _venomPosition;
        private Point _starPosition;

        private static readonly Random random = new Random(); 


        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Loaded += GameCanvas_Loaded;
        }

        private void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _snakeHead = CreateSnakeSegment(new Point(5, 5));
            GameCanvas.Children.Add(_snakeHead);

            PlaceApple();
            PlaceVenom();
            PlaceStar();
        }

        private void PlaceApple()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            int appleX = random.Next(0, maxX);
            int appleY = random.Next(0, maxY);

            _applePosition = new Point(appleX, appleY);

            Image appleImage = new Image
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("D:\\User\\C\\Snakes\\Snakes\\img\\яблоко.png"))
            };

            Canvas.SetLeft(appleImage, appleX * SnakeSquareSize); ///расположение слева
            Canvas.SetTop(appleImage, appleY * SnakeSquareSize); /// расположение сверху

            GameCanvas.Children.Add(appleImage);
        }

        private void PlaceVenom()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            int venomX = random.Next(0, maxX);
            int venomY = random.Next(0, maxY);

            _venomPosition = new Point(venomX, venomY);

            Image venomImage = new Image
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("D:\\User\\C\\Snakes\\Snakes\\img\\яд.jpg.png"))
            };

            Canvas.SetLeft(venomImage, venomX * SnakeSquareSize); ///расположение слева
            Canvas.SetTop(venomImage, venomY * SnakeSquareSize); /// расположение сверху

            GameCanvas.Children.Add(venomImage);
        }

        private void PlaceStar()
        {
            int maxX = (int)(GameCanvas.ActualWidth / SnakeSquareSize);
            int maxY = (int)(GameCanvas.ActualHeight / SnakeSquareSize);

            int starX = random.Next(0, maxX);
            int starY = random.Next(0, maxY);

            _starPosition = new Point(starX, starY);

            Image starImage = new Image
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Source = new BitmapImage(new Uri("D:\\User\\C\\Snakes\\Snakes\\img\\звезда.png"))
            };

            Canvas.SetLeft(starImage, starX * SnakeSquareSize); ///расположение слева
            Canvas.SetTop(starImage, starY * SnakeSquareSize); /// расположение сверху

            GameCanvas.Children.Add(starImage);
        }

        private Rectangle CreateSnakeSegment(Point position)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = SnakeSquareSize,
                Height = SnakeSquareSize,
                Fill = _snakeColor
            };

            Canvas.SetLeft(rectangle, position.X * SnakeSquareSize); ///расположение слева
            Canvas.SetTop(rectangle, position.Y * SnakeSquareSize); /// расположение сверху

            return rectangle;
        }
    }
}
