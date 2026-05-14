using Space_RPG.Models;
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

namespace Space_RPG.Views
{
    using Space_RPG.Helpers;

    /// <summary>
    /// Interaction logic for ShipMapView.xaml
    /// </summary>
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
