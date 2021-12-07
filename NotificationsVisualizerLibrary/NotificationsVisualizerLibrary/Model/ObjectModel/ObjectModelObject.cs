using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NotificationsVisualizerLibrary.Model
{
    internal interface IObjectModelValue
    {
        string ToJavascriptString();
        string ToJavascriptString(List<string> declaredVars);
        string ToCPlusPlusString(List<string> declaredVars);
    }

    internal class ObjectModelObject : IObjectModelValue, IEnumerable<KeyValuePair<string, IObjectModelValue>>
    {
        public const string ROOT_JS_NAMESPACE = "notifLib";

        public string Name { get; set; }

        public ObjectModelObject(string name, params IObjectModelValue[] constructorValues)
        {
            Name = name;

            foreach (var cVal in constructorValues)
            {
                ConstructorValues.Add(cVal);
            }
        }

        public List<IObjectModelValue> ConstructorValues { get; private set; } = new List<IObjectModelValue>();

        public List<KeyValuePair<string, IObjectModelValue>> PropertyValues { get; private set; } = new List<KeyValuePair<string, IObjectModelValue>>();

        public void Add(string propertyName, object propertyValue)
        {
            if (propertyValue == null)
            {
                return;
            }

            IObjectModelValue objectPropertyValue = propertyValue as IObjectModelValue;
            if (objectPropertyValue == null)
            {
                objectPropertyValue = Create(propertyValue);
            }

            PropertyValues.Add(new KeyValuePair<string, IObjectModelValue>(propertyName, objectPropertyValue));
        }

        public List<KeyValuePair<ObjectModelBindingPropertyAttribute, string>> Bindings { get; private set; } = new List<KeyValuePair<ObjectModelBindingPropertyAttribute, string>>();

        public void AddBinding(ObjectModelBindingPropertyAttribute attr, string bindingName)
        {
            if (bindingName == null)
            {
                return;
            }

            Bindings.Add(new KeyValuePair<ObjectModelBindingPropertyAttribute, string>(attr, bindingName));
        }

        public override string ToString()
        {
            string answer = $"new {Name}({string.Join(", ", ConstructorValues)})";

            if (PropertyValues.Count > 0 || Bindings.Count > 0)
            {
                string propertyValues = "\n{";
                bool addedProperty = false;

                foreach (var pVal in PropertyValues.Where(i => i.Value != null))
                {
                    var val = pVal.Value.ToString();
                    if (val == null)
                    {
                        continue;
                    }

                    propertyValues += $"\n    {pVal.Key} = ";

                    propertyValues += IndentNewLines(val, 4);

                    propertyValues += ",";

                    addedProperty = true;
                }

                if (Bindings.Count > 0)
                {
                    foreach (var b in Bindings)
                    {
                        propertyValues += $"\n    {b.Key.PropertyName} = new {b.Key.BindableTypeName}({new ObjectModelString(b.Value)}),";
                    }

                    addedProperty = true;
                }

                // Trim the extra ending ","
                if (addedProperty)
                {
                    propertyValues = propertyValues.Substring(0, propertyValues.Length - 1);
                    propertyValues += "\n}";
                    answer += propertyValues;
                }
            }

            return answer;
        }

        public string ToJavascriptString()
        {
            return ToJavascriptString(new List<string>());
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            var jsVar = Var();
            string answer = "";
            string propertyValues = "";

            answer += $"{DeclareJsVar(declaredVars)} = new {ROOT_JS_NAMESPACE}.{Name}({string.Join(", ", ConstructorValues)});";

            // If there's complex properties, we need to construct those first
            foreach (var pVal in PropertyValues.Where(i => i.Value is ObjectModelObject))
            {
                var val = pVal.Value.ToJavascriptString(declaredVars);
                if (val != null)
                {
                    answer += "\n" + val + "\n";
                    answer += $"{jsVar}.{ToLowerCamelCase(pVal.Key)} = {(pVal.Value as ObjectModelObject).Var()};\n";
                }
            }

            if (PropertyValues.Count > 0)
            {
                foreach (var pVal in PropertyValues.Where(i => i.Value != null && !(i.Value is ObjectModelObject)))
                {
                    var val = pVal.Value.ToJavascriptString(declaredVars);
                    if (val == null)
                    {
                        continue;
                    }

                    // If list, we construct an item then add it, then repeat
                    if (pVal.Value is ObjectModelListInitialization)
                    {
                        var listValues = (pVal.Value as ObjectModelListInitialization).Values.Where(i => i != null).ToArray();
                        if (listValues.Length > 0)
                        {
                            propertyValues += "\n";

                            foreach (var listItem in listValues)
                            {
                                // If complex value, need to construct it first
                                if (listItem is ObjectModelObject)
                                {
                                    propertyValues += "\n" + listItem.ToJavascriptString(declaredVars);
                                    propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)}.push({(listItem as ObjectModelObject).Var()});";
                                }
                                else
                                {
                                    propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)}.push({listItem.ToJavascriptString(declaredVars)});";
                                }

                                propertyValues += "\n";
                            }
                        }
                    }

                    else
                    {
                        propertyValues += $"\n{jsVar}.{ToLowerCamelCase(pVal.Key)} = {val};";
                    }
                }
            }

            if (Bindings.Count > 0)
            {
                foreach (var b in Bindings)
                {
                    var enumValue = new ObjectModelEnum(Name + "BindableProperty", b.Key.PropertyName);

                    propertyValues += $"\n{jsVar}.bindings.insert({enumValue.ToJavascriptString()}, {new ObjectModelString(b.Value).ToJavascriptString()});";
                }
            }

            answer += propertyValues;

            return answer;
        }

        public string ToCPlusPlusString()
        {
            return ToCPlusPlusString(new List<string>());
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            var cppVar = Var();
            string answer = "";
            string propertyValues = "";

            answer += $"{DeclareCPlusPlusVar(declaredVars)} = ref new {Name}({string.Join(", ", ConstructorValues)});";

            // If there's complex properties, we need to construct those first
            foreach (var pVal in PropertyValues.Where(i => i.Value is ObjectModelObject))
            {
                var val = pVal.Value.ToCPlusPlusString(declaredVars);
                if (val != null)
                {
                    answer += "\n" + val + "\n";
                    answer += $"{cppVar}->{pVal.Key} = {(pVal.Value as ObjectModelObject).Var()};\n";
                }
            }

            if (PropertyValues.Count > 0)
            {
                foreach (var pVal in PropertyValues.Where(i => i.Value != null && !(i.Value is ObjectModelObject)))
                {
                    var val = pVal.Value.ToCPlusPlusString(declaredVars);
                    if (val == null)
                    {
                        continue;
                    }

                    // If list, we construct an item then add it, then repeat
                    if (pVal.Value is ObjectModelListInitialization)
                    {
                        var listValues = (pVal.Value as ObjectModelListInitialization).Values.Where(i => i != null).ToArray();
                        if (listValues.Length > 0)
                        {
                            propertyValues += "\n";

                            foreach (var listItem in listValues)
                            {
                                // If complex value, need to construct it first
                                if (listItem is ObjectModelObject)
                                {
                                    propertyValues += "\n" + listItem.ToCPlusPlusString(declaredVars);
                                    propertyValues += $"\n{cppVar}->{pVal.Key}->Append({(listItem as ObjectModelObject).Var()});";
                                }
                                else
                                {
                                    propertyValues += $"\n{cppVar}->{pVal.Key}->Append({listItem.ToCPlusPlusString(declaredVars)});";
                                }

                                propertyValues += "\n";
                            }
                        }
                    }

                    else
                    {
                        propertyValues += $"\n{cppVar}->{pVal.Key} = {val};";
                    }
                }
            }

            if (Bindings.Count > 0)
            {
                foreach (var b in Bindings)
                {
                    var enumValue = new ObjectModelEnum(Name + "BindableProperty", b.Key.PropertyName);

                    propertyValues += $"\n{cppVar}->Bindings->Insert({enumValue.ToCPlusPlusString(declaredVars)}, {new ObjectModelString(b.Value).ToCPlusPlusString(declaredVars)});";
                }
            }

            answer += propertyValues;

            return answer;
        }

        private string DeclareJsVar(List<string> declaredVars)
        {
            var jsVar = Var();

            if (declaredVars.Contains(jsVar))
            {
                return jsVar;
            }
            else
            {
                declaredVars.Add(jsVar);
                return "var " + jsVar;
            }
        }

        private string DeclareCPlusPlusVar(List<string> declaredVars)
        {
            var v = Var();

            if (declaredVars.Contains(v))
            {
                return v;
            }
            else
            {
                declaredVars.Add(v);
                return "auto " + v;
            }
        }

        private string Var()
        {
            return ToLowerCamelCase(Name);
        }

        internal static string ToLowerCamelCase(string str)
        {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string IndentNewLines(string str, int amount)
        {
            string indentation = string.Join(" ", new string[amount + 1]);
            return str.Replace("\n", "\n" + indentation);
        }

        public IEnumerator<KeyValuePair<string, IObjectModelValue>> GetEnumerator()
        {
            return PropertyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IObjectModelValue Create(object obj)
        {
            if (obj is IObjectModelValue)
            {
                return obj as IObjectModelValue;
            }
            if (obj is string)
            {
                return new ObjectModelString(obj as string);
            }
            if (obj is double)
            {
                return new ObjectModelDouble((double)obj);
            }
            if (obj is Enum)
            {
                return new ObjectModelEnum(RetrieveEnumName((Enum)obj), obj.ToString());
            }
            if (obj is Uri)
            {
                return new ObjectModelUri((Uri)obj);
            }
            if (obj is bool)
            {
                return new ObjectModelBool((bool)obj);
            }
            if (obj is DateTime)
            {
                return new ObjectModelDateTime((DateTime)obj);
            }
            if (obj is int)
            {
                return new ObjectModelInt((int)obj);
            }

            return null;
        }

        private static string RetrieveEnumName(Enum e)
        {
            var typeInfo = e.GetType();

            return typeInfo.Name;
        }
    }

    internal class ObjectModelListInitialization : IObjectModelValue
    {
        public List<IObjectModelValue> Values { get; private set; } = new List<IObjectModelValue>();

        public ObjectModelListInitialization() { }

        public ObjectModelListInitialization(IEnumerable<IObjectModelValue> values)
        {
            Values.AddRange(values);
        }

        public void Add(IObjectModelValue value)
        {
            Values.Add(value);
        }

        public override string ToString()
        {
            if (Values.Count == 0)
            {
                return null;
            }

            var valuesWithIndentation = Values.Where(i => i != null).Select(i => ObjectModelObject.IndentNewLines(i.ToString(), 4));

            return "\n{\n    " + string.Join(",\n    ", valuesWithIndentation) + "\n}";
        }

        public string ToJavascriptString()
        {
            return "Shouldn't get hit";
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return "Shouldn't get hit";
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return "Shouldn't get hit";
        }
    }

    internal class ObjectModelString : IObjectModelValue
    {
        public string Value { get; private set; }

        public ObjectModelString(string value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"\"{Value.Replace("\"", "\\\"")}\"";
        }

        public string ToJavascriptString()
        {
            return ToString();
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return ToString();
        }
    }

    internal class ObjectModelDouble : IObjectModelValue
    {
        public double Value { get; private set; }

        public ObjectModelDouble(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToJavascriptString()
        {
            return ToString();
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return ToString();
        }
    }

    internal class ObjectModelInt : IObjectModelValue
    {
        public int Value { get; private set; }

        public ObjectModelInt(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToJavascriptString()
        {
            return ToString();
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return ToString();
        }
    }

    internal class ObjectModelEnum : IObjectModelValue
    {
        public string EnumName { get; private set; }
        public string EnumValue { get; private set; }

        public ObjectModelEnum(string enumName, string enumValue)
        {
            EnumName = enumName;
            EnumValue = enumValue;
        }

        public override string ToString()
        {
            return $"{EnumName}.{EnumValue}";
        }

        public string ToJavascriptString()
        {
            return $"{ObjectModelObject.ROOT_JS_NAMESPACE}.{EnumName}.{ObjectModelObject.ToLowerCamelCase(EnumValue)}";
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return $"{EnumName}::{EnumValue}";
        }
    }

    internal class ObjectModelUri : IObjectModelValue
    {
        public Uri Value { get; private set; }

        public ObjectModelUri(Uri value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"new Uri({new ObjectModelString(Value.OriginalString)})";
        }

        public string ToJavascriptString()
        {
            return $"new Windows.Foundation.Uri({new ObjectModelString(Value.OriginalString).ToJavascriptString()})";
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return $"ref new Windows::Foundation::Uri({new ObjectModelString(Value.OriginalString).ToCPlusPlusString(declaredVars)})";
        }
    }

    internal class ObjectModelBool : IObjectModelValue
    {
        public bool Value { get; set; }

        public ObjectModelBool(bool value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString().ToLower();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return ToString();
        }

        public string ToJavascriptString()
        {
            return ToString();
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToString();
        }
    }

    internal class ObjectModelDateTime : IObjectModelValue
    {
        public DateTime Value { get; set; }

        public ObjectModelDateTime(DateTime value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"new DateTime({Value.Year}, {Value.Month}, {Value.Day}, {Value.Hour}, {Value.Minute}, {Value.Second}, DateTimeKind.{Value.Kind})";
        }

        public string ToJavascriptString()
        {
            return $"new Date({new ObjectModelString(Value.ToString("o"))})";
        }

        public string ToJavascriptString(List<string> declaredVars)
        {
            return ToJavascriptString();
        }

        public string ToCPlusPlusString(List<string> declaredVars)
        {
            return $"\"Conversion currently not supported\"";
        }
    }
}
