using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Models
{
    public class Applicant : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }

        public string PhotoPath { get; set; }

        public int Age { get; set; }

        public string Skills { get; set; }

        public int Price { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();

                SelectionChangedAction?.Invoke();
            }
        }

        public Action SelectionChangedAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
