using AdaptiveShell.LiveTiles.Models.BaseElements;
using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models {
    public abstract class AdaptiveChildElement : BaseAdaptiveElement {
        public AdaptiveChildElement(XmlTemplateParser.NotificationType context) : base(context) { }

        internal AdaptiveParentElement Parent { get; set; }

        /// <summary>
        /// Returns the next element below this current element
        /// </summary>
        /// <returns></returns>
        internal virtual AdaptiveChildElement GetNextElementBelowThisOne() {
            if (this.Parent == null)
                return null;

            Boolean foundMyself = false;

            foreach (AdaptiveChildElement child in this.Parent.GetAllChildren()) {
                if (foundMyself)
                    return child;

                if (child == this)
                    foundMyself = true; // we'll return the next child
            }

            // There was no next child, so go to the parent and have the parent return its next element
            return this.Parent.GetNextElementBelowThisOne();
        }
    }
}
