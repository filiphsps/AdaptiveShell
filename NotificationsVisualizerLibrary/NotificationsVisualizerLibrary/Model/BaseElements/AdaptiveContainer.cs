using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model.Enums;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Model.BaseElements;

namespace NotificationsVisualizerLibrary.Model
{
    internal class AdaptiveContainer : AdaptiveParentElement
    {
        public AdaptiveContainer(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        private static readonly String ATTR_TEXT_STACKING = "hint-textStacking";

        private List<AdaptiveChildElement> _children = new List<AdaptiveChildElement>();
        internal IReadOnlyList<AdaptiveChildElement> Children
        {
            get { return this._children; }
        }

        public void SwapChildren(IEnumerable<AdaptiveChildElement> newChildren)
        {
            this._children.Clear();
            this._children.AddRange(newChildren);
        }

        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        /// <summary>
        /// Parses a node that has already been parsed some (for example, the Binding actually parses the node first, and then this parses some extra stuff. Binding is responsible for showing warnings about unknown unprocessed attributes after this method has parsed through what it understands).
        /// </summary>
        internal void ContinueParsing(ParseResult result, XElement node, AttributesHelper attributes, IEnumerable<XElement> children, String baseUri, Boolean addImageQuery)
        {
            this.ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // 0-n children
            foreach (XElement child in children)
            {
                try
                {
                    this.HandleChild(result, child, baseUri, addImageQuery);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }

        protected void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, String baseUri, Boolean addImageQuery)
        {
            if (this.Context != NotificationType.Toast)
            {
                // hint-textStacking is optional
                HintTextStacking hintTextStacking;
                if (this.TryParseEnum(result, attributes, ATTR_TEXT_STACKING, out hintTextStacking))
                    this.HintTextStacking = hintTextStacking;
            }
        }

        private Boolean _hasWarnedAboutTooManyTextElements;
        private Boolean _hasWarnedAboutTooManyInlineImages;
        protected void HandleChild(ParseResult result, XElement child, String baseUri, Boolean addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    var text = new AdaptiveTextField(this.Context, this.SupportedFeatures);
                    text.Parse(result, child, true);

                    if (!result.IsOkForRender())
                        return;

                    if (text.Placement == TextPlacement.Inline && this.Context == NotificationType.Toast)
                    {
                        if (this.Children.OfType<AdaptiveTextField>().Where(i => i.Placement == TextPlacement.Inline).Count() >= 3)
                        {
                            if (!this._hasWarnedAboutTooManyTextElements)
                            {
                                String warningMessage;

                                if (this.SupportedFeatures.RS1_Style_Toasts)
                                {
                                    warningMessage = "Toasts can only display up to 3 text elements outside of a group/subgroup. Place your additional text elements inside a group/subgroup.";
                                }
                                else
                                {
                                    warningMessage = "Toasts can only display up to 3 text elements.";
                                }

                                result.AddWarning(warningMessage, GetErrorPositionInfo(child));
                                this._hasWarnedAboutTooManyTextElements = true;
                            }

                            return;
                        }
                    }

                    this.Add(text);

                    return;



                case "image":

                    var image = new AdaptiveImage(this.Context, this.SupportedFeatures);
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                        return;

                    if (this.Context == NotificationType.Toast
                        && !this.SupportedFeatures.AdaptiveToasts
                        && image.Placement == Placement.Inline
                        )
                    {
                        if (this.Children.OfType<AdaptiveImage>().Where(i => i.Placement == Placement.Inline).Count() >= 6)
                        {
                            if (!this._hasWarnedAboutTooManyInlineImages)
                            {
                                result.AddWarning("Toasts can only display up to 6 inline images.", GetErrorPositionInfo(child));
                                this._hasWarnedAboutTooManyInlineImages = true;
                            }

                            return;
                        }
                    }

                    this.Add(image);

                    return;



                case "group":

                    if (this.Context != NotificationType.Toast
                        || this.SupportedFeatures.AdaptiveToasts
                        )
                    {
                        var group = new AdaptiveGroup(this.Context, this.SupportedFeatures);
                        group.Parse(result, child, baseUri, addImageQuery);

                        if (!result.IsOkForRender())
                            return;

                        if (group != null)
                            this.Add(group);

                        return;
                    }

                    break;



                case "progress":

                    if (this.Context == NotificationType.Toast && this.SupportedFeatures.ToastProgressBar)
                    {
                        var progress = new AdaptiveProgress(this.Context, this.SupportedFeatures);
                        progress.Parse(result, child);

                        if (!result.IsOkForRender())
                        {
                            return;
                        }

                        this.Add(progress);
                        return;
                    }
                    break;
            }

            result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under binding element. It will be ignored.", GetErrorPositionInfo(child));
        }

        private void Add(AdaptiveTextField child)
        {
            this.AddHelper(child);
        }

        private void Add(AdaptiveImage child)
        {
            this.AddHelper(child);
        }

        private void Add(AdaptiveGroup child)
        {
            this.AddHelper(child);
        }

        private void Add(AdaptiveProgress child)
        {
            this.AddHelper(child);
        }

        private void AddHelper(AdaptiveChildElement child)
        {
            this._children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return this.Children;
        }

        protected override IEnumerable<String> GetAttributesNotSupportedByVisualizer()
        {
            return new String[] { };
        }

        internal void RemoveChildAt(Int32 index)
        {
            this._children.RemoveAt(index);
        }
    }
}
