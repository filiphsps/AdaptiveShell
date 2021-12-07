using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TemplateVisualizerLibrary.Parsers;

namespace TemplateVisualizerLibrary.Model.TileElements
{
    internal class TileGroup : Group
    {
        protected override void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            if (child.IsType("subgroup"))
            {
                TileSubgroup subgroup = new TileSubgroup();
                subgroup.Parse(result, child, baseUri, addImageQuery);

                if (!result.IsOkForRender())
                    throw new IncompleteElementException();

                if (subgroup != null)
                    this.Add(subgroup);
            }

            else
            {
                result.AddError($@"Invalid child ""{child.Name.LocalName}"" found in a group. Groups can only contain subgroups.", GetErrorPositionInfo(child));
            }
        }
    }
}
