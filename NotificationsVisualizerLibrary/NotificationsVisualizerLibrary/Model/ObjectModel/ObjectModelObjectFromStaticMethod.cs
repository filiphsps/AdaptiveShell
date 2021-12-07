using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelObjectFromStaticMethod : IObjectModelValue
    {
        public const string ROOT_JS_NAMESPACE = "notifLib";

        public string ClassName { get; set; }

        public string MethodName { get; set; }

        public ObjectModelObjectFromStaticMethod(string className, string methodName, params IObjectModelValue[] parameterValues)
        {
            ClassName = className;
            MethodName = methodName;

            foreach (var cVal in parameterValues)
            {
                ParameterValues.Add(cVal);
            }
        }

        public List<IObjectModelValue> ParameterValues { get; private set; } = new List<IObjectModelValue>();

        public override string ToString()
        {
            return $"{ClassName}.{MethodName}({string.Join(", ", ParameterValues)})";
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return $"{ClassName}::{MethodName}({string.Join(", ", ParameterValues)})";
        }

        public string ToJavascriptString()
        {
            return ToJavascriptString(new List<string>());
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return $"{ROOT_JS_NAMESPACE}.{ClassName}.{ObjectModelObject.ToLowerCamelCase(MethodName)}({string.Join(", ", ParameterValues)})";
        }
    }
}
