using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Parsers {
    public sealed class ErrorPositionInfo {
        public Int32 LineNumber { get; set; }

        internal static readonly ErrorPositionInfo Default = new ErrorPositionInfo() {
            LineNumber = 0
        };

        public Boolean Equals(ErrorPositionInfo other) {
            return this.LineNumber == other.LineNumber;
        }
    }

    public sealed class ParseError {
        public String Message { get; private set; }

        public ParseErrorType Type { get; private set; }

        public ErrorPositionInfo Position { get; private set; }

        public ParseError(ParseErrorType type, String message) {
            this.Type = type;
            this.Message = message;
        }

        public ParseError(ParseErrorType type, String message, ErrorPositionInfo position) {
            this.Type = type;
            this.Message = message;
            this.Position = position;
        }

        public Boolean Equals(ParseError other) {
            return this.Message.Equals(other.Message)
                   && this.Type == other.Type
                   && ((this.Position == null && other.Position == null) || (this.Position != null && other.Position != null && this.Position.Equals(other.Position)));
        }
    }

    public enum ParseErrorType {
        Error, ErrorButRenderAllowed, Warning
    }
}
