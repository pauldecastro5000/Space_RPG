using Space_RPG.Models;
using Space_RPG.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace Space_RPG.Windows
{
    public partial class ShipEditorWindow : Window
    {
        private const double TileSize = 14;

        private double _zoom = 1.0;

        private ShipRoomDesign _draggingRoom;
        private bool _isRoomDragging;
        private Point _roomDragStartMouseWorldPoint;
        private int _roomDragStartWorldX;
        private int _roomDragStartWorldY;

        private ShipRoomDesign _resizingRoom;
        private bool _isRoomResizing;
        private Point _resizeStartMouseWorldPoint;
        private int _resizeStartWidth;
        private int _resizeStartHeight;

        private const int MinimumRoomWidth = 2;
        private const int MinimumRoomHeight = 2;
        private const double ResizeHandleSize = 10;

        private bool _isPreviewDragging;
        private bool _hasDragged;
        private Point _previewDragStartPoint;
        private double _previewStartOffsetX;
        private double _previewStartOffsetY;

        public bool IsSaved { get; set; }

        public ShipEditorWindow(string filePath)
        {
            InitializeComponent();
            IsSaved = false;
            ShipEditorViewModel vm = new ShipEditorViewModel(filePath);
            vm.RequestClose += () =>
            {
                DialogResult = true;
                Close();
            };

            DataContext = vm;

            Loaded += (s, e) => DrawShipPreview();
        }

        private void DrawShipPreview()
        {
            ShipEditorViewModel vm = DataContext as ShipEditorViewModel;

            if (vm == null || vm.Rooms == null)
                return;

            PreviewCanvas.Children.Clear();
            GridCanvas.Children.Clear();

            DrawGrid();

            foreach (ShipRoomDesign room in vm.Rooms)
            {
                DrawRoom(room);

                foreach (ShipObjectDesign obj in room.Objects)
                    DrawObject(room, obj);
            }
        }

        private void DrawGrid()
        {
            double gridSize = TileSize;

            for (double x = 0; x < GridCanvas.Width; x += gridSize)
            {
                Line line = new Line
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = GridCanvas.Height,
                    Stroke = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    StrokeThickness = 1
                };

                GridCanvas.Children.Add(line);
            }

            for (double y = 0; y < GridCanvas.Height; y += gridSize)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = GridCanvas.Width,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                    StrokeThickness = 1
                };

                GridCanvas.Children.Add(line);
            }
        }

        private void DrawRoom(ShipRoomDesign room)
        {
            double x = room.WorldX * TileSize;
            double y = room.WorldY * TileSize;
            double width = room.Width * TileSize;
            double height = room.Height * TileSize;

            Rectangle rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = GetRoomBrush(room.RoomType),
                Stroke = Brushes.White,
                StrokeThickness = 1,
                Tag = room,
                Cursor = Cursors.SizeAll
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            PreviewCanvas.Children.Add(rect);

            TextBlock label = new TextBlock
            {
                Text = room.Name,
                Foreground = Brushes.White,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(label, x + 3);
            Canvas.SetTop(label, y + 3);
            PreviewCanvas.Children.Add(label);

            Rectangle resizeHandle = new Rectangle
            {
                Width = ResizeHandleSize,
                Height = ResizeHandleSize,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(resizeHandle, x + width - ResizeHandleSize);
            Canvas.SetTop(resizeHandle, y + height - ResizeHandleSize);
            PreviewCanvas.Children.Add(resizeHandle);
        }

        private void PreviewHost_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double zoomStep = 0.1;
            const double minZoom = 0.3;
            const double maxZoom = 4.0;

            if (e.Delta > 0)
                _zoom += zoomStep;
            else
                _zoom -= zoomStep;

            if (_zoom < minZoom)
                _zoom = minZoom;

            if (_zoom > maxZoom)
                _zoom = maxZoom;

            PreviewScaleTransform.ScaleX = _zoom;
            PreviewScaleTransform.ScaleY = _zoom;
        }
        private void DrawObject(ShipRoomDesign room, ShipObjectDesign obj)
        {
            double x = (room.WorldX + obj.X) * TileSize;
            double y = (room.WorldY + obj.Y) * TileSize;
            double width = obj.Width * TileSize;
            double height = obj.Height * TileSize;

            Rectangle rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = GetObjectBrush(obj.TileType),
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            PreviewCanvas.Children.Add(rect);
        }

        private Brush GetRoomBrush(FacilityType roomType)
        {
            switch (roomType)
            {
                case FacilityType.MainDeck:
                    return Brushes.DarkSlateBlue;

                case FacilityType.Cafeteria:
                    return Brushes.DarkOliveGreen;

                case FacilityType.MedicalBay:
                    return Brushes.DarkCyan;

                case FacilityType.Cargo:
                    return Brushes.SaddleBrown;

                case FacilityType.Corridor:
                    return Brushes.DimGray;

                default:
                    return Brushes.DarkSlateGray;
            }
        }

        private Brush GetObjectBrush(InteriorTileType tileType)
        {
            switch (tileType)
            {
                case InteriorTileType.Cockpit:
                    return (Brush)new BrushConverter().ConvertFromString("#287FC7");

                case InteriorTileType.WeaponsConsole:
                    return (Brush)new BrushConverter().ConvertFromString("#A31459");

                case InteriorTileType.MedicalConsole:
                    return (Brush)new BrushConverter().ConvertFromString("#2271BF");

                case InteriorTileType.Bed:
                    return Brushes.SteelBlue;

                case InteriorTileType.Table:
                    return Brushes.Peru;

                case InteriorTileType.StorageBox:
                    return Brushes.SaddleBrown;

                case InteriorTileType.Workbench:
                    return Brushes.DarkOrange;

                case InteriorTileType.KitchenCounter:
                    return Brushes.DarkRed;

                case InteriorTileType.Sofa:
                    return Brushes.MediumPurple;

                default:
                    return Brushes.White;
            }
        }

        private void PreviewHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _hasDragged = false;

            Point mouseCanvasPoint = e.GetPosition(PreviewCanvas);
            ShipRoomDesign resizeRoom = GetRoomResizeHandleAtPoint(mouseCanvasPoint);

            if (resizeRoom != null)
            {
                _isRoomResizing = true;
                _resizingRoom = resizeRoom;

                _resizeStartMouseWorldPoint = new Point(
                    mouseCanvasPoint.X / TileSize,
                    mouseCanvasPoint.Y / TileSize);

                _resizeStartWidth = resizeRoom.Width;
                _resizeStartHeight = resizeRoom.Height;

                PreviewHost.CaptureMouse();
                return;
            }

            ShipRoomDesign clickedRoom = GetRoomAtPoint(mouseCanvasPoint);

            if (clickedRoom != null)
            {
                _isRoomDragging = true;
                _draggingRoom = clickedRoom;

                _roomDragStartMouseWorldPoint = new Point(
                    mouseCanvasPoint.X / TileSize,
                    mouseCanvasPoint.Y / TileSize);

                _roomDragStartWorldX = clickedRoom.WorldX;
                _roomDragStartWorldY = clickedRoom.WorldY;

                PreviewHost.CaptureMouse();
                return;
            }

            _isPreviewDragging = true;

            _previewDragStartPoint = e.GetPosition(this);
            _previewStartOffsetX = PreviewTranslateTransform.X;
            _previewStartOffsetY = PreviewTranslateTransform.Y;

            PreviewHost.CaptureMouse();
        }

        private void PreviewHost_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseCanvasPoint = e.GetPosition(PreviewCanvas);

            if (!_isRoomDragging && !_isRoomResizing && !_isPreviewDragging)
            {
                PreviewHost.Cursor = GetRoomResizeHandleAtPoint(mouseCanvasPoint) != null
                    ? Cursors.SizeNWSE
                    : Cursors.Arrow;
            }

            if (_isRoomResizing && _resizingRoom != null)
            {
                double currentWorldX = mouseCanvasPoint.X / TileSize;
                double currentWorldY = mouseCanvasPoint.Y / TileSize;

                int deltaWidth = (int)Math.Round(currentWorldX - _resizeStartMouseWorldPoint.X);
                int deltaHeight = (int)Math.Round(currentWorldY - _resizeStartMouseWorldPoint.Y);

                if (deltaWidth != 0 || deltaHeight != 0)
                    _hasDragged = true;

                _resizingRoom.Width = Math.Max(GetMinimumWidthForRoom(_resizingRoom), _resizeStartWidth + deltaWidth);
                _resizingRoom.Height = Math.Max(GetMinimumHeightForRoom(_resizingRoom), _resizeStartHeight + deltaHeight);

                DrawShipPreview();
                return;
            }

            if (_isRoomDragging && _draggingRoom != null)
            {
                double currentWorldX = mouseCanvasPoint.X / TileSize;
                double currentWorldY = mouseCanvasPoint.Y / TileSize;

                int deltaX = (int)Math.Round(currentWorldX - _roomDragStartMouseWorldPoint.X);
                int deltaY = (int)Math.Round(currentWorldY - _roomDragStartMouseWorldPoint.Y);

                if (deltaX != 0 || deltaY != 0)
                    _hasDragged = true;

                _draggingRoom.WorldX = _roomDragStartWorldX + deltaX;
                _draggingRoom.WorldY = _roomDragStartWorldY + deltaY;

                DrawShipPreview();
                return;
            }

            if (!_isPreviewDragging)
                return;

            Point currentPoint = e.GetPosition(this);

            double previewDeltaX = currentPoint.X - _previewDragStartPoint.X;
            double previewDeltaY = currentPoint.Y - _previewDragStartPoint.Y;

            if (Math.Abs(previewDeltaX) > 3 || Math.Abs(previewDeltaY) > 3)
                _hasDragged = true;

            PreviewTranslateTransform.X = _previewStartOffsetX + previewDeltaX;
            PreviewTranslateTransform.Y = _previewStartOffsetY + previewDeltaY;
        }

        private void PreviewHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool wasRoomDragging = _isRoomDragging;
            bool wasRoomResizing = _isRoomResizing;

            _isPreviewDragging = false;
            _isRoomDragging = false;
            _isRoomResizing = false;
            _draggingRoom = null;
            _resizingRoom = null;
            PreviewHost.Cursor = Cursors.Arrow;

            PreviewHost.ReleaseMouseCapture();

            if (_hasDragged || wasRoomDragging || wasRoomResizing)
                return;

            Point mousePoint = e.GetPosition(PreviewCanvas);

            ShipRoomDesign clickedRoom = GetRoomAtPoint(mousePoint);

            if (clickedRoom == null)
                return;

            OpenRoomEditor(clickedRoom);
        }

        private ShipRoomDesign GetRoomResizeHandleAtPoint(Point mousePoint)
        {
            ShipEditorViewModel vm = DataContext as ShipEditorViewModel;

            if (vm == null || vm.Rooms == null)
                return null;

            for (int i = vm.Rooms.Count - 1; i >= 0; i--)
            {
                ShipRoomDesign room = vm.Rooms[i];

                double handleLeft = (room.WorldX + room.Width) * TileSize - ResizeHandleSize;
                double handleTop = (room.WorldY + room.Height) * TileSize - ResizeHandleSize;
                double handleRight = handleLeft + ResizeHandleSize;
                double handleBottom = handleTop + ResizeHandleSize;

                bool isInsideHandle =
                    mousePoint.X >= handleLeft &&
                    mousePoint.X <= handleRight &&
                    mousePoint.Y >= handleTop &&
                    mousePoint.Y <= handleBottom;

                if (isInsideHandle)
                    return room;
            }

            return null;
        }

        private int GetMinimumWidthForRoom(ShipRoomDesign room)
        {
            int minimumWidth = MinimumRoomWidth;

            if (room.Objects != null)
            {
                foreach (ShipObjectDesign obj in room.Objects)
                    minimumWidth = Math.Max(minimumWidth, obj.X + obj.Width);
            }

            return minimumWidth;
        }

        private int GetMinimumHeightForRoom(ShipRoomDesign room)
        {
            int minimumHeight = MinimumRoomHeight;

            if (room.Objects != null)
            {
                foreach (ShipObjectDesign obj in room.Objects)
                    minimumHeight = Math.Max(minimumHeight, obj.Y + obj.Height);
            }

            return minimumHeight;
        }

        private ShipRoomDesign GetRoomAtPoint(Point mousePoint)
        {
            ShipEditorViewModel vm = DataContext as ShipEditorViewModel;

            if (vm == null || vm.Rooms == null)
                return null;

            double worldX = mousePoint.X / TileSize;
            double worldY = mousePoint.Y / TileSize;

            // Reverse loop so the topmost/latest drawn room gets selected first
            for (int i = vm.Rooms.Count - 1; i >= 0; i--)
            {
                ShipRoomDesign room = vm.Rooms[i];

                bool isInside =
                    worldX >= room.WorldX &&
                    worldX < room.WorldX + room.Width &&
                    worldY >= room.WorldY &&
                    worldY < room.WorldY + room.Height;

                if (isInside)
                    return room;
            }

            return null;
        }

        private void OpenRoomEditor(ShipRoomDesign room)
        {
            RoomEditorWindow window = new RoomEditorWindow(room)
            {
                Owner = this
            };

            bool? result = window.ShowDialog();

            if (result == true)
                DrawShipPreview();
        }
    }
}