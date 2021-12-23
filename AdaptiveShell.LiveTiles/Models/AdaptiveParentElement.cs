using AdaptiveShell.LiveTiles.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models {
    public abstract class AdaptiveParentElement : AdaptiveChildElement {
        public AdaptiveParentElement(XmlTemplateParser.NotificationType context) : base(context) { }

        /// <summary>
        /// Returns true if this element or any descendants use data binding
        /// </summary>
        /// <returns></returns>
        public Boolean AnyUseDataBinding() {
            return this.UsesDataBinding || this.GetAllDescendants().Any(i => i.UsesDataBinding);
        }

        internal abstract IEnumerable<AdaptiveChildElement> GetAllChildren();

        internal IEnumerable<AdaptiveChildElement> GetAllDescendants() {
            foreach (AdaptiveChildElement child in this.GetAllChildren()) {
                yield return child;

                if (child is AdaptiveParentElement) {
                    foreach (AdaptiveChildElement descendant in (child as AdaptiveParentElement).GetAllDescendants())
                        yield return descendant;
                }
            }
        }

        internal override void ApplyDataBinding(DataBindingValues dataBinding) {
            base.ApplyDataBindingToThisInstance(dataBinding);

            foreach (AdaptiveChildElement child in this.GetAllChildren()) {
                child.ApplyDataBinding(dataBinding);
            }
        }
    }
}
