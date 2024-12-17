using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate.Utils
{
    public class CustomPropertyChangedEvent : PropertyChangedEventArgs
    {
        public object NewValue { get; }

        public CustomPropertyChangedEvent(string propertyName, object newValue)
            : base(propertyName)
        {
            NewValue = newValue;
        }
    }
}
