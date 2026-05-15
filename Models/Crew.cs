using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace Space_RPG.Models
{
    public enum CrewAction
    {
        None,
        Eat,
        Work,
        Sleep
    }

    public class Crew : ViewModelBase
    {
        #region Profile

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private int _age;
        public int Age
        {
            get { return _age; }
            set { SetProperty(ref _age, value); }
        }

        private bool _isPlayer;
        public bool IsPlayer
        {
            get { return _isPlayer; }
            set { SetProperty(ref _isPlayer, value); }
        }

        #endregion Profile

        #region Location

        private List<ShipMapTile> _currentPath;
        public List<ShipMapTile> CurrentPath
        {
            get { return _currentPath; }
            set { SetProperty(ref _currentPath, value); }
        }

        private int _currentPathIndex;
        public int CurrentPathIndex
        {
            get { return _currentPathIndex; }
            set { SetProperty(ref _currentPathIndex, value); }
        }
        private int _x;
        public int X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        private int _targetX;
        public int TargetX
        {
            get { return _targetX; }
            set { SetProperty(ref _targetX, value); }
        }

        private int _targetY;
        public int TargetY
        {
            get { return _targetY; }
            set { SetProperty(ref _targetY, value); }
        }

        private bool _isInShip;
        public bool IsInShip
        {
            get { return _isInShip; }
            set { SetProperty(ref _isInShip, value); }
        }

        private bool _isInRover;
        public bool IsInRover
        {
            get { return _isInRover; }
            set { SetProperty(ref _isInRover, value); }
        }

        private bool _isInPlanet;
        public bool IsInPlanet
        {
            get { return _isInPlanet; }
            set { SetProperty(ref _isInPlanet, value); }
        }

        private int _assignedBedX;
        public int AssignedBedX
        {
            get { return _assignedBedX; }
            set { _assignedBedX = value; OnPropertyChanged(); }
        }

        private int _assignedBedY;
        public int AssignedBedY
        {
            get { return _assignedBedY; }
            set { _assignedBedY = value; OnPropertyChanged(); }
        }

        private Guid _shipId;
        public Guid ShipId
        {
            get { return _shipId; }
            set { SetProperty(ref _shipId, value); }
        }

        private Guid _roverId;
        public Guid RoverId
        {
            get { return _roverId; }
            set { SetProperty(ref _roverId, value); }
        }

        private Guid _planetId;
        public Guid PlanetId
        {
            get { return _planetId; }
            set { SetProperty(ref _planetId, value); }
        }

        #endregion Location

        #region Job Properties

        private Job _job;
        public Job Job
        {
            get { return _job; }
            set { SetProperty(ref _job, value); }
        }

        private JobStatus _jobStatus;
        public JobStatus JobStatus
        {
            get { return _jobStatus; }
            set { SetProperty(ref _jobStatus, value); }
        }

        private Activity _activity;
        public Activity Activity
        {
            get { return _activity; }
            set { SetProperty(ref _activity, value); }
        }

        private CrewAction _action;
        public CrewAction Action
        {
            get { return _action; }
            set { SetProperty(ref _action, value); }
        }
        private int _actionStartedTick;
        public int ActionStartedTick
        {
            get { return _actionStartedTick; }
            set { SetProperty(ref _actionStartedTick, value); }
        }

        private int _nextActionDecisionTick;
        public int NextActionDecisionTick
        {
            get { return _nextActionDecisionTick; }
            set { SetProperty(ref _nextActionDecisionTick, value); }
        }
        #endregion Job Properties

        #region Skills

        private double _pilot;
        public double Pilot
        {
            get { return _pilot; }
            set { SetProperty(ref _pilot, value); }
        }

        private double _drive;
        public double Drive
        {
            get { return _drive; }
            set { SetProperty(ref _drive, value); }
        }

        private double _aim;
        public double Aim
        {
            get { return _aim; }
            set { SetProperty(ref _aim, value); }
        }

        private double _medic;
        public double Medic
        {
            get { return _medic; }
            set { SetProperty(ref _medic, value); }
        }

        private double _mine;
        public double Mine
        {
            get { return _mine; }
            set { SetProperty(ref _mine, value); }
        }

        private double _repair;
        public double Repair
        {
            get { return _repair; }
            set { SetProperty(ref _repair, value); }
        }

        #endregion Skills

        #region Status

        private bool _isAlive;
        public bool IsAlive
        {
            get { return _isAlive; }
            set { SetProperty(ref _isAlive, value); }
        }

        private bool _isSleeping;
        public bool IsSleeping
        {
            get { return _isSleeping; }
            set { SetProperty(ref _isSleeping, value); }
        }

        private bool _isEating;
        public bool IsEating
        {
            get { return _isEating; }
            set { SetProperty(ref _isEating, value); }
        }

        private int _price;
        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private int _cash;
        public int Cash
        {
            get { return _cash; }
            set { SetProperty(ref _cash, value); }
        }

        private int _health;
        public int Health
        {
            get { return _health; }
            set { SetProperty(ref _health, value); }
        }

        private float _fatigue;
        public float Fatigue
        {
            get { return _fatigue; }
            set { SetProperty(ref _fatigue, value); }
        }

        private float _hunger;
        public float Hunger
        {
            get { return _hunger; }
            set { SetProperty(ref _hunger, value); }
        }

        private string _deathReason;
        public string DeathReason
        {
            get { return _deathReason; }
            set { SetProperty(ref _deathReason, value); }
        }

        #endregion Status

        #region Constructor

        public Crew()
        {
            Id = Guid.NewGuid();
            Job = Job.None;
            IsAlive = true;
        }

        #endregion Constructor
    }


}
