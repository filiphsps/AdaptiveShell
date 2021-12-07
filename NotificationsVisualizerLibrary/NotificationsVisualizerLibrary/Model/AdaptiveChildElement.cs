using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model.BaseElements;

namespace NotificationsVisualizerLibrary.Model
{
    internal abstract class AdaptiveChildElement : BaseAdaptiveElement
    {
        public AdaptiveChildElement(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal AdaptiveParentElement Parent { get; set; }

        /// <summary>
        /// Returns the next element below this current element
        /// </summary>
        /// <returns></returns>
        internal virtual AdaptiveChildElement GetNextElementBelowThisOne()
        {
            if (Parent == null)
                return null;

            bool foundMyself = false;

            foreach (var child in Parent.GetAllChildren())
            {
                if (foundMyself)
                    return child;

                if (child == this)
                    foundMyself = true; // we'll return the next child
            }

            // There was no next child, so go to the parent and have the parent return its next element
            return Parent.GetNextElementBelowThisOne();
        }
    }
}
