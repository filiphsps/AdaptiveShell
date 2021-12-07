using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Parsers
{
    public sealed class ErrorPositionInfo
    {
        public int LineNumber { get; set; }

        internal static readonly ErrorPositionInfo Default = new ErrorPositionInfo()
        {
            LineNumber = 0
        };

        public bool Equals(ErrorPositionInfo other)
        {
            return LineNumber == other.LineNumber;
        }
    }

    public sealed class ParseError
    {
        public string Message { get; private set; }

        public ParseErrorType Type { get; private set; }

        public ErrorPositionInfo Position { get; private set; }

        public ParseError(ParseErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public ParseError(ParseErrorType type, string message, ErrorPositionInfo position)
        {
            Type = type;
            Message = message;
            Position = position;
        }

        public bool Equals(ParseError other)
        {
            return Message.Equals(other.Message)
                && Type == other.Type
                && ((Position == null && other.Position == null) || (Position != null && other.Position != null && Position.Equals(other.Position)));
        }
    }

    public enum ParseErrorType
    {
        Error, ErrorButRenderAllowed, Warning
    }
}
