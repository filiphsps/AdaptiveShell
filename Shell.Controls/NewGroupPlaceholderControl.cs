using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Shell.Controls {
    /// <summary>
    /// The <see cref="NewGroupPlaceholderControl"/> control implements simple placeholder with DragOver visual state 
    /// to supply UI representation during drag and drop operations.
    /// </summary>
    [TemplateVisualState(Name = "DragOver", GroupName = "DragStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "DragStates")]
    public class NewGroupPlaceholderControl : Control {
        public NewGroupPlaceholderControl() {
            this.AllowDrop = true;
            this.DefaultStyleKey = typeof(NewGroupPlaceholderControl);
        }

        protected override void OnDragOver(DragEventArgs e) {
            base.OnDragOver(e);
            VisualStateManager.GoToState(this, "DragOver", true);
        }

        protected override void OnDrop(DragEventArgs e) {
            base.OnDrop(e);
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnDragLeave(DragEventArgs e) {
            base.OnDragLeave(e);
            VisualStateManager.GoToState(this, "Normal", true);
        }
    }
}
