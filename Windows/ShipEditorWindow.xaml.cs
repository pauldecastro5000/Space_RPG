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

        private ShipRoomDesign _selectedObjectRoom;
        private ShipObjectDesign _selectedPlacedObject;

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

            Loaded += (s, e) =>
            {
                LoadObjectPalette();
                DrawShipPreview();
            };
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

            bool isSelected = ReferenceEquals(obj, _selectedPlacedObject);

            Rectangle rect = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = GetObjectBrush(obj.TileType),
                Stroke = isSelected ? Brushes.Yellow : Brushes.Black,
                StrokeThickness = isSelected ? 3 : 1,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            PreviewCanvas.Children.Add(rect);

            TextBlock label = new TextBlock
            {
                Text = obj.TileType.ToString(),
                Foreground = Brushes.White,
                FontSize = 9,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(label, x + 2);
            Canvas.SetTop(label, y + 2);
            PreviewCanvas.Children.Add(label);
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

        private void LoadObjectPalette()
        {
            ObjectTypeComboBox.ItemsSource = Enum.GetValues(typeof(InteriorObjectType));
            TileTypeComboBox.ItemsSource = Enum.GetValues(typeof(InteriorTileType));

            ObjectTypeComboBox.SelectedItem = InteriorObjectType.Table;
            TileTypeComboBox.SelectedItem = InteriorTileType.Table;
        }

        private bool IsObjectPlacementMode()
        {
            return PlaceObjectToggleButton != null && PlaceObjectToggleButton.IsChecked == true;
        }

        private int GetObjectWidthFromInput()
        {
            int value;
            if (!int.TryParse(ObjectWidthTextBox.Text, out value))
                value = 1;

            return Math.Max(1, value);
        }

        private int GetObjectHeightFromInput()
        {
            int value;
            if (!int.TryParse(ObjectHeightTextBox.Text, out value))
                value = 1;

            return Math.Max(1, value);
        }

        private void RotateObjectSize_Click(object sender, RoutedEventArgs e)
        {
            string widthText = ObjectWidthTextBox.Text;
            ObjectWidthTextBox.Text = GetObjectHeightFromInput().ToString();
            ObjectHeightTextBox.Text = GetObjectWidthFromInputFromText(widthText).ToString();
        }

        private int GetObjectWidthFromInputFromText(string text)
        {
            int value;
            if (!int.TryParse(text, out value))
                value = 1;

            return Math.Max(1, value);
        }

        private void RotateSelectedObject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlacedObject == null || _selectedObjectRoom == null)
                return;

            int newWidth = _selectedPlacedObject.Height;
            int newHeight = _selectedPlacedObject.Width;

            if (!ObjectCanFit(_selectedObjectRoom, _selectedPlacedObject, _selectedPlacedObject.X, _selectedPlacedObject.Y, newWidth, newHeight))
            {
                MessageBox.Show("The selected object cannot be rotated there because it will overlap another object or go outside the room.", "Ship Editor");
                return;
            }

            _selectedPlacedObject.Width = newWidth;
            _selectedPlacedObject.Height = newHeight;

            DrawShipPreview();
            UpdateSelectedObjectText();
        }

        private void DeleteSelectedObject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPlacedObject == null || _selectedObjectRoom == null)
                return;

            _selectedObjectRoom.Objects.Remove(_selectedPlacedObject);
            _selectedPlacedObject = null;
            _selectedObjectRoom = null;

            DrawShipPreview();
            UpdateSelectedObjectText();
        }

        private void PlaceObjectAt(Point mouseCanvasPoint)
        {
            ShipEditorViewModel vm = DataContext as ShipEditorViewModel;

            if (vm == null || vm.Rooms == null)
                return;

            ShipRoomDesign room = GetRoomAtPoint(mouseCanvasPoint);

            if (room == null)
                return;

            int roomTileX = (int)Math.Floor(mouseCanvasPoint.X / TileSize) - room.WorldX;
            int roomTileY = (int)Math.Floor(mouseCanvasPoint.Y / TileSize) - room.WorldY;
            int objectWidth = GetObjectWidthFromInput();
            int objectHeight = GetObjectHeightFromInput();

            if (!ObjectCanFit(room, null, roomTileX, roomTileY, objectWidth, objectHeight))
            {
                MessageBox.Show("The object cannot be placed there because it will overlap another object or go outside the room.", "Ship Editor");
                return;
            }

            InteriorObjectType objectType = InteriorObjectType.None;
            InteriorTileType tileType = InteriorTileType.Table;

            if (ObjectTypeComboBox.SelectedItem is InteriorObjectType)
                objectType = (InteriorObjectType)ObjectTypeComboBox.SelectedItem;

            if (TileTypeComboBox.SelectedItem is InteriorTileType)
                tileType = (InteriorTileType)TileTypeComboBox.SelectedItem;

            ShipObjectDesign obj = new ShipObjectDesign
            {
                Name = tileType.ToString(),
                ObjectType = objectType,
                TileType = tileType,
                X = roomTileX,
                Y = roomTileY,
                Width = objectWidth,
                Height = objectHeight,
                AllowMultipleCrew = false
            };

            obj.InteractionDirections.Add(InteractionDirection.Bottom);
            room.Objects.Add(obj);

            _selectedObjectRoom = room;
            _selectedPlacedObject = obj;

            DrawShipPreview();
            UpdateSelectedObjectText();
        }

        private bool ObjectCanFit(ShipRoomDesign room, ShipObjectDesign objectToIgnore, int objectX, int objectY, int objectWidth, int objectHeight)
        {
            if (objectX < 0 || objectY < 0)
                return false;

            if (objectX + objectWidth > room.Width || objectY + objectHeight > room.Height)
                return false;

            foreach (ShipObjectDesign existingObject in room.Objects)
            {
                if (ReferenceEquals(existingObject, objectToIgnore))
                    continue;

                bool overlaps =
                    objectX < existingObject.X + existingObject.Width &&
                    objectX + objectWidth > existingObject.X &&
                    objectY < existingObject.Y + existingObject.Height &&
                    objectY + objectHeight > existingObject.Y;

                if (overlaps)
                    return false;
            }

            return true;
        }

        private ShipObjectDesign GetObjectAtPoint(Point mousePoint, out ShipRoomDesign objectRoom)
        {
            ShipEditorViewModel vm = DataContext as ShipEditorViewModel;
            objectRoom = null;

            if (vm == null || vm.Rooms == null)
                return null;

            double worldX = mousePoint.X / TileSize;
            double worldY = mousePoint.Y / TileSize;

            for (int roomIndex = vm.Rooms.Count - 1; roomIndex >= 0; roomIndex--)
            {
                ShipRoomDesign room = vm.Rooms[roomIndex];

                for (int objectIndex = room.Objects.Count - 1; objectIndex >= 0; objectIndex--)
                {
                    ShipObjectDesign obj = room.Objects[objectIndex];

                    bool isInside =
                        worldX >= room.WorldX + obj.X &&
                        worldX < room.WorldX + obj.X + obj.Width &&
                        worldY >= room.WorldY + obj.Y &&
                        worldY < room.WorldY + obj.Y + obj.Height;

                    if (isInside)
                    {
                        objectRoom = room;
                        return obj;
                    }
                }
            }

            return null;
        }

        private void UpdateSelectedObjectText()
        {
            if (SelectedObjectTextBlock == null)
                return;

            if (_selectedPlacedObject == null || _selectedObjectRoom == null)
            {
                SelectedObjectTextBlock.Text = "None";
                return;
            }

            SelectedObjectTextBlock.Text =
                _selectedPlacedObject.TileType +
                "\nRoom: " + _selectedObjectRoom.Name +
                "\nX: " + _selectedPlacedObject.X +
                ", Y: " + _selectedPlacedObject.Y +
                "\nSize: " + _selectedPlacedObject.Width +
                " x " + _selectedPlacedObject.Height;
        }

        private void PreviewHost_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _hasDragged = false;

            Point mouseCanvasPoint = e.GetPosition(PreviewCanvas);

            if (IsObjectPlacementMode())
            {
                PlaceObjectAt(mouseCanvasPoint);
                PreviewHost.CaptureMouse();
                return;
            }

            ShipRoomDesign objectRoom;
            ShipObjectDesign clickedObject = GetObjectAtPoint(mouseCanvasPoint, out objectRoom);

            if (clickedObject != null)
            {
                _selectedObjectRoom = objectRoom;
                _selectedPlacedObject = clickedObject;
                DrawShipPreview();
                UpdateSelectedObjectText();
                return;
            }

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

            if (_hasDragged || wasRoomDragging || wasRoomResizing || IsObjectPlacementMode())
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