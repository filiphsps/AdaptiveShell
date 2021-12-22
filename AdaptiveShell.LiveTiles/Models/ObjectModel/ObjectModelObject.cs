using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveShell.LiveTiles.Models.ObjectModel {
    public interface IObjectModelValue {
    }

    public class ObjectModelObject : IObjectModelValue, IEnumerable<KeyValuePair<String, IObjectModelValue>> {
        public String Name { get; set; }

        public ObjectModelObject(String name, params IObjectModelValue[] constructorValues) {
            this.Name = name;

            foreach (IObjectModelValue cVal in constructorValues) {
                this.ConstructorValues.Add(cVal);
            }
        }

        public List<IObjectModelValue> ConstructorValues { get; private set; } = new List<IObjectModelValue>();

        public List<KeyValuePair<String, IObjectModelValue>> PropertyValues { get; private set; } = new List<KeyValuePair<String, IObjectModelValue>>();

        public void Add(String propertyName, Object propertyValue) {
            if (propertyValue == null) {
                return;
            }

            var objectPropertyValue = propertyValue as IObjectModelValue;
            if (objectPropertyValue == null) {
                objectPropertyValue = Create(propertyValue);
            }

            this.PropertyValues.Add(new KeyValuePair<String, IObjectModelValue>(propertyName, objectPropertyValue));
        }

        public List<KeyValuePair<ObjectModelBindingPropertyAttribute, String>> Bindings { get; private set; } = new List<KeyValuePair<ObjectModelBindingPropertyAttribute, String>>();

        public void AddBinding(ObjectModelBindingPropertyAttribute attr, String bindingName) {
            if (bindingName == null) {
                return;
            }

            this.Bindings.Add(new KeyValuePair<ObjectModelBindingPropertyAttribute, String>(attr, bindingName));
        }

        public override String ToString() {
            String answer = $"new {this.Name}({String.Join(", ", this.ConstructorValues)})";

            if (this.PropertyValues.Count > 0 || this.Bindings.Count > 0) {
                String propertyValues = "\n{";
                Boolean addedProperty = false;

                foreach (KeyValuePair<String, IObjectModelValue> pVal in this.PropertyValues.Where(i => i.Value != null)) {
                    String val = pVal.Value.ToString();
                    if (val == null) {
                        continue;
                    }

                    propertyValues += $"\n    {pVal.Key} = ";

                    propertyValues += IndentNewLines(val, 4);

                    propertyValues += ",";

                    addedProperty = true;
                }

                if (this.Bindings.Count > 0) {
                    foreach (var b in this.Bindings) {
                        propertyValues += $"\n    {b.Key.PropertyName} = new {b.Key.BindableTypeName}({new ObjectModelString(b.Value)}),";
                    }

                    addedProperty = true;
                }

                // Trim the extra ending ","
                if (addedProperty) {
                    propertyValues = propertyValues.Substring(0, propertyValues.Length - 1);
                    propertyValues += "\n}";
                    answer += propertyValues;
                }
            }

            return answer;
        }

        internal static String ToLowerCamelCase(String str) {
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static String IndentNewLines(String str, Int32 amount) {
            String indentation = String.Join(" ", new String[amount + 1]);
            return str.Replace("\n", "\n" + indentation);
        }

        public IEnumerator<KeyValuePair<String, IObjectModelValue>> GetEnumerator() {
            return this.PropertyValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public static IObjectModelValue Create(Object obj) {
            if (obj is IObjectModelValue) {
                return obj as IObjectModelValue;
            }
            if (obj is String) {
                return new ObjectModelString(obj as String);
            }
            if (obj is Double) {
                return new ObjectModelDouble((Double)obj);
            }
            if (obj is Enum) {
                return new ObjectModelEnum(RetrieveEnumName((Enum)obj), obj.ToString());
            }
            if (obj is Uri) {
                return new ObjectModelUri((Uri)obj);
            }
            if (obj is Boolean) {
                return new ObjectModelBool((Boolean)obj);
            }
            if (obj is DateTime) {
                return new ObjectModelDateTime((DateTime)obj);
            }
            if (obj is Int32) {
                return new ObjectModelInt((Int32)obj);
            }

            return null;
        }

        private static String RetrieveEnumName(Enum e) {
            Type typeInfo = e.GetType();

            return typeInfo.Name;
        }
    }

    internal class ObjectModelListInitialization : IObjectModelValue {
        public List<IObjectModelValue> Values { get; private set; } = new List<IObjectModelValue>();

        public ObjectModelListInitialization() { }

        public ObjectModelListInitialization(IEnumerable<IObjectModelValue> values) {
            this.Values.AddRange(values);
        }

        public void Add(IObjectModelValue value) {
            this.Values.Add(value);
        }

        public override String ToString() {
            if (this.Values.Count == 0) {
                return null;
            }

            IEnumerable<String> valuesWithIndentation = this.Values.Where(i => i != null).Select(i => ObjectModelObject.IndentNewLines(i.ToString(), 4));

            return "\n{\n    " + String.Join(",\n    ", valuesWithIndentation) + "\n}";
        }
    }

    internal class ObjectModelString : IObjectModelValue {
        public String Value { get; private set; }

        public ObjectModelString(String value) {
            this.Value = value;
        }

        public override String ToString() {
            return $"\"{this.Value.Replace("\"", "\\\"")}\"";
        }
    }

    internal class ObjectModelDouble : IObjectModelValue {
        public Double Value { get; private set; }

        public ObjectModelDouble(Double value) {
            this.Value = value;
        }

        public override String ToString() {
            return this.Value.ToString();
        }
    }

    internal class ObjectModelInt : IObjectModelValue {
        public Int32 Value { get; private set; }

        public ObjectModelInt(Int32 value) {
            this.Value = value;
        }

        public override String ToString() {
            return this.Value.ToString();
        }
    }

    internal class ObjectModelEnum : IObjectModelValue {
        public String EnumName { get; private set; }
        public String EnumValue { get; private set; }

        public ObjectModelEnum(String enumName, String enumValue) {
            this.EnumName = enumName;
            this.EnumValue = enumValue;
        }

        public override String ToString() {
            return $"{this.EnumName}.{this.EnumValue}";
        }
    }

    internal class ObjectModelUri : IObjectModelValue {
        public Uri Value { get; private set; }

        public ObjectModelUri(Uri value) {
            this.Value = value;
        }

        public override String ToString() {
            return $"new Uri({new ObjectModelString(this.Value.OriginalString)})";
        }
    }

    internal class ObjectModelBool : IObjectModelValue {
        public Boolean Value { get; set; }

        public ObjectModelBool(Boolean value) {
            this.Value = value;
        }

        public override String ToString() {
            return this.Value.ToString().ToLower();
        }
    }

    internal class ObjectModelDateTime : IObjectModelValue {
        public DateTime Value { get; set; }

        public ObjectModelDateTime(DateTime value) {
            this.Value = value;
        }

        public override String ToString() {
            return $"new DateTime({this.Value.Year}, {this.Value.Month}, {this.Value.Day}, {this.Value.Hour}, {this.Value.Minute}, {this.Value.Second}, DateTimeKind.{this.Value.Kind})";
        }
    }
}
