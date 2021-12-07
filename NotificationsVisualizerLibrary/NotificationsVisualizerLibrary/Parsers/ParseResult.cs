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
        public bool IsOkForRender()
        {
            return !Errors.Any(i => i.Type == ParseErrorType.Error);
        }

        /// <summary>
        /// Returns true if the content uses data binding
        /// </summary>
        /// <returns></returns>
        public bool UsesDataBinding()
        {
            return (Toast != null && Toast.AnyUseDataBinding())
                || (AdaptiveContent != null && AdaptiveContent.AnyUseDataBinding());
        }

        public static ParseResult GenerateForError(ParseError error)
        {
            ParseResult result = new ParseResult();
            result.Errors.Add(error);
            return result;
        }

        public void AddError(string message, ErrorPositionInfo errorPositionInfo)
        {
            ParseError error = new ParseError(ParseErrorType.Error, message, errorPositionInfo);

            Add(error);
        }

        public void AddErrorButRenderAllowed(string message, ErrorPositionInfo errorPositionInfo)
        {
            ParseError error = new ParseError(ParseErrorType.ErrorButRenderAllowed, message, errorPositionInfo);

            Add(error);
        }

        public void AddWarning(string message, ErrorPositionInfo errorPositionInfo)
        {
            Add(new ParseError(ParseErrorType.Warning, message, errorPositionInfo));
        }

        public void Add(ParseError error)
        {
            if (error.Type == ParseErrorType.Error)
            {
                for (int i = 0; i < Errors.Count; i++)
                    if (Errors[i].Type != ParseErrorType.Error)
                    {
                        Errors.Insert(i, error);
                        return;
                    }
            }
            else if (error.Type == ParseErrorType.ErrorButRenderAllowed)
            {
                for (int i = 0; i < Errors.Count; i++)
                    if (Errors[i].Type == ParseErrorType.Warning)
                    {
                        Errors.Insert(i, error);
                        return;
                    }
            }

            Errors.Add(error);
        }

        /// <summary>
        /// Returns a string of C# code using the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public string GenerateCSharpCode()
        {
            if (Toast != null)
            {
                return WrapToastForCSharp(Toast.ConvertToObject().ToString());
            }
            if (Tile != null)
            {
                return WrapTileForCSharp(Tile.ConvertToObject().ToString());
            }
            return "Not implemented";
        }

        private static string WrapTileForCSharp(string original)
        {
            return $"var tileContent = {original};";
        }

        private static string WrapToastForCSharp(string original)
        {
            return $"var toastContent = {original};";
        }

        /// <summary>
        /// Returns a string of Javascript code usign the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public string GenerateJavascriptCode()
        {
            if (Toast != null)
            {
                return IncludeJsNamespaceDeclaration(
                    Toast.ConvertToObject().ToJavascriptString());
            }
            if (Tile != null)
            {
                return IncludeJsNamespaceDeclaration(
                    Tile.ConvertToObject().ToJavascriptString());
            }
            return "Not implemented";
        }

        /// <summary>
        /// Returns a string of C++ code usign the Notifications library that represents the content of the notification.
        /// </summary>
        /// <returns></returns>
        public string GenerateCPlusPlusCode()
        {
            if (Toast != null)
            {
                return Toast.ConvertToObject().ToCPlusPlusString();
            }
            if (Tile != null)
            {
                return Tile.ConvertToObject().ToCPlusPlusString();
            }
            return "Not implemented";
        }

        private string IncludeJsNamespaceDeclaration(string code)
        {
            return $"var {ObjectModelObject.ROOT_JS_NAMESPACE} = Microsoft.Toolkit.Uwp.Notifications;\n\n{code}";
        }
    }
}
