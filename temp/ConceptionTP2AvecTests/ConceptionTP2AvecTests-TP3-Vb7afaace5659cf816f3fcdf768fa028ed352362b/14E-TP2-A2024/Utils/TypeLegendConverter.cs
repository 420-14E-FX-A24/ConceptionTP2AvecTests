using Automate.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Automate.Utils
{
    public class TypeLegendConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Task.TypeEnum task)
            {
                var legend = Brushes.Transparent;

                switch (task)
                {
                    case Task.TypeEnum.Semis:
                        legend = Brushes.DarkRed;
                        break;
                    case Task.TypeEnum.Rempotage:
                        legend = Brushes.Green;
                        break;
                    case Task.TypeEnum.Entretien:
                        legend = Brushes.Blue;
                        break;
                    case Task.TypeEnum.Arrosage:
                        legend = Brushes.Black;
                        break;
                    case Task.TypeEnum.Récolte:
                        legend = Brushes.Gold;
                        break;
                    case Task.TypeEnum.Commandes:
                        legend = Brushes.Indigo;
                        break;
                    case Task.TypeEnum.Événements:
                        legend = Brushes.DarkOliveGreen;
                        break;
                    default:
                        legend = Brushes.Transparent;
                        break;
                }
                return legend;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
