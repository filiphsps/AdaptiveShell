using System;
using System.Collections.Generic;

namespace NotificationsVisualizerLibrary.Model
{
    internal class ObjectModelObjectFromStaticMethod : IObjectModelValue
    {
        public const String ROOT_JS_NAMESPACE = "notifLib";

        public String ClassName { get; set; }

        public String MethodName { get; set; }

        public ObjectModelObjectFromStaticMethod(String className, String methodName, params IObjectModelValue[] parameterValues)
        {
            this.ClassName = className;
            this.MethodName = methodName;

            foreach (var cVal in parameterValues)
            {
                this.ParameterValues.Add(cVal);
            }
        }

        public List<IObjectModelValue> ParameterValues { get; private set; } = new List<IObjectModelValue>();

        public override String ToString()
        {
            return $"{this.ClassName}.{this.MethodName}({String.Join(", ", this.ParameterValues)})";
        }

        public String ToCPlusPlusString(List<String> declaredVars)
        {
            return $"{this.ClassName}::{this.MethodName}({String.Join(", ", this.ParameterValues)})";
        }

        public String ToJavascriptString()
        {
            return this.ToJavascriptString(new List<String>());
        }

        public String ToJavascriptString(List<String> declaredVars)
        {
            return $"{ROOT_JS_NAMESPACE}.{this.ClassName}.{ObjectModelObject.ToLowerCamelCase(this.MethodName)}({String.Join(", ", this.ParameterValues)})";
        }
    }
}
