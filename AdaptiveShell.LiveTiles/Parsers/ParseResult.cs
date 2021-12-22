using AdaptiveShell.LiveTiles.Models;
using AdaptiveShell.LiveTiles.Models.BaseElements;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Parsers {
    public sealed class ParseResult {
        /// <summary>
        /// Only visible internally. The structure of the parsed adaptive tile.
        /// </summary>
        internal Tile Tile { get; set; }

        internal AdaptiveContainer AdaptiveContent { get; set; }

        /// <summary>
        /// Errors found when parsing the payload. If there were only warnings, like specifying two of the same bindings, the tile is still updated.
        /// </summary>
        public IList<ParseError> Errors { get; private set; } = new ObservableCollection<ParseError>();

        /// <summary>
        /// Returns true if there's no fatal error, meaning that the tile rendering can proceed (but there still may be warnings or critical system errors that Windows won't allow).
        /// </summary>
        /// <returns></returns>
        public Boolean IsOkForRender() {
            return this.Errors.All(i => i.Type != ParseErrorType.Error);
        }

        public static ParseResult GenerateForError(ParseError error) {
            var result = new ParseResult();
            result.Errors.Add(error);
            return result;
        }

        public void AddError(String message, ErrorPositionInfo errorPositionInfo) {
            var error = new ParseError(ParseErrorType.Error, message, errorPositionInfo);

            this.Add(error);
        }

        public void AddErrorButRenderAllowed(String message, ErrorPositionInfo errorPositionInfo) {
            var error = new ParseError(ParseErrorType.ErrorButRenderAllowed, message, errorPositionInfo);

            this.Add(error);
        }

        public void AddWarning(String message, ErrorPositionInfo errorPositionInfo) {
            this.Add(new ParseError(ParseErrorType.Warning, message, errorPositionInfo));
        }

        public void Add(ParseError error) {
            if (error.Type == ParseErrorType.Error) {
                for (Int32 i = 0; i < this.Errors.Count; i++)
                    if (this.Errors[i].Type != ParseErrorType.Error) {
                        this.Errors.Insert(i, error);
                        return;
                    }
            } else if (error.Type == ParseErrorType.ErrorButRenderAllowed) {
                for (Int32 i = 0; i < this.Errors.Count; i++)
                    if (this.Errors[i].Type == ParseErrorType.Warning) {
                        this.Errors.Insert(i, error);
                        return;
                    }
            }

            this.Errors.Add(error);
        }

        /// <summary>
        /// Returns a string of C# code using the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public String GenerateCSharpCode() {
            if (this.Tile != null) {
                return WrapTileForCSharp(this.Tile.ConvertToObject().ToString());
            }

            throw new NotImplementedException();
        }

        private static String WrapTileForCSharp(String original) {
            return $"var tileContent = {original};";
        }

        private static String WrapToastForCSharp(String original) {
            return $"var toastContent = {original};";
        }
    }
}
