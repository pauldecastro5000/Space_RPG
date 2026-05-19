using Newtonsoft.Json;
using Space_RPG.Models;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Space_RPG.Windows
{
    public partial class RoomEditorWindow : Window
    {
        private readonly ShipRoomDesign _originalRoom;
        private readonly ShipRoomDesign _editableRoom;

        public Array FacilityTypeOptions
        {
            get { return Enum.GetValues(typeof(FacilityType)); }
        }

        public Array InteriorObjectTypeOptions
        {
            get { return Enum.GetValues(typeof(InteriorObjectType)); }
        }

        public Array InteriorTileTypeOptions
        {
            get { return Enum.GetValues(typeof(InteriorTileType)); }
        }

        public ObservableCollection<ShipObjectDesign> Objects { get; private set; }

        public string Name
        {
            get { return _editableRoom.Name; }
            set { _editableRoom.Name = value; }
        }

        public FacilityType RoomType
        {
            get { return _editableRoom.RoomType; }
            set { _editableRoom.RoomType = value; }
        }

        public int RoomWidth
        {
            get { return _editableRoom.Width; }
            set { _editableRoom.Width = value; }
        }

        public int RoomHeight
        {
            get { return _editableRoom.Height; }
            set { _editableRoom.Height = value; }
        }

        public int WorldX
        {
            get { return _editableRoom.WorldX; }
            set { _editableRoom.WorldX = value; }
        }

        public int WorldY
        {
            get { return _editableRoom.WorldY; }
            set { _editableRoom.WorldY = value; }
        }

        public RoomEditorWindow(ShipRoomDesign room)
        {
            InitializeComponent();

            _originalRoom = room;

            string json = JsonConvert.SerializeObject(room);
            _editableRoom = JsonConvert.DeserializeObject<ShipRoomDesign>(json);

            Objects = new ObservableCollection<ShipObjectDesign>(_editableRoom.Objects);

            DataContext = this;
        }

        private void IntegerOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-]+");

            e.Handled = regex.IsMatch(e.Text);
        }
        private void AddObject_Click(object sender, RoutedEventArgs e)
        {
            ShipObjectDesign obj = new ShipObjectDesign
            {
                Name = "New Object",
                ObjectType = InteriorObjectType.None,
                TileType = InteriorTileType.Table,
                X = 1,
                Y = 1,
                Width = 1,
                Height = 1,
                AllowMultipleCrew = false
            };

            obj.InteractionDirections.Add(InteractionDirection.Bottom);

            Objects.Add(obj);
        }

        private void RemoveObject_Click(object sender, RoutedEventArgs e)
        {
            ShipObjectDesign selected = ObjectsGrid.SelectedItem as ShipObjectDesign;

            if (selected == null)
                return;

            Objects.Remove(selected);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            _editableRoom.Objects.Clear();

            foreach (ShipObjectDesign obj in Objects)
                _editableRoom.Objects.Add(obj);

            _originalRoom.Name = _editableRoom.Name;
            _originalRoom.RoomType = _editableRoom.RoomType;
            _originalRoom.Width = _editableRoom.Width;
            _originalRoom.Height = _editableRoom.Height;
            _originalRoom.WorldX = _editableRoom.WorldX;
            _originalRoom.WorldY = _editableRoom.WorldY;

            _originalRoom.Objects.Clear();

            foreach (ShipObjectDesign obj in _editableRoom.Objects)
                _originalRoom.Objects.Add(obj);

            DialogResult = true;
            Close();
        }
    }
}