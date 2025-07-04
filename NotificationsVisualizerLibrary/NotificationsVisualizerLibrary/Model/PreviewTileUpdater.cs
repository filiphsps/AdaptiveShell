using NotificationsVisualizerLibrary.Parsers;
using Windows.UI.Notifications;

namespace NotificationsVisualizerLibrary
{
    /// <summary>
    /// Changes the content of the specific tile that the updater is bound to.
    /// </summary>
    public sealed class PreviewTileUpdater
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();
        private PreviewTile _previewTile;

        internal PreviewTileUpdater(PreviewTile previewTile)
        {
            this._previewTile = previewTile;
        }

        /// <summary>
        /// Updates the tile with the notification. If the tile payload has an error, the tile will not be updated. The first error, or multiple warnings, will be reported via the returned ParseResult.
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public ParseResult Update(TileNotification notification)
        {
            // TODO: handle notification queue
            ParseResult result = _parser.Parse(notification.Content.GetXml(), this._previewTile.CurrFeatureSet);

            if (result.IsOkForRender())
            {
                this._previewTile.Show(result.Tile, true);
            }

            return result;
        }

        /// <summary>
        /// Removes all updates and causes the tile to display its default contentas declared in the tile properties.
        /// </summary>
        public void Clear()
        {
            this._previewTile.Show(null, true);
        }
    }
}
