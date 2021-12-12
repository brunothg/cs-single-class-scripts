using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CS_Single_Class_Scripts
{
    public static class Json
    {
        public static readonly JNull Null = new JNull();

        public abstract class JValue
        {
            /**
             * Get the value (bool, string, double or Dictionary, Array)
             */
            public abstract object Value { get; set; }

            public JValue this[string key]
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            
            public JValue this[int index]
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            
            /**
             * Get as String value
             */
            public abstract string AsString();

            /**
             * Get as floating point number or NaN
             */
            public double AsNumber() => (IsNumber()) ? double.Parse(Value.ToString()) : double.NaN;

            /**
             * Get as boolean value oder false
             */
            public bool AsBoolean() => (IsBoolean()) && bool.Parse(Value.ToString());

            public bool IsNumber()
            {
                return this is JNumber;
            }

            public bool IsString()
            {
                return this is JString;
            }

            public bool IsBoolean()
            {
                return this is JBoolean;
            }

            public bool IsNull()
            {
                return this is JNull;
            }

            public bool IsObject()
            {
                return this is JObject;
            }

            public bool IsArray()
            {
                return this is JArray;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (!(obj is JValue jObj))
                {
                    return false;
                }

                if (GetType() != obj.GetType())
                {
                    return false;
                }

                var thisAsString = ToJson();
                var objAsString = jObj.ToJson();
                return (thisAsString == null && objAsString == null) ||
                       (thisAsString != null && objAsString != null && thisAsString.Equals(objAsString));
            }

            public override int GetHashCode()
            {
                var stringRepresentation = AsString();
                return (stringRepresentation != null) ? stringRepresentation.GetHashCode() : -1;
            }

            /**
             * Return the value as JSON-String
             */
            public abstract string ToJson();
        }

        public class JArray : JValue
        {
            private List<JValue> _array = new List<JValue>();

            public List<JValue> Array
            {
                get => _array;
                set => _array = value ?? new List<JValue>();
            }

            public override object Value
            {
                get => Array;
                set => Array = (List<JValue>) value;
            }

            public JArray(List<JValue> value)
            {
                Array = value;
            }

            public JArray() : this(new List<JValue>())
            {
            }
            
            public new JValue this[int index]
            {
                get => Array[index];
                set => Array[index] = value;
            }

            public override string AsString()
            {
                return ToJson();
            }

            public override string ToJson()
            {
                var jsonString = "[";
                foreach (var entry in Array)
                {
                    jsonString += entry.ToJson() + ",";
                }

                jsonString = jsonString.Substring(0, jsonString.Length - 1) + "]";

                return jsonString;
            }
        }

        public class JObject : JValue
        {
            private Dictionary<string, JValue> _dictionary;

            public Dictionary<string, JValue> Dictionary
            {
                get => _dictionary;
                set => _dictionary = value ?? new Dictionary<string, JValue>();
            }

            public override object Value
            {
                get => Dictionary;
                set => Dictionary = (Dictionary<string, JValue>) value;
            }

            public JObject(Dictionary<string, JValue> value)
            {
                Dictionary = value;
            }

            public JObject() : this(new Dictionary<string, JValue>())
            {
            }
            
            public new JValue this[string key]
            {
                get => Dictionary[key];
                set => Dictionary[key] = value;
            }

            public override string AsString()
            {
                return ToJson();
            }

            public override string ToJson()
            {
                var jsonString = "{";
                foreach (var entry in Dictionary)
                {
                    jsonString += "\"" + entry.Key + "\":" + entry.Value.ToJson() + ",";
                }

                jsonString = jsonString.Substring(0, jsonString.Length - 1) + "}";

                return jsonString;
            }
        }

        public class JString : JValue
        {
            private string _string;

            public string String
            {
                get => _string;
                set => _string = value ?? "";
            }

            public override object Value
            {
                get => String;
                set => String = (string) value;
            }

            public JString(string value)
            {
                String = value;
            }

            public JString() : this("")
            {
            }

            public override string AsString()
            {
                return String;
            }

            public override string ToJson()
            {
                var json = String
                    .Replace("\\", "\\\\")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\"", "\\\"");
                return "\"" + json + "\"";
            }
        }

        public class JNumber : JValue
        {
            public double Double { get; set; }

            public override object Value
            {
                get => Double;
                set => Double = (double) value;
            }

            public JNumber(double value)
            {
                Double = value;
            }

            public override string AsString()
            {
                return Double.ToString();
            }

            public override string ToJson()
            {
                return Double.ToString(CultureInfo.InvariantCulture);
            }
        }

        public class JBoolean : JValue
        {
            public JBoolean(bool value)
            {
                Bool = value;
            }

            public bool Bool { get; set; }

            public override object Value
            {
                get => Bool;
                set => Bool = (bool) value;
            }

            public override string AsString()
            {
                return Bool.ToString();
            }

            public override string ToJson()
            {
                return (Bool) ? "true" : "false";
            }
        }

        public class JNull : JValue
        {
            public override object Value
            {
                get => null;
                set { }
            }

            public override string AsString()
            {
                return "null";
            }

            public override string ToJson()
            {
                return "null";
            }
        }

        public static string WriteJson(JValue jsonValue)
        {
            return jsonValue.ToJson();
        }


        private static readonly Dictionary<char, string> JStringMods = new Dictionary<char, string>
        {
            ['\\'] = "\\",
            ['t'] = "\t",
            ['n'] = "\n",
            ['r'] = "\r",
            ['b'] = "\b",
            ['f'] = "\f",
            ['"'] = "\"",
        };

        private static readonly List<char> JWhitespace = new List<char>()
        {
            ' ', '\n', '\r', '\t'
        };

        private static readonly List<char> JDividers = new List<char>()
        {
            ',', ':'
        };

        private static readonly Regex JNumberRegex = new Regex("^([+-]?[0-9][0-9]*)([.][0-9]*)?([eE][+-]?[0-9]*)?");

        public static JValue ReadJson(string json, out bool ok)
        {
            json = json.Trim();

            JValue rootObject = null;
            var objectStack = new Stack<JValue>();
            string propertyName = null;
            var anyArror = false;

            Action<JValue> addValue = (jValue) =>
            {
                var resetPropertyName = true;
                var currentObject = (objectStack.Count <= 0) ? null : objectStack.Peek();
                if (currentObject != null)
                {
                    if (currentObject is JArray jArray)
                    {
                        jArray.Array.Add(jValue);
                    }
                    else if (currentObject is JObject jObject && propertyName != null)
                    {
                        jObject.Dictionary[propertyName] = jValue;
                    }
                    else if (currentObject is JObject && propertyName == null && jValue is JString jString)
                    {
                        propertyName = jString.String;
                        resetPropertyName = false;
                    }
                    else
                    {
                        anyArror = true;
                    }
                }

                if (resetPropertyName)
                {
                    propertyName = null;
                }

                if (jValue is JObject || jValue is JArray)
                {
                    objectStack.Push(jValue);
                }

                if (rootObject == null)
                {
                    rootObject = jValue;
                }
            };

            Action closeValue = () =>
            {
                if (objectStack.Count > 0)
                {
                    objectStack.Pop();
                }
                else
                {
                    anyArror = true;
                }
            };

            for (var i = 0; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    addValue(new JObject());
                }
                else if (json[i] == '}')
                {
                    closeValue();
                }
                else if (json[i] == '[')
                {
                    addValue(new JArray());
                }
                else if (json[i] == ']')
                {
                    closeValue();
                }
                else if (json[i] == '"')
                {
                    var jString = "";
                    var mask = false;
                    while (++i < json.Length)
                    {
                        if (json[i] == '\\' && !mask)
                        {
                            mask = true;
                        }
                        else if (json[i] == '"' && !mask)
                        {
                            break;
                        }
                        else
                        {
                            if (mask)
                            {
                                mask = false;
                                var maskValue = JStringMods[json[i]];
                                if (maskValue != null)
                                {
                                    jString += maskValue;
                                }
                                else
                                {
                                    // Unknown mask value
                                    anyArror = true;
                                }
                            }
                            else
                            {
                                jString += json[i];
                            }
                        }
                    }

                    addValue(new JString(jString));
                }
                else if (JWhitespace.Contains(json[i]) || JDividers.Contains(json[i]))
                {
                    // Can be ignored
                }
                else if (json.Substring(i).StartsWith("true"))
                {
                    addValue(new JBoolean(true));
                    i += 3;
                }
                else if (json.Substring(i).StartsWith("false"))
                {
                    addValue(new JBoolean(false));
                    i += 4;
                }
                else if (json.Substring(i).StartsWith("null"))
                {
                    addValue(new JNull());
                    i += 3;
                }
                else
                {
                    var numberMatch = JNumberRegex.Match(json.Substring(i));
                    if (numberMatch.Success && numberMatch.Index == 0)
                    {
                        addValue(new JNumber(double.Parse(
                            numberMatch.Value,
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture
                        )));
                        i += numberMatch.Length - 1;
                    }
                    else
                    {
                        // Unknown or unexpected char
                        anyArror = true;
                    }
                }
            }

            // Unfinished objects
            if (objectStack.Count > 0)
            {
                anyArror = true;
            }

            ok = !anyArror;
            return rootObject;
        }

        public static JValue ReadJson(string json)
        {
            return ReadJson(json, out _);
        }
    }
}