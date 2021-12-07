using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class AdaptiveParentElement : AdaptiveChildElement
    {
        public AdaptiveParentElement(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        /// <summary>
        /// Returns true if this element or any descendants use data binding
        /// </summary>
        /// <returns></returns>
        public bool AnyUseDataBinding()
        {
            return UsesDataBinding || GetAllDescendants().Any(i => i.UsesDataBinding);
        }

        internal abstract IEnumerable<AdaptiveChildElement> GetAllChildren();

        internal IEnumerable<AdaptiveChildElement> GetAllDescendants()
        {
            foreach (var child in GetAllChildren())
            {
                yield return child;

                if (child is AdaptiveParentElement)
                {
                    foreach (var descendant in (child as AdaptiveParentElement).GetAllDescendants())
                        yield return descendant;
                }
            }
        }

        internal override void ApplyDataBinding(DataBindingValues dataBinding)
        {
            base.ApplyDataBindingToThisInstance(dataBinding);

            foreach (var child in GetAllChildren())
            {
                child.ApplyDataBinding(dataBinding);
            }
        }
    }
}
