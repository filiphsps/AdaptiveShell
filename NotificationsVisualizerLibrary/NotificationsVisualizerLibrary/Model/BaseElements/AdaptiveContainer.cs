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

        private static readonly string ATTR_TEXT_STACKING = "hint-textStacking";

        private List<AdaptiveChildElement> _children = new List<AdaptiveChildElement>();
        internal IReadOnlyList<AdaptiveChildElement> Children
        {
            get { return _children; }
        }

        public void SwapChildren(IEnumerable<AdaptiveChildElement> newChildren)
        {
            _children.Clear();
            _children.AddRange(newChildren);
        }

        public HintTextStacking HintTextStacking { get; set; } = HintTextStacking.Default;

        /// <summary>
        /// Parses a node that has already been parsed some (for example, the Binding actually parses the node first, and then this parses some extra stuff. Binding is responsible for showing warnings about unknown unprocessed attributes after this method has parsed through what it understands).
        /// </summary>
        internal void ContinueParsing(ParseResult result, XElement node, AttributesHelper attributes, IEnumerable<XElement> children, string baseUri, bool addImageQuery)
        {
            ParseKnownAttributes(node, attributes, result, baseUri, addImageQuery);

            // 0-n children
            foreach (XElement child in children)
            {
                try
                {
                    HandleChild(result, child, baseUri, addImageQuery);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }

        protected void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result, string baseUri, bool addImageQuery)
        {
            if (Context != NotificationType.Toast)
            {
                // hint-textStacking is optional
                HintTextStacking hintTextStacking;
                if (TryParseEnum(result, attributes, ATTR_TEXT_STACKING, out hintTextStacking))
                    this.HintTextStacking = hintTextStacking;
            }
        }

        private bool _hasWarnedAboutTooManyTextElements;
        private bool _hasWarnedAboutTooManyInlineImages;
        protected void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    AdaptiveTextField text = new AdaptiveTextField(Context, SupportedFeatures);
                    text.Parse(result, child, true);

                    if (!result.IsOkForRender())
                        return;

                    if (text.Placement == TextPlacement.Inline && Context == NotificationType.Toast)
                    {
                        if (Children.OfType<AdaptiveTextField>().Where(i => i.Placement == TextPlacement.Inline).Count() >= 3)
                        {
                            if (!_hasWarnedAboutTooManyTextElements)
                            {
                                string warningMessage;

                                if (SupportedFeatures.RS1_Style_Toasts)
                                {
                                    warningMessage = "Toasts can only display up to 3 text elements outside of a group/subgroup. Place your additional text elements inside a group/subgroup.";
                                }
                                else
                                {
                                    warningMessage = "Toasts can only display up to 3 text elements.";
                                }

                                result.AddWarning(warningMessage, GetErrorPositionInfo(child));
                                _hasWarnedAboutTooManyTextElements = true;
                            }

                            return;
                        }
                    }

                    this.Add(text);

                    return;



                case "image":

                    AdaptiveImage image = new AdaptiveImage(Context, SupportedFeatures);
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                        return;

                    if (Context == NotificationType.Toast
                        && !SupportedFeatures.AdaptiveToasts
                        && image.Placement == Placement.Inline
                        )
                    {
                        if (Children.OfType<AdaptiveImage>().Where(i => i.Placement == Placement.Inline).Count() >= 6)
                        {
                            if (!_hasWarnedAboutTooManyInlineImages)
                            {
                                result.AddWarning("Toasts can only display up to 6 inline images.", GetErrorPositionInfo(child));
                                _hasWarnedAboutTooManyInlineImages = true;
                            }

                            return;
                        }
                    }

                    this.Add(image);

                    return;



                case "group":

                    if (Context != NotificationType.Toast
                        || SupportedFeatures.AdaptiveToasts
                        )
                    {
                        AdaptiveGroup group = new AdaptiveGroup(Context, SupportedFeatures);
                        group.Parse(result, child, baseUri, addImageQuery);

                        if (!result.IsOkForRender())
                            return;

                        if (group != null)
                            this.Add(group);

                        return;
                    }

                    break;



                case "progress":

                    if (Context == NotificationType.Toast && SupportedFeatures.ToastProgressBar)
                    {
                        AdaptiveProgress progress = new AdaptiveProgress(Context, SupportedFeatures);
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
            AddHelper(child);
        }

        private void Add(AdaptiveImage child)
        {
            AddHelper(child);
        }

        private void Add(AdaptiveGroup child)
        {
            AddHelper(child);
        }

        private void Add(AdaptiveProgress child)
        {
            AddHelper(child);
        }

        private void AddHelper(AdaptiveChildElement child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            return Children;
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal void RemoveChildAt(int index)
        {
            _children.RemoveAt(index);
        }
    }
}
