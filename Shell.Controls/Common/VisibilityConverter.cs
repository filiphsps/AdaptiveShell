using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Shell.Controls.Common
{
    /// <summary>
    /// Converter from/to Visibility and Boolean.
    /// </summary>
    /// <remarks>
    /// true = Visible
    /// false = Collapsed
    /// </remarks>
    public class VisibilityConverter : IValueConverter {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisibilityConverter"/> class.
        /// </summary>
        public VisibilityConverter() {
            this.Opposite = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="VisibilityConverter"/> is opposite.
        /// </summary>
        /// <value><c>true</c> if opposite; otherwise, <c>false</c>.</value>
        public Boolean Opposite { get; set; }

        #region IValueConverter Members

        public Object Convert(Object value, Type targetType, Object parameter, String language) {
            Boolean not = Object.Equals(parameter, "not") || this.Opposite;
            if (value is Boolean && targetType == typeof(Visibility)) {
                return ((Boolean)value) != not ? Visibility.Visible : Visibility.Collapsed;
            }
            if (value is Visibility && targetType.GetType() == typeof(Boolean)) {
                return (((Visibility)value) == Visibility.Visible) != not;
            }
            return value;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, String culture) {
            return this.Convert(value, targetType, parameter, culture);
        }


        #endregion
    }
}
