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
        public int Integer { get; private set; } = 0;

        public BadgeValueType Type { get; private set; } = BadgeValueType.none;

        private BadgeValue(string value)
        {
            if (value == null)
                return;

            int integer;

            //if they provided a valid integer
            if (int.TryParse(value, out integer))
            {
                if (integer < 0)
                    integer = 0;

                Type = BadgeValueType.integer;
                Integer = integer;
                return;
            }

            BadgeValueType type;

            //otherwise if they provided a valid glyph value
            if (Enum.TryParse(value, out type))
            {
                if (type == BadgeValueType.integer)
                    type = BadgeValueType.none;

                Type = type;
                return;
            }

            //otherwise their value wasn't valid, and we fall back to none
        }

        public override string ToString()
        {
            if (Type == BadgeValueType.none)
                return "";

            if (Type == BadgeValueType.integer)
            {
                if (Integer <= 0)
                    return "";

                return Integer.ToString();
            }

            return Type.ToString();
        }

        public static BadgeValue Parse(string str)
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
        public bool HasBadge()
        {
            if (Type == BadgeValueType.none)
                return false;

            if (Type == BadgeValueType.integer && Integer <= 0)
                return false;

            return true;
        }
    }
}
