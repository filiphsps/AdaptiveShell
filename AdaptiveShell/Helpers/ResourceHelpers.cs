using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

// https://github.com/files-community/Files/blob/845882b30bd4883d2aebea68e9874a1b08e4ed0f/src/Files/Helpers/ResourceHelpers.cs
namespace AdaptiveShell.Helpers {
    [MarkupExtensionReturnType(ReturnType = typeof(String))]
    public sealed class ResourceString : MarkupExtension {
        private static ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

        public String Name {
            get; set;
        }

        protected override String ProvideValue() {
            return resourceLoader.GetString(this.Name);
        }
    }
}
