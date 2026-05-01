using Space_RPG.Helpers;
using Space_RPG.Models;
using Space_RPG.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Space_RPG.ViewModel
{
    public class ApplicantSelectionViewModel
    {
        public ObservableCollection<Applicant> Applicants { get; set; }

        public ObservableCollection<Applicant> SelectedApplicants { get; set; }

        public RelayCommand AcceptCommand { get; }

        public ApplicantSelectionViewModel()
        {
            Applicants = new ObservableCollection<Applicant>
            {
                new Applicant { Name="Applicant 1", Age=25, Skills="C#, WPF", Price=100, PhotoPath="/Images/applicant1.png" },
                new Applicant { Name="Applicant 2", Age=28, Skills="SQL, PLC", Price=120, PhotoPath="/Images/applicant2.png" },
                new Applicant { Name="Applicant 3", Age=24, Skills="Vision, C++", Price=150, PhotoPath="/Images/applicant3.png" },
                new Applicant { Name="Applicant 4", Age=30, Skills="MVVM", Price=130, PhotoPath="/Images/applicant4.png" },

                new Applicant { Name="Applicant 5", Age=26, Skills="C#", Price=110, PhotoPath="/Images/applicant5.png" },
                new Applicant { Name="Applicant 6", Age=29, Skills="Automation", Price=140, PhotoPath="/Images/applicant6.png" },
                new Applicant { Name="Applicant 7", Age=23, Skills="UI Design", Price=90, PhotoPath="/Images/applicant7.png" },
                new Applicant { Name="Applicant 8", Age=31, Skills="Database", Price=160, PhotoPath="/Images/applicant8.png" },

                new Applicant { Name="Applicant 9", Age=27, Skills="Motion Control", Price=170, PhotoPath="/Images/applicant9.png" },
                new Applicant { Name="Applicant 10", Age=25, Skills="Testing", Price=100, PhotoPath="/Images/applicant10.png" },
                new Applicant { Name="Applicant 11", Age=32, Skills="Debugging", Price=180, PhotoPath="/Images/applicant11.png" },
                new Applicant { Name="Applicant 12", Age=24, Skills="C++, WPF", Price=130, PhotoPath="/Images/applicant12.png" },
            };

            SelectedApplicants = new ObservableCollection<Applicant>();

            foreach (var applicant in Applicants)
            {
                applicant.SelectionChangedAction = RefreshButtons;
            }

            AcceptCommand = new RelayCommand(
                Accept,
                CanAccept);
        }

        private void Accept()
        {
            SelectedApplicants.Clear();

            foreach (var applicant in Applicants.Where(x => x.IsSelected))
            {
                SelectedApplicants.Add(applicant);
            }

            foreach (Window window in Application.Current.Windows)
            {
                if (window is ApplicantSelectionWindow)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }

        private bool CanAccept()
        {
            return Applicants.Any(x => x.IsSelected);
        }

        private void RefreshButtons()
        {
            AcceptCommand.RaiseCanExecuteChanged();
        }
    }
}
