using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model.Enums;

namespace NotificationsVisualizerLibrary.Model
{
    public sealed class BadgeValue
    {
        public Int32 Integer { get; private set; } = 0;

        public BadgeValueType Type { get; private set; } = BadgeValueType.none;

        private BadgeValue(String value)
        {
            if (value == null)
                return;

            Int32 integer;

            //if they provided a valid integer
            if (Int32.TryParse(value, out integer))
            {
                if (integer < 0)
                    integer = 0;

                this.Type = BadgeValueType.integer;
                this.Integer = integer;
                return;
            }

            BadgeValueType type;

            //otherwise if they provided a valid glyph value
            if (Enum.TryParse(value, out type))
            {
                if (type == BadgeValueType.integer)
                    type = BadgeValueType.none;

                this.Type = type;
                return;
            }

            //otherwise their value wasn't valid, and we fall back to none
        }

        public override String ToString()
        {
            if (this.Type == BadgeValueType.none)
                return "";

            if (this.Type == BadgeValueType.integer)
            {
                if (this.Integer <= 0)
                    return "";

                return this.Integer.ToString();
            }

            return this.Type.ToString();
        }

        public static BadgeValue Parse(String str)
        {
            return new BadgeValue(str);
        }

        public static BadgeValue Default()
        {
            return new BadgeValue(null);
        }

        /// <summary>
        /// Returns true if there's a badge to be displayed. Otherwise returns false.
        /// </summary>
        /// <returns></returns>
        public Boolean HasBadge()
        {
            if (this.Type == BadgeValueType.none)
                return false;

            if (this.Type == BadgeValueType.integer && this.Integer <= 0)
                return false;

            return true;
        }
    }
}
