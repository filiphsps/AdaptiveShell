using System;

namespace NotificationsVisualizerLibrary.Parsers
{
    public interface ITemplateParser
    {
        Boolean IsMatch(String text);

        ParseResult Parse(String text);
    }
}
