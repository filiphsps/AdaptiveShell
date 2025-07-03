using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsVisualizerLibrary.Model;

namespace NotificationsVisualizerLibrary.Parsers
{
    public interface ITemplateParser
    {
        Boolean IsMatch(String text);

        ParseResult Parse(String text);
    }
}
