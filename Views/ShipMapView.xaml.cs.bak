using Space_RPG.Helpers;
using Space_RPG.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Space_RPG.Views
{
    public partial class ShipMapView : UserControl
    {
        private const double TileSize = 24;
        private const double OffsetX = 40;
        private const double OffsetY = 40;

        private bool _isDragging;
        private Point _dragStartPoint;
        private double _startHorizontalOffset;
        private double _startVerticalOffset;

        public static readonly DependencyProperty ShipProperty =
            DependencyProperty.Register(
                nameof(Ship),
                typeof(Ship),
                typeof(ShipMapView),
                new PropertyMetadata(null, OnShipChanged));

        public Ship Ship
        {
            get { return (Ship)GetValue(ShipProperty); }
            set { SetValue(ShipProperty, value); }
        }

        public static readonly DependencyProperty CrewsProperty =
            DependencyProperty.Register(
                nameof(Crews),
                typeof(ObservableCollection<Crew>),
                typeof(ShipMapView),
                new PropertyMetadata(null, OnCrewsChanged));

        public ObservableCollection<Crew> Crews
        {
            get { return (ObservableCollection<Crew>)GetValue(CrewsProperty); }
            set { SetValue(CrewsProperty, value); }
        }

        public ShipMapView()
        {
            InitializeComponent();
        }

        private static void OnShipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShipMapView view = d as ShipMapView;

            if (view == null)
                return;

            view.DrawShip();
        }

        private static void OnCrewsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShipMapView view = d as ShipMapView;

            if (view == null)
                return;

            ObservableCollection<Crew> oldCrews = e.OldValue as ObservableCollection<Crew>;
            ObservableCollection<Crew> newCrews = e.NewValue as ObservableCollection<Crew>;

            if (oldCrews != null)
            {
                oldCrews.CollectionChanged -= view.Crews_CollectionChanged;

                foreach (Crew crew in oldCrews)
                {
                    crew.PropertyChanged -= view.Crew_PropertyChanged;
                }
            }

            if (newCrews != null)
            {
                newCrews.CollectionChanged += view.Crews_CollectionChanged;

                foreach (Crew crew in newCrews)
                {
                    crew.PropertyChanged += view.Crew_PropertyChanged;
                }
            }

            view.DrawShip();
        }

        private void Crews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Crew crew in e.OldItems)
                {
                    crew.PropertyChanged -= Crew_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (Crew crew in e.NewItems)
                {
                    crew.PropertyChanged += Crew_PropertyChanged;
                }
            }

            DrawShip();
        }

        private void Crew_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Crew.X) ||
                e.PropertyName == nameof(Crew.Y) ||
                e.PropertyName == nameof(Crew.Action) ||
                e.PropertyName == nameof(Crew.Activity) ||
                e.PropertyName == nameof(Crew.IsAlive))
            {
                DrawShip();
            }
        }

        private void DrawShip()
        {
            ShipCanvas.Children.Clear();

            if (Ship == null ||
                Ship.Interior == null ||
                Ship.Interior.Rooms == null)
                return;

            ShipMapBuilder.RebuildGlobalTileMap(Ship);

            foreach (ShipMapTile tile in Ship.Interior.GlobalTileMap.Values)
            {
                DrawTile(tile);
            }

            foreach (InteriorRoom room in Ship.Interior.Rooms)
            {
                DrawRoomName(room);
            }

            DrawCrews();
        }

        private void DrawCrews()
        {
            if (Crews == null)
                return;

            foreach (Crew crew in Crews)
            {
                if (!crew.IsAlive)
                    continue;

                if (!crew.IsInShip)
                    continue;

                if (Ship != null && crew.ShipId != Ship.Id)
                    continue;

                DrawCrew(crew);
            }
        }

        private void DrawCrew(Crew crew)
        {
            double drawX = OffsetX + (crew.X * TileSize);
            double drawY = OffsetY + (crew.Y * TileSize);

            Grid crewMarker = new Grid
            {
                Width = TileSize,
                Height = TileSize,
                ToolTip = string.Format(
                    "{0}\nX: {1}, Y: {2}\nAction: {3}\nActivity: {4}",
                    crew.Name,
                    crew.X,
                    crew.Y,
                    crew.Action,
                    crew.Activity)
            };

            Ellipse body = new Ellipse
            {
                Width = 18,
                Height = 18,
                Fill = GetCrewBrush(crew),
                Stroke = Brushes.LightGray,
                StrokeThickness = 1.5,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock label = new TextBlock
            {
                Text = string.IsNullOrEmpty(crew.Name) ? "?" : crew.Name.Substring(0, 1),
                Foreground = GetCrewTextBrush(crew),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            crewMarker.Children.Add(body);
            crewMarker.Children.Add(label);

            Canvas.SetLeft(crewMarker, drawX);
            Canvas.SetTop(crewMarker, drawY);

            ShipCanvas.Children.Add(crewMarker);
        }

        private Brush GetCrewTextBrush(Crew crew)
        {
            switch (crew.Action)
            {
                case CrewAction.Eat:
                case CrewAction.Sleep:
                case CrewAction.Work:
                    return Brushes.White;

                default:
                    return Brushes.Black;
            }
        }

        private Brush GetCrewBrush(Crew crew)
        {
            switch (crew.Action)
            {
                case CrewAction.Eat:
                    return Brushes.LimeGreen;

                case CrewAction.Sleep:
                    return Brushes.MediumPurple;

                case CrewAction.Work:
                    return Brushes.DeepSkyBlue;

                default:
                    return Brushes.White;
            }
        }

        private void DrawTile(ShipMapTile tile)
        {
            double drawX = OffsetX + (tile.X * TileSize);
            double drawY = OffsetY + (tile.Y * TileSize);

            Rectangle rect = new Rectangle
            {
                Width = TileSize,
                Height = TileSize,
                Fill = GetTileBrush(tile.TileType),
                Stroke = GetTileStroke(tile.TileType),
                StrokeThickness = GetStrokeThickness(tile.TileType),
                ToolTip = GetTileToolTip(tile)
            };

            Canvas.SetLeft(rect, drawX);
            Canvas.SetTop(rect, drawY);

            ShipCanvas.Children.Add(rect);

            if (tile.ObjectId.HasValue)
            {
                DrawObjectMarker(drawX, drawY);
            }
        }

        private void DrawRoomName(InteriorRoom room)
        {
            double drawX = OffsetX + (room.WorldX * TileSize);
            double drawY = OffsetY + (room.WorldY * TileSize) - 22;

            TextBlock textBlock = new TextBlock
            {
                Text = room.Name,
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
                Padding = new Thickness(4, 2, 4, 2)
            };

            Canvas.SetLeft(textBlock, drawX);
            Canvas.SetTop(textBlock, drawY);

            ShipCanvas.Children.Add(textBlock);
        }

        private void DrawObjectMarker(double drawX, double drawY)
        {
            Ellipse marker = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(marker, drawX + (TileSize / 2) - 4);
            Canvas.SetTop(marker, drawY + (TileSize / 2) - 4);

            ShipCanvas.Children.Add(marker);
        }

        private Brush GetTileBrush(InteriorTileType tileType)
        {
            switch (tileType)
            {
                #region Workstations
                case InteriorTileType.Cockpit:
                    return (Brush)new BrushConverter().ConvertFromString("#287FC7");

                case InteriorTileType.WeaponsConsole:
                    return (Brush)new BrushConverter().ConvertFromString("#A31459");

                case InteriorTileType.MedicalConsole:
                    return (Brush)new BrushConverter().ConvertFromString("#2271BF");
                #endregion Workstations

                case InteriorTileType.Floor:
                    return Brushes.DimGray;

                case InteriorTileType.Wall:
                    return Brushes.DarkSlateGray;

                case InteriorTileType.Door:
                    return Brushes.Goldenrod;

                case InteriorTileType.Bed:
                    return Brushes.SteelBlue;

                case InteriorTileType.StorageBox:
                    return Brushes.SaddleBrown;

                case InteriorTileType.Workbench:
                    return Brushes.DarkOrange;

                case InteriorTileType.KitchenCounter:
                    return Brushes.DarkRed;

                case InteriorTileType.Table:
                    return Brushes.Peru;

                case InteriorTileType.Sofa:
                    return Brushes.MediumPurple;

                default:
                    return Brushes.Gray;
            }
        }

        private Brush GetTileStroke(InteriorTileType tileType)
        {
            if (tileType == InteriorTileType.Door)
                return Brushes.Yellow;

            return Brushes.Black;
        }

        private double GetStrokeThickness(InteriorTileType tileType)
        {
            if (tileType == InteriorTileType.Door)
                return 1.5;

            return 0.5;
        }

        private string GetTileToolTip(ShipMapTile tile)
        {
            return string.Format(
                "Global: {0},{1}\nTile: {2}\nFacility: {3}\nWalkable: {4}\nShared Rooms: {5}",
                tile.X,
                tile.Y,
                tile.TileType,
                tile.FacilityType,
                tile.IsWalkable,
                tile.RoomIds == null ? 0 : tile.RoomIds.Count);
        }

        private void ShipCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;

            _dragStartPoint = e.GetPosition(this);
            _startHorizontalOffset = MapScrollViewer.HorizontalOffset;
            _startVerticalOffset = MapScrollViewer.VerticalOffset;

            ShipCanvas.CaptureMouse();
            Cursor = Cursors.Hand;

            e.Handled = true;
        }

        private void ShipCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging)
                return;

            Point currentPoint = e.GetPosition(this);

            double deltaX = currentPoint.X - _dragStartPoint.X;
            double deltaY = currentPoint.Y - _dragStartPoint.Y;

            MapScrollViewer.ScrollToHorizontalOffset(_startHorizontalOffset - deltaX);
            MapScrollViewer.ScrollToVerticalOffset(_startVerticalOffset - deltaY);

            e.Handled = true;
        }

        private void ShipCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDrag();
            e.Handled = true;
        }

        private void ShipCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton != MouseButtonState.Pressed)
            {
                EndDrag();
            }
        }

        private void EndDrag()
        {
            _isDragging = false;
            ShipCanvas.ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
        }
    }
}