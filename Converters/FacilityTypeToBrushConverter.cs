using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Space_RPG.Converters
{
    public class FacilityTypeToBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (!(value is FacilityType type))
                return Brushes.Transparent;

            switch (type)
            {
                case FacilityType.MainDeck:
                    return Brushes.SteelBlue;

                case FacilityType.Cafeteria:
                    return Brushes.Orange;

                case FacilityType.Armoury:
                    return Brushes.DarkRed;

                case FacilityType.MaleCrewQuarters:
                    return Brushes.MediumPurple;

                case FacilityType.FemaleCrewQuarters:
                    return Brushes.LightPink;

                case FacilityType.Garden:
                    return Brushes.ForestGreen;

                case FacilityType.Gym:
                    return Brushes.DarkCyan;

                case FacilityType.MedicalBay:
                    return Brushes.DeepPink;

                case FacilityType.Cargo:
                    return Brushes.SaddleBrown;

                case FacilityType.Corridor:
                    return Brushes.DimGray;

                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
