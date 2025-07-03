using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model;
using System.Collections.ObjectModel;

namespace NotificationsVisualizerLibrary.Parsers
{

    public sealed class ParseResult
    {
        /// <summary>
        /// Only visible internally. The structure of the parsed adaptive tile.
        /// </summary>
        internal Model.Tile Tile { get; set; }

        internal Toast Toast { get; set; }

        internal AdaptiveContainer AdaptiveContent { get; set; }

        /// <summary>
        /// Errors found when parsing the payload. If there were only warnings, like specifying two of the same bindings, the tile is still updated.
        /// </summary>
        public IList<ParseError> Errors { get; private set; } = new ObservableCollection<ParseError>();

        /// <summary>
        /// Returns true if there's no fatal error, meaning that the tile rendering can proceed (but there still may be warnings or critical system errors that Windows won't allow).
        /// </summary>
        /// <returns></returns>
        public Boolean IsOkForRender()
        {
            return !this.Errors.Any(i => i.Type == ParseErrorType.Error);
        }

        /// <summary>
        /// Returns true if the content uses data binding
        /// </summary>
        /// <returns></returns>
        public Boolean UsesDataBinding()
        {
            return (this.Toast != null && this.Toast.AnyUseDataBinding())
                || (this.AdaptiveContent != null && this.AdaptiveContent.AnyUseDataBinding());
        }

        public static ParseResult GenerateForError(ParseError error)
        {
            var result = new ParseResult();
            result.Errors.Add(error);
            return result;
        }

        public void AddError(String message, ErrorPositionInfo errorPositionInfo)
        {
            var error = new ParseError(ParseErrorType.Error, message, errorPositionInfo);

            this.Add(error);
        }

        public void AddErrorButRenderAllowed(String message, ErrorPositionInfo errorPositionInfo)
        {
            var error = new ParseError(ParseErrorType.ErrorButRenderAllowed, message, errorPositionInfo);

            this.Add(error);
        }

        public void AddWarning(String message, ErrorPositionInfo errorPositionInfo)
        {
            this.Add(new ParseError(ParseErrorType.Warning, message, errorPositionInfo));
        }

        public void Add(ParseError error)
        {
            if (error.Type == ParseErrorType.Error)
            {
                for (Int32 i = 0; i < this.Errors.Count; i++)
                    if (this.Errors[i].Type != ParseErrorType.Error)
                    {
                        this.Errors.Insert(i, error);
                        return;
                    }
            }
            else if (error.Type == ParseErrorType.ErrorButRenderAllowed)
            {
                for (Int32 i = 0; i < this.Errors.Count; i++)
                    if (this.Errors[i].Type == ParseErrorType.Warning)
                    {
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
        public String GenerateCSharpCode()
        {
            if (this.Toast != null)
            {
                return WrapToastForCSharp(this.Toast.ConvertToObject().ToString());
            }
            if (this.Tile != null)
            {
                return WrapTileForCSharp(this.Tile.ConvertToObject().ToString());
            }
            return "Not implemented";
        }

        private static String WrapTileForCSharp(String original)
        {
            return $"var tileContent = {original};";
        }

        private static String WrapToastForCSharp(String original)
        {
            return $"var toastContent = {original};";
        }

        /// <summary>
        /// Returns a string of Javascript code usign the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public String GenerateJavascriptCode()
        {
            if (this.Toast != null)
            {
                return this.IncludeJsNamespaceDeclaration(
                    this.Toast.ConvertToObject().ToJavascriptString());
            }
            if (this.Tile != null)
            {
                return this.IncludeJsNamespaceDeclaration(
                    this.Tile.ConvertToObject().ToJavascriptString());
            }
            return "Not implemented";
        }

        /// <summary>
        /// Returns a string of C++ code usign the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public String GenerateCPlusPlusCode()
        {
            if (this.Toast != null)
            {
                return this.Toast.ConvertToObject().ToCPlusPlusString();
            }
            if (this.Tile != null)
            {
                return this.Tile.ConvertToObject().ToCPlusPlusString();
            }
            return "Not implemented";
        }

        private String IncludeJsNamespaceDeclaration(String code)
        {
            return $"var {ObjectModelObject.ROOT_JS_NAMESPACE} = Microsoft.Toolkit.Uwp.Notifications;\n\n{code}";
        }
    }
}
