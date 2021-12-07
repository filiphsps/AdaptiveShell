using NotificationsVisualizerLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace NotificationsVisualizerLibrary
{
    /// <summary>
    /// Updates a badge overlay on the specific tile that the updater is bound to.
    /// </summary>
    public sealed class PreviewBadgeUpdater
    {
        private PreviewTile _previewTile;

        internal PreviewBadgeUpdater(PreviewTile previewTile)
        {
            _previewTile = previewTile;
        }

        /// <summary>
        /// Removes the badge from the tile that the updater is bound to.
        /// </summary>
        public void Clear()
        {
            _previewTile.SetBadge(BadgeValue.Default());
        }

        /// <summary>
        /// Applies a change to the badge's glyph or number.
        /// </summary>
        /// <param name="badgeNotification"></param>
        public void Update(BadgeNotification badgeNotification)
        {
            if (badgeNotification.Content == null)
                throw new NullReferenceException("badgeNotification.Content must be provided.");

            IXmlNode badgeNode = badgeNotification.Content.SelectSingleNode("/badge");
            if (badgeNode == null)
                throw new NullReferenceException("<badge> element must be provided in the xml");

            var valueAttribute = badgeNode.Attributes.GetNamedItem("value");
            if (valueAttribute == null)
                throw new NullReferenceException("<badge> element must have a \"value\" attribute specified.");

            _previewTile.SetBadge(BadgeValue.Parse(valueAttribute.InnerText));
        }
    }
}
