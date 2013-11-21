using Eriver.Trackers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Eriver.GUIServer
{
    [ValueConversion(typeof(int), typeof(Brush))]
    class ITrackerToColorConverter : MarkupExtension, IValueConverter
    {

        public ITrackerToColorConverter()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        {
            int status = (Int32) parameter;
            Color c;
            switch (status)
            {
                case 0: c = Colors.Green; break;
                case 1: c = Colors.Yellow; break;
                case 2: c = Colors.Red; break;
                default: c = Colors.Black; break;
            }
            var converter = new System.Windows.Media.BrushConverter();
            var myBrush = converter.ConvertFromString(c.ToString()) as Brush;

            return myBrush;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
