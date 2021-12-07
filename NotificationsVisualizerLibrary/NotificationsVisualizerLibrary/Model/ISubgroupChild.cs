using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateVisualizerLibrary.Model
{
    /// <summary>
    /// Valid children are <see cref="TemplateVisualizerLibrary.Model.TextField"/> and <see cref="TemplateVisualizerLibrary.Model.Image"/>
    /// </summary>
    internal interface ISubgroupChild : AdaptiveChildElement
    {
    }
}
