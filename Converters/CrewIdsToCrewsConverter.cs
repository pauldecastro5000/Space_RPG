using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Space_RPG.Converters
{
    public class CrewIdsToCrewsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var crewIds = values[0] as IEnumerable<Guid>;
            var allCrews = values[1] as IEnumerable<Crew>;

            if (crewIds == null || allCrews == null)
                return null;

            return allCrews
                .Where(x => crewIds.Contains(x.Id))
                .ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
