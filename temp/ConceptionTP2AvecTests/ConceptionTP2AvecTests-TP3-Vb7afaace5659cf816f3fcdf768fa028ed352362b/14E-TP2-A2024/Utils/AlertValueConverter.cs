using Automate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Automate.Utils
{
    public class AlertValueConverter : IValueConverter
    {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is List<Task.TypeEnum> taches)
			{
				int arrosagesCount = taches.Count(t => t == Task.TypeEnum.Arrosage);
				int semisCount = taches.Count(t => t == Task.TypeEnum.Semis);
				return $"Arrosages: {arrosagesCount}\nSemis: {semisCount}";
			}
			return string.Empty;
		}


		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
