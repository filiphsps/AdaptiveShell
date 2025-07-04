using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Model;

namespace NotificationsVisualizerLibrary.Parsers
{
    /// <summary>
    /// Exception thrown when required attributes from an element is missing, and thus the element cannot exist / should not be added to the parsed model. Error should have already been added, this is just used specifically to make sure that parent doesn't add the item.
    /// </summary>
    internal class IncompleteElementException : Exception
    {

    }

    internal sealed partial class AttributesHelper : List<XAttribute>
    {
        public AttributesHelper(IEnumerable<XAttribute> attributes) : base(attributes) { }

        public XAttribute PopAttribute(String name, Boolean caseSensitive = true)
        {
            XAttribute answer = this.FirstOrDefault(i => i.IsType(name, caseSensitive));

            if (answer != null)
                base.Remove(answer);

            return answer;
        }

        public String PopAttributeValue(String name, Boolean caseSensitive = true)
        {
            XAttribute attr = this.PopAttribute(name, caseSensitive);

            return attr?.Value;
        }
    }

    internal static class ExtensionHelpers
    {
        internal static XElement FirstElementOrDefault(this XContainer currNode, String name)
        {
            return currNode.Elements().FirstOrDefault(i => i.IsType(name));
        }

        internal static Boolean IsType(this XElement node, String name)
        {
            return node.Name.LocalName.Equals(name, StringComparison.CurrentCultureIgnoreCase);
        }

