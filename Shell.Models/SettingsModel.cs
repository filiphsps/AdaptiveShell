using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell.Models {
    public class SettingsModel {
        /// <summary>
        /// If elements should follow the newer rounded Windows 11 style
        /// </summary>
        public Boolean CornerRadius { get; set; } = true;
    }
}
