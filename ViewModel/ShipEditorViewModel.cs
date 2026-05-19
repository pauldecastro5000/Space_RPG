using Space_RPG.Helpers;
using Space_RPG.Models;
using Space_RPG.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Space_RPG.ViewModel
{
    public class ShipEditorViewModel : ViewModelBase
    {
        private readonly string _filePath;

        public ShipDesign Design { get; private set; }

        public ObservableCollection<ShipRoomDesign> Rooms { get; set; }

        private ShipRoomDesign _selectedRoom;
        public ShipRoomDesign SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                _selectedRoom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Objects));
            }
        }

        private ShipObjectDesign _selectedObject;
        public ShipObjectDesign SelectedObject
        {
            get { return _selectedObject; }
            set { _selectedObject = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ShipObjectDesign> Objects
        {
            get
            {
                if (SelectedRoom == null)
                    return null;

                return new ObservableCollection<ShipObjectDesign>(SelectedRoom.Objects);
            }
        }

        public Array FacilityTypeOptions => Enum.GetValues(typeof(FacilityType));
        public Array InteriorObjectTypeOptions => Enum.GetValues(typeof(InteriorObjectType));
        public Array InteriorTileTypeOptions => Enum.GetValues(typeof(InteriorTileType));

        public RelayCommand AddRoomCommand { get; private set; }
        public RelayCommand RemoveRoomCommand { get; private set; }
        public RelayCommand AddObjectCommand { get; private set; }
        public RelayCommand RemoveObjectCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public event Action RequestClose;

        public ShipEditorViewModel(string filePath)
        {
            _filePath = filePath;

            Design = ShipDesignLoader.LoadFromFile(filePath);

            if (Design == null)
                Design = new ShipDesign { Name = "Default Ship" };

            Rooms = new ObservableCollection<ShipRoomDesign>(Design.Rooms);

            AddRoomCommand = new RelayCommand(AddRoom);
            RemoveRoomCommand = new RelayCommand(RemoveRoom);
            AddObjectCommand = new RelayCommand(AddObject);
            RemoveObjectCommand = new RelayCommand(RemoveObject);
            SaveCommand = new RelayCommand(Save);
        }

        private void AddRoom()
        {
            ShipRoomDesign room = new ShipRoomDesign
            {
                Name = "New Room",
                RoomType = FacilityType.Corridor,
                Width = 5,
                Height = 5,
                WorldX = 0,
                WorldY = 0
            };

            Rooms.Add(room);
            SelectedRoom = room;
        }

        private void RemoveRoom()
        {
            if (SelectedRoom == null)
                return;

            Rooms.Remove(SelectedRoom);
            SelectedRoom = null;
        }

        private void AddObject()
        {
            if (SelectedRoom == null)
                return;

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

            SelectedRoom.Objects.Add(obj);

            OnPropertyChanged(nameof(Objects));
            SelectedObject = obj;
        }

        private void RemoveObject()
        {
            if (SelectedRoom == null || SelectedObject == null)
                return;

            SelectedRoom.Objects.Remove(SelectedObject);
            SelectedObject = null;

            OnPropertyChanged(nameof(Objects));
        }

        private void Save()
        {
            Design.Rooms.Clear();

            foreach (ShipRoomDesign room in Rooms)
            {
                Design.Rooms.Add(room);
            }

            if (ShipDesignLoader.SaveToFile(_filePath, Design))
                MessageBox.Show("Ship design saved successfully.", "Ship Editor");

            RequestClose?.Invoke();
        }
    }
}