        internal static Boolean IsType(this XAttribute node, String name, Boolean caseSensitive = true)
        {
            return node.Name.LocalName.Equals(name, caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public sealed class XmlTemplateParser
    {
        private enum NotificationType
        {
            Tile
            , Toast
            , AdaptiveContent
        }

        private static readonly String ATTR_TILE_VERSION = "version";

        private static readonly String ATTR_VISUAL_VERSION = "version";
        private static readonly String ATTR_VISUAL_BRANDING = "branding";
        private static readonly String ATTR_VISUAL_DISPLAY_NAME = "displayName";
        private static readonly String ATTR_VISUAL_LANG = "lang";
        private static readonly String ATTR_VISUAL_BASEURI = "baseUri";
        private static readonly String ATTR_VISUAL_ADDIMAGEQUERY = "addImageQuery";
        private static readonly String ATTR_VISUAL_CONTENTID = "contentId";

        private static readonly String ATTR_BINDING_TEMPLATE = "template";
        private static readonly String ATTR_BINDING_BRANDING = "branding";
        private static readonly String ATTR_BINDING_DISPLAY_NAME = "displayName";
        private static readonly String ATTR_BINDING_FALLBACK = "fallback";
        private static readonly String ATTR_BINDING_LANG = "lang";
        private static readonly String ATTR_BINDING_BASEURI = "baseUri";
        private static readonly String ATTR_BINDING_ADDIMAGEQUERY = "addImageQuery";
        private static readonly String ATTR_BINDING_CONTENTID = "contentId";
        private static readonly String ATTR_BINDING_HINT_TEXT_STACKING = "hint-textStacking";
        private static readonly String ATTR_BINDING_HINT_OVERLAY = "hint-overlay";

        private static readonly String ATTR_SUBGROUP_HINT_WEIGHT = "hint-weight";
        private static readonly String ATTR_SUBGROUP_HINT_TEXT_STACKING = "hint-textStacking";


        private static readonly String ATTR_TEXT_HINT_STYLE = "hint-style";
        private static readonly String ATTR_TEXT_HINT_WRAP = "hint-wrap";
        private static readonly String ATTR_TEXT_HINT_MAX_LINES = "hint-maxLines";
        private static readonly String ATTR_TEXT_HINT_MIN_LINES = "hint-minLines";
        private static readonly String ATTR_TEXT_HINT_ALIGN = "hint-align";
        private static readonly String ATTR_TEXT_LANG = "lang";


        private static readonly String ATTR_IMAGE_SRC = "src";
        private static readonly String ATTR_IMAGE_PLACEMENT = "placement";
        private static readonly String ATTR_IMAGE_ALT = "alt";
        private static readonly String ATTR_IMAGE_ADDIMAGEQUERY = "addImageQuery";
        private static readonly String ATTR_IMAGE_HINT_CROP = "hint-crop";
        private static readonly String ATTR_IMAGE_HINT_REMOVE_MARGIN = "hint-removeMargin";
        private static readonly String ATTR_IMAGE_HINT_ALIGN = "hint-align";

        /// <summary>
        /// 5 KB (5120 bytes) is too much, only up to 5119 bytes are allowed
        /// </summary>
        private static readonly Int32 PAYLOAD_SIZE_LIMIT = 5119;

        public Boolean IsMatch(String text)
        {
            return text.Trim().StartsWith("<");
        }

        internal ParseResult Parse(String text, FeatureSet supportedFeatures)
        {
            return this.Parse(text, NotificationType.Tile, supportedFeatures);
        }

        internal ParseResult ParseToast(String text, FeatureSet supportedFeatures)
        {
            return this.Parse(text, NotificationType.Toast, supportedFeatures);
        }

        internal ParseResult ParseAdaptiveContent(String text, FeatureSet supportedFeatures)
        {
            return this.Parse(text, NotificationType.AdaptiveContent, supportedFeatures);
        }

        private ParseResult Parse(String text, NotificationType type, FeatureSet supportedFeatures)
        {
            XDocument doc;

            try { doc = XDocument.Load(new StringReader(text), LoadOptions.SetLineInfo); }
            catch { return ParseResult.GenerateForError(new ParseError(ParseErrorType.Error, "Invalid XML", ErrorPositionInfo.Default)); }

            var result = new ParseResult();

            HandlePayloadSizeError(result, text);

            switch (type)
            {
                case NotificationType.Tile:
                    return this.ParseTile(result, doc, supportedFeatures);

                case NotificationType.Toast:
                    return this.ParseToast(result, doc, supportedFeatures);

                case NotificationType.AdaptiveContent:
                    return this.ParseAdaptiveContent(result, doc, supportedFeatures);

                default:
                    throw new NotImplementedException();
            }
        }


        private ParseResult ParseTile(ParseResult result, XDocument doc, FeatureSet supportedFeatures)
        {
            this.ParseTileHelper(result, doc, supportedFeatures);

            return result;
        }

        private ParseResult ParseToast(ParseResult result, XDocument doc, FeatureSet supportedFeatures)
        {
            this.ParseToastHelper(result, doc, supportedFeatures);

            return result;
        }

        private ParseResult ParseAdaptiveContent(ParseResult result, XDocument doc, FeatureSet supportedFeatures)
        {
            this.ParseAdaptiveContentHelper(result, doc.Root, supportedFeatures);

            return result;
        }

        private static void HandlePayloadSizeError(ParseResult result, String xml)
        {
            Int32 payloadSize = System.Text.Encoding.UTF8.GetByteCount(xml);

            if (payloadSize > PAYLOAD_SIZE_LIMIT)
                result.AddErrorButRenderAllowed($"Your payload exceeds the 5 KB size limit (it is {payloadSize.ToString("N0")} Bytes). Please reduce your payload, or else Windows will not display it.", ErrorPositionInfo.Default);
        }



        private void ParseToastHelper(ParseResult result, XContainer parentNode, FeatureSet supportedFeatures)
        {
            XElement node = parentNode.FirstElementOrDefault("toast");

            if (node == null)
            {
                result.AddError("toast element was not found", ErrorPositionInfo.Default);
                return;
            }

            result.Toast = new Toast(supportedFeatures);

            try
            {
                result.Toast.Parse(result, node);
            }

            catch (IncompleteElementException) { }
        }

        private void ParseAdaptiveContentHelper(ParseResult result, XElement parentNode, FeatureSet supportedFeatures)
        {
            result.AdaptiveContent = new AdaptiveContainer(Model.NotificationType.AdaptiveContent, supportedFeatures);

            try
            {
                var attributes = new AttributesHelper(parentNode.Attributes());
                result.AdaptiveContent.ContinueParsing(result, parentNode, attributes, parentNode.Elements(), null, false);
            }

            catch (IncompleteElementException) { }
        }

        private void ParseTileHelper(ParseResult result, XContainer parentNode, FeatureSet supportedFeatures)
        {
            XElement node = parentNode.FirstElementOrDefault("tile");

            if (node == null)
            {
                result.AddError("tile element was not found", ErrorPositionInfo.Default);
                return;
            }

            if (!EnsureNodeOnlyHasElementsAsChildren(result, node))
                return;

            result.Tile = new Model.Tile(supportedFeatures);

            var attributes = new AttributesHelper(node.Attributes());

            // tile doesn't have any attributes

            AddWarningsForUnknownAttributes(result, attributes);

            this.ParseVisual(result, result.Tile.Visual, node);
        }

        private void ParseVisual(ParseResult result, Visual into, XElement parentNode)
        {
            XElement node = parentNode.FirstElementOrDefault("visual");

            if (node == null)
            {
                result.AddError("visual element was not found", GetErrorPositionInfo(parentNode));
                return;
            }

            try
            {
                into.Parse(result, node);
            }

            catch (IncompleteElementException) { }
        }

        internal static void AddWarningsForAttributesNotSupportedByVisualizer(ParseResult result, AttributesHelper attributes, params String[] attributeNames)
        {
            foreach (String attrName in attributeNames)
            {
                XAttribute a = attributes.PopAttribute(attrName);
                if (a != null)
                {
                    result.AddWarning($@"The attribute ""{attrName}"" is a valid attribute, but the Visualizer does not support it.", GetErrorPositionInfo(a));
                }
            }
        }

        internal static void AddWarningsForUnknownAttributes(ParseResult result, IEnumerable<XAttribute> unknownAttributes)
        {
            foreach (XAttribute attribute in unknownAttributes)
            {
                result.AddWarning($"Unknown attribute {attribute.Name.LocalName}.", GetErrorPositionInfo(attribute));
            }
        }

        internal static ErrorPositionInfo GetErrorPositionInfo(XObject xObject)
        {
            var i = xObject as IXmlLineInfo;

            return new ErrorPositionInfo()
            {
                LineNumber = i.LineNumber
            };
        }
        
        
        internal static String ConstructUri(String src, String baseUri, Boolean addImageQuery)
        {
            String answer = baseUri + src;
            
            if (addImageQuery)
            {
                if (answer.Contains('?'))
                    answer += "&";
                else
                    answer += "?";

                answer += "ms-scale=100&ms-contrast=standard&ms-lang=en-us";
            }

            return answer;
        }
        
        /// <summary>
        /// Returns true if fine. Sets error and returns false if there's an error.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static Boolean EnsureNodeOnlyHasElementsAsChildren(ParseResult result, XElement node)
        {
            foreach (XNode child in node.Elements())
            {
                switch (child.NodeType)
                {
                    case System.Xml.XmlNodeType.Text:

                        // A text node might just be white space... but if it contains text, that's invalid
                        if (!String.IsNullOrWhiteSpace((child as XText).Value))
                        {
                            result.AddError($@"<{node.Name}> elements cannot have text directly as content. Your text was ""{(child as XText).Value.Trim()}"".", GetErrorPositionInfo(child));
                            return false;
                        }

                        break;
                }
            }

            return true;
        }
        
        private ParseResult Error(String message)
        {
            return ParseResult.GenerateForError(new ParseError(ParseErrorType.Error, message));
        }

        internal static Boolean TryParseEnum<TEnum>(String text, out TEnum answer) where TEnum : struct
        {
            if (text != null)
            {
                foreach (Object e in Enum.GetValues(typeof(TEnum)))
                {
                    if (text.Equals(e.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        answer = (TEnum)e;
                        return true;
                    }
                }
            }

            answer = default(TEnum);
            return false;
        }

        internal static Boolean TryParseEnum<TEnum>(ParseResult result, AttributesHelper attributes, String attributeName, out TEnum answer, Boolean caseSensitive = true) where TEnum : struct
        {
            XAttribute attr = attributes.PopAttribute(attributeName, caseSensitive);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    if (TryParseEnum(attr.Value, out answer))
                        return true;

                    // Couldn't find matching enum
                    result.AddWarning($@"Unknown value ""{attr.Value}"" on {attributeName} attribute", GetErrorPositionInfo(attr));
                }

                // Attribute doesn't have a value
                else
                    result.AddWarning($@"Attribute {attributeName} has no value specified", GetErrorPositionInfo(attr));
            }

            // Attribute isn't found, no warning
            answer = default(TEnum);
            return false;
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Int32 answer)
        {
            XAttribute attr = attributes.PopAttribute(attributeName);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    // If it's an integer
                    if (Int32.TryParse(attr.Value, out answer))
                        return true;

                    // Otherwise warning about not being integer
                    result.AddWarning($@"{attributeName} only accepts integer values, not ""{attr.Value}""", GetErrorPositionInfo(attr));
                }

                // Attribute doesn't have a value
                else
                    result.AddWarning($@"Attribute {attributeName} has no value specified", GetErrorPositionInfo(attr));
            }

            // Attribute isn't found, no warning
            answer = default(Int32);
            return false;
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Double answer)
        {
            XAttribute attr = attributes.PopAttribute(attributeName);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    // If it's a double
                    if (Double.TryParse(attr.Value, out answer))
                        return true;

                    // Otherwise warning about not being double
                    result.AddWarning($@"{attributeName} only accepts double values, not ""{attr.Value}""", GetErrorPositionInfo(attr));
                }

                // Attribute doesn't have a value
                else
                    result.AddWarning($@"Attribute {attributeName} has no value specified", GetErrorPositionInfo(attr));
            }

            // Attribute isn't found, no warning
            answer = default(Double);
            return false;
        }

        internal static Boolean TryParse(ParseResult result, AttributesHelper attributes, String attributeName, out Boolean answer)
        {
            XAttribute attr = attributes.PopAttribute(attributeName);

            // If it has the attribute
            if (attr != null)
            {
                // And the attribute has a value
                if (attr.Value != null)
                {
                    // If it's a bool
                    if (Boolean.TryParse(attr.Value, out answer))
                        return true;

                    // Otherwise warning about not being double
                    result.AddWarning($@"{attributeName} only accepts boolean values, not ""{attr.Value}""", GetErrorPositionInfo(attr));
                }

                // Attribute doesn't have a value
                else
                    result.AddWarning($@"Attribute {attributeName} has no value specified", GetErrorPositionInfo(attr));
            }

            // Attribute isn't found, no warning
            answer = default(Boolean);
            return false;
        }
    }
}
