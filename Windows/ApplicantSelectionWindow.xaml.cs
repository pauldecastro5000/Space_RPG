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
using System.Windows.Shapes;

namespace Space_RPG.Windows
{
    /// <summary>
    /// Interaction logic for ApplicantSelectionWindow.xaml
    /// </summary>
    public partial class ApplicantSelectionWindow : Window
    {
        public ApplicantSelectionWindow()
        {
            InitializeComponent();
        }

        private void ApplicantBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border &&
                border.DataContext is Applicant applicant)
            {
                applicant.IsSelected = !applicant.IsSelected;
            }
        }
    }
}
