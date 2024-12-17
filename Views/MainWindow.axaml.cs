using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RayTracingApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly Canvas _canvas;
        private readonly List<Shape> _shapes = new List<Shape>();
        private Shape? _draggedShape;
        private Point _dragStart;
        private TextBlock? _messageTextBlock;

        private static readonly Random _random = new Random();
        //  Генерация случайных чисел

        public MainWindow()
        {
            InitializeComponent();
            _canvas = this.FindControl<Canvas>("DrawingCanvas");

            // Установка обработчиков событий для взаимодействия с холстом
            _canvas.PointerPressed += OnPointerPressed; // Обработчик нажатия указателя
            _canvas.PointerMoved += OnPointerMoved; // Обработчик перемещения указателя
            _canvas.PointerReleased += OnPointerReleased; // Обработчик отпускания указателя

            _canvas.Loaded += OnCanvasLoaded; // Обработчик события загрузки холста
        }

        private void OnCanvasLoaded(object? sender, RoutedEventArgs e) // Удаление обработчика события, чтобы он не вызывался повторно
        {
            
            _canvas.Loaded -= OnCanvasLoaded;
            
            AddRandomShape(CreateSquare()); // Добавление квадрата
            AddRandomShape(CreatePentagon()); // Добавление пятиугольника
            AddRandomShape(CreateOctagon()); // Добавление восьмиугольника
            AddRandomShape(CreateTriangle()); // Добавление треугольника
            AddRandomShape(CreateCircle()); // Добавление круга
            AddRandomShape(CreateDiamond()); // Добавление ромба
        }

        private void AddRandomShape(Shape shape)
        {
            // Генерация случайной позиции для фигуры
            var randomPosition = GenerateRandomPosition(shape);
            Canvas.SetLeft(shape, randomPosition.X); // Установка позиции по оси X
            Canvas.SetTop(shape, randomPosition.Y); // Установка позиции по оси Y
            AddShape(shape); // Добавление фигуры на холст
        }


        private Rectangle CreateSquare() // Создание квадрата с заданными размерами и цветом
        {
            
            return new Rectangle
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.DeepPink
            };
        }

        private Polygon CreateDiamond() // Создание ромба с заданными размерами и цветом
        {
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(40, 0), 
            new Point(80, 40),  
            new Point(40, 80),  
            new Point(0, 40)    
        },
                Fill = Brushes.MediumPurple
            };
        }


        private Ellipse CreateCircle() // Создание круга с заданными размерами и цветом
        {
            return new Ellipse
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.DeepSkyBlue 
            };
        }

        private Polygon CreateTriangle()  // Создание треугольника с заданными размерами и цветом
        {
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
            {
                new Point(40, 0),   
                new Point(80, 80),  
                new Point(0, 80)    
             },
                Fill = Brushes.Red
            };
        }


        private Polygon CreatePentagon()  // Создание пятиугольника с заданными размерами и цветом
        {
            
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 30),
                    new Point(65, 80),
                    new Point(15, 80),
                    new Point(0, 30)
                },
                Fill = Brushes.Orange
            };
        }


        private Polygon CreateOctagon()  // Создание восьмиугольника с заданными размерами и цветом
        {
            
            return new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(30, 0),
                    new Point(50, 0),
                    new Point(80, 30),
                    new Point(80, 50),
                    new Point(50, 80),
                    new Point(30, 80),
                    new Point(0, 50),
                    new Point(0, 30)
                },
                Fill = Brushes.Black
            };
        }

        private void AddShape(Shape shape)
        {
            _shapes.Add(shape); // Добавление фигуры в список фигур
            _canvas.Children.Add(shape); // Добавление фигуры на холст
        }

        private void ClearShapes()
        // Очистка всех фигур с холста и из списка фигур
        {

            _shapes.Clear(); // Очистка списка фигур
            _canvas.Children.Clear(); // Очистка холста от фигур
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetPosition(_canvas); // Получаем позицию указателя относительно холста
            _draggedShape = null; // Сбрасываем переменную, отвечающую за перетаскиваемую фигуру

            // Проверяем, на какую фигуру нажал пользователь
            foreach (var shape in _shapes)
            {
                if (shape.Bounds.Contains(point))
                {
                    _draggedShape = shape; // Запоминаем фигуру, которую перетаскиваем
                    _dragStart = point; // Сохраняем начальную позицию для перемещения
                    break; // Выходим из цикла, так как фигура найдена
                }
            }

            if (_draggedShape == null)
            {
                ShowMessage("Пусто"); // Если фигура не найдена, выводим сообщение
            }
            else
            {

                // Определение типа фигуры для вывода сообщения
                string shapeName = _draggedShape is Rectangle ? "Квадрат" :
                                  _draggedShape is Ellipse ? "Круг" :
                                  _draggedShape is Polygon polygon ? (polygon.Points.Count == 5 ? "Пятиугольник" :
                                   polygon.Points.Count == 4 ? "Ромб" :
                                   polygon.Points.Count == 3 ? "Треугольник" : "Восьмиугольник"
                                   ) :
                                   "";

                ShowMessage($"{shapeName}"); // Вывод названия фигуры
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_draggedShape != null)
            {
                // Перемещаем фигуру вслед за указателем мыши
                var currentPoint = e.GetPosition(_canvas);
                var delta = currentPoint - _dragStart;

                var newLeft = Canvas.GetLeft(_draggedShape) + delta.X;
                var newTop = Canvas.GetTop(_draggedShape) + delta.Y;

                // Проверка границ холста, чтобы фигура не выходила за его пределы
                if (newLeft < 0) newLeft = 0;
                if (newTop < 0) newTop = 0;
                if (newLeft + _draggedShape.Bounds.Width > _canvas.Bounds.Width) newLeft = _canvas.Bounds.Width - _draggedShape.Bounds.Width;
                if (newTop + _draggedShape.Bounds.Height > _canvas.Bounds.Height) newTop = _canvas.Bounds.Height - _draggedShape.Bounds.Height;

                Canvas.SetLeft(_draggedShape, newLeft);
                Canvas.SetTop(_draggedShape, newTop);

                _dragStart = currentPoint;
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _draggedShape = null;
        }

        private async void ShowMessage(string message) // Проверяем, существует ли уже текстовый блок для сообщения
        {
            
            if (_messageTextBlock != null)
            {
                _canvas.Children.Remove(_messageTextBlock);
                _messageTextBlock = null;
            }

            
            _messageTextBlock = new TextBlock
            {
                Text = message,
                FontSize = 40,
                Foreground = Brushes.Black,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0)
            };

            // Выравниваем по центру наше сообщение
            Canvas.SetLeft(_messageTextBlock, (_canvas.Bounds.Width - _messageTextBlock.Bounds.Width) / 2);
            Canvas.SetTop(_messageTextBlock, 0);

            _canvas.Children.Add(_messageTextBlock);

            // Удаляем сообщение через 3 сек
            await Task.Delay(3000);
            if (_messageTextBlock != null)
            {
                _canvas.Children.Remove(_messageTextBlock);
                _messageTextBlock = null;
            }
        }

        private Point GenerateRandomPosition(Shape shape)
        {
            // Случайная генерации фигуры и контроль выхода за пределы границ
            double x = _random.NextDouble() * (_canvas.Bounds.Width - shape.Bounds.Width);
            double y = _random.NextDouble() * (_canvas.Bounds.Height - shape.Bounds.Height);
            return new Point(x, y);
        }

        
        private void OnDrawSquareClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый квадрат с заданными размерами и цветом
            var square = new Rectangle
            {
                Width = 80,
                Height = 80,
                Fill = Brushes.DeepPink
            };

            // Устанавливаем положение квадрата по центру холста
            Canvas.SetLeft(square, (DrawingCanvas.Bounds.Width - square.Width) / 2);
            Canvas.SetTop(square, (DrawingCanvas.Bounds.Height - square.Height) / 2);

            AddShape(square); // Добавляем квадрат на холст
        }

        private void OnDrawCircleClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый круг
            var circle = new Ellipse
            {
                Width = 80,  // Ширина круга
                Height = 80, // Высота круга
                Fill = Brushes.DeepSkyBlue // Цвет заливки
            };

            // Получаем размеры холста
            double canvasWidth = DrawingCanvas.Bounds.Width;  // Ширина холста
            double canvasHeight = DrawingCanvas.Bounds.Height; // Высота холста

            // Устанавливаем позицию круга на холсте (по центру)
            Canvas.SetLeft(circle, (canvasWidth - circle.Width) / 2); // Позиция X
            Canvas.SetTop(circle, (canvasHeight - circle.Height) / 2); // Позиция Y

            AddShape(circle); // Добавляем круг на холст
        }


        private void OnDrawPentagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый пятиугольник с заданными вершинами и цветом
            var pentagon = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
                {
                    new Point(40, 0),
                    new Point(80, 30),
                    new Point(65, 80),
                    new Point(15, 80),
                    new Point(0, 30)
                },
                Fill = Brushes.Orange
            };

            Canvas.SetLeft(pentagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(pentagon, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(pentagon); // Добавляем пятиугольник на холст
        }

        private void OnDrawTriangleClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый треугольник с заданными вершинами и цветом
            var triangle = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(40, 0),   
            new Point(80, 80),  
            new Point(0, 80)   
        },
                Fill = Brushes.Red
            };

            Canvas.SetLeft(triangle, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(triangle, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(triangle); // Добавляем треугольник на холст
        }

        private void OnDrawDiamondClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый ромб с заданными вершинами и цветом
            var diamond = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(40, 0),   // Верхняя вершина
            new Point(80, 40),  // Правая вершина
            new Point(40, 80),  // Нижняя вершина
            new Point(0, 40)    // Левая вершина
        },
                Fill = Brushes.MediumPurple // Цвет заливки
            };

            // Устанавливаем позицию ромба на холсте (по центру)
            Canvas.SetLeft(diamond, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(diamond, (DrawingCanvas.Bounds.Height - 80) / 2);

            AddShape(diamond); // Добавляем ромб на холст
        }

        private void OnDrawOctagonClick(object? sender, RoutedEventArgs e)
        {
            ClearShapes();

            // Создаем новый восьмиугольник с заданными вершинами и цветом
            var octagon = new Polygon
            {
                Points = new Avalonia.Collections.AvaloniaList<Point>
        {
            new Point(30, 0),
                    new Point(50, 0),
                    new Point(80, 30),
                    new Point(80, 50),
                    new Point(50, 80),
                    new Point(30, 80),
                    new Point(0, 50),
                    new Point(0, 30)
        },
                Fill = Brushes.Black // Цвет заливки
            };

            // Устанавливаем позицию восьмиугольника на холсте (по центру)
            Canvas.SetLeft(octagon, (DrawingCanvas.Bounds.Width - 80) / 2);
            Canvas.SetTop(octagon, (DrawingCanvas.Bounds.Height - 120) / 2);

            AddShape(octagon); // Добавляем восьмиугольник на холст
        }
    }
}