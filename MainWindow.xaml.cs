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
using System.Windows.Threading;

namespace Snakes
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int SnakeSquareSize = 25;

        private readonly SolidColorBrush _snakeColor = Brushes.Green;
        
        private enum Direction
        {
            Left, Right, Top, Bottom
        }

        private Direction _direction = Direction.Right;
        private const int TimerInterval = 300; // начальная скорость
        private DispatcherTimer _timer; // таймер для движения
        private int _currentInterval = TimerInterval;

        private const int SpeedBoost = 5000; //длительность ускорения 5 секунд

        private Rectangle _snakeHead;
        private Point _applePosition;
        private Point _venomPosition;
        private Point _starPosition;

        private static readonly Random random = new Random();

        private List<Rectangle> _snake = new List<Rectangle>();

        private List<Image> appleImageList = new List<Image>();

        private int _score = 0;
        


        public MainWindow()
        {
            InitializeComponent();
            GameCanvas.Loaded += GameCanvas_Loaded;
        }

        private void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _snakeHead = CreateSnakeSegment(new Point(5, 5));
            _snake.Add(_snakeHead);
            GameCanvas.Children.Add(_snakeHead);

            PlaceApple();
            PlaceVenom();
            PlaceStar();

            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            _timer.Interval = TimeSpan.FromMilliseconds(_currentInterval);
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Point newHeadPosition = CalcuteNewHeadPosition();
            
            if(newHeadPosition == _applePosition)
            {
                EatApple();
                PlaceApple();
                
            }

            if(newHeadPosition == _venomPosition)
            {
                EatVenom();
                PlaceVenom();
            }

            if (newHeadPosition == _starPosition)
            {
                EatStar();
                PlaceStar();
            }

            if (newHeadPosition.X < 0 || newHeadPosition.Y < 0
                || newHeadPosition.X >= GameCanvas.ActualWidth / SnakeSquareSize
                || newHeadPosition.Y >= GameCanvas.ActualHeight / SnakeSquareSize)
            {
                EndGame();
                return;
            }

            if(_snake.Count >= 4)
            {
                for(int i = 0; i < _snake.Count; i ++)
                {
                    Point currentPos = new Point(Canvas.GetLeft(_snake[i]), Canvas.GetTop(_snake[i]));

                    for(int j = i + 1; j < _snake.Count; j ++)
                    {
                        Point nextPos = new Point(Canvas.GetLeft(_snake[j]), Canvas.GetTop(_snake[j]));

                        if (currentPos == nextPos)
                        {
                            EndGame();
                            return;
                        }
                    }
                }
            }

            for(int i = _snake.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(_snake[i], Canvas.GetLeft(_snake[i - 1])); ///расположение слева
                Canvas.SetTop(_snake[i], Canvas.GetTop(_snake[i - 1])); /// расположение сверху
            }

            Canvas.SetLeft(_snakeHead, newHeadPosition.X * SnakeSquareSize); ///расположение слева
            Canvas.SetTop(_snakeHead, newHeadPosition.Y * SnakeSquareSize); /// расположение сверху
        }

        private void EndGame()
        {
            _timer.Stop();
        }

        private void EatApple()
        {
            _score++;
            ScoreTextBlock.Text = "Score: " + _score.ToString();
            foreach(var element in GameCanvas.Children.OfType<Image>().ToList())
            {
                if(element.Tag?.ToString() == "Apple")
                {
                    GameCanvas.Children.Remove(element);
                }
            }

            Rectangle newSnake = CreateSnakeSegment(_applePosition);
            _snake.Add(newSnake);
            GameCanvas.Children.Add(newSnake);
        }

        private void EatVenom()
        {
            _score = Math.Max(0, _score - 10);
            ScoreTextBlock.Text = "Score: " + _score.ToString();
            foreach (var element in GameCanvas.Children.OfType<Image>().ToList())
            {
                if (element.Tag?.ToString() == "Venom")
                {
                    GameCanvas.Children.Remove(element);
                }
            }
        }

        private void EatStar()
        {
            foreach (var element in GameCanvas.Children.OfType<Image>().ToList())
            {
                if (element.Tag?.ToString() == "Star")
                {
                    GameCanvas.Children.Remove(element);
                }
            }

            ApplySpeedBoost(); // Активируем ускорение
            StartBoostTimer(); // Запускаем таймер отката
        }

        private void ApplySpeedBoost()
        {
            _currentInterval = (int)(TimerInterval * 0.6); // уменьшаем интервал на 40%
            _timer.Interval = TimeSpan.FromMilliseconds(_currentInterval);
        }

        private void StartBoostTimer()
        {
            var boostTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(SpeedBoost)
            };

            boostTimer.Tick += (s, e) =>
            {
                _currentInterval = TimerInterval;
                _timer.Interval = TimeSpan.FromMilliseconds(_currentInterval);
                _timer.Start();
                ((DispatcherTimer)s).Stop();
            };

            boostTimer.Start();
        }

        private Point CalcuteNewHeadPosition()
        {
            double left = Canvas.GetLeft(_snakeHead) / SnakeSquareSize;
            double top = Canvas.GetTop(_snakeHead) / SnakeSquareSize;

            Point headCurretPosition = new Point(left, top);
            Point newHeadPosition = new Point();

            switch(_direction)
            {
                case Direction.Left:
                    newHeadPosition = new Point(headCurretPosition.X - 1, headCurretPosition.Y);
                    break;
                case Direction.Right:
                    newHeadPosition = new Point(headCurretPosition.X + 1, headCurretPosition.Y);
                    break;
                case Direction.Top:
                    newHeadPosition = new Point(headCurretPosition.X, headCurretPosition.Y - 1);
                    break;
                case Direction.Bottom:
                    newHeadPosition = new Point(headCurretPosition.X, headCurretPosition.Y + 1);
                    break;
            }

            return newHeadPosition;
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
                Source = new BitmapImage(new Uri("pack://application:,,,/img/яблоко.png"))
            };

            appleImage.Tag = "Apple";
            appleImageList.Add(appleImage);

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
                Source = new BitmapImage(new Uri("pack://application:,,,/img/яд.jpg.png"))
            };

            venomImage.Tag = "Venom";

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
                Source = new BitmapImage(new Uri("pack://application:,,,/img/звезда.png"))
            };

            starImage.Tag = "Star";

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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    if(_direction != Direction.Bottom)
                        _direction = Direction.Top;
                    break;
                case Key.Down:
                    if (_direction != Direction.Top)
                        _direction = Direction.Bottom;
                    break;
                case Key.Left:
                    if (_direction != Direction.Right)
                        _direction = Direction.Left;
                    break;
                case Key.Right:
                    if (_direction != Direction.Left)
                        _direction = Direction.Right;
                    break;
            }
        }
    }
}
