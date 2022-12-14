/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using BidFX.Public.API.Price.Tools;
using BidFX.Public.API.Trade.Order;
using Serilog;
using Serilog.Core;

namespace BidFX.Public.API.Trade.Rest.Json
{
    internal static class JsonMarshaller
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "JsonMarshaller");
        public static string ToJson(IJsonMarshallable item, long messageId)
        {
            StringBuilder stringBuilder = new StringBuilder(256);
            IDictionary<string, object> components = item.GetJsonMap();
            components.Add("correlation_id", messageId.ToString());
            stringBuilder.Append("[");
            AppendDictionary(components, stringBuilder);
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        private static void AppendItem(object item, StringBuilder stringBuilder)
        {
            if (item == null)
            {
                stringBuilder.Append("null");
                return;
            }

            if (item is int || item is long || item is byte || item is sbyte || item is uint || item is short ||
                item is ushort || item is decimal || item is ulong)
            {
                AppendNumber((decimal) item, stringBuilder);
                return;
            }

            if (item is float || item is double)
            {
                AppendNumber((double) item, stringBuilder);
                return;
            }

            if (item is char)
            {
                AppendString(item.ToString(), stringBuilder);
                return;
            }

            if (item is bool)
            {
                AppendBoolean((bool) item, stringBuilder);
                return;
            }

            if (item.GetType().IsArray || item is IEnumerable<object>)
            {
                AppendEnumerable((IEnumerable<object>) item, stringBuilder);
                return;
            }

            if (item is IJsonMarshallable)
            {
                AppendDictionary(((IJsonMarshallable) item).GetJsonMap(), stringBuilder);
                return;
            }

            if (item is IDictionary<string, object>)
            {
                AppendDictionary((IDictionary<string, object>) item, stringBuilder);
                return;
            }

            AppendString(item.ToString(), stringBuilder);
        }

        private static void AppendDictionary(IDictionary<string, object> item, StringBuilder stringBuilder)
        {
            stringBuilder.Append("{");
            string loopDelim = "";
            foreach (KeyValuePair<string, object> keyValuePair in item)
            {
                stringBuilder.Append(loopDelim);
                AppendString(keyValuePair.Key, stringBuilder);
                stringBuilder.Append(":");
                AppendItem(keyValuePair.Value, stringBuilder);
                loopDelim = ",";
            }

            stringBuilder.Append("}");
        }

        private static void AppendEnumerable(IEnumerable<object> list, StringBuilder stringBuilder)
        {
            stringBuilder.Append("[");
            string loopDelim = "";
            foreach (object item in list)
            {
                stringBuilder.Append(loopDelim);
                AppendItem(item, stringBuilder);
                loopDelim = ",";
            }

            stringBuilder.Append("]");
        }

        private static void AppendNumber(decimal item, StringBuilder stringBuilder)
        {
            stringBuilder.Append(item.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendNumber(double item, StringBuilder stringBuilder)
        {
            stringBuilder.Append(item.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendBoolean(bool item, StringBuilder stringBuilder)
        {
            stringBuilder.Append(item ? "true" : "false");
        }

        private static void AppendString(string item, StringBuilder stringBuilder)
        {
            stringBuilder.Append("\"" + item.Replace("\"", "\\\"") + "\"");
        }

        public static object FromJson(string json)
        {
            Log.Debug("Parsing {json}", json);
            int pointer = 0;
            return ParseItem(json, ref pointer);
        }

        private static object ParseItem(string json, ref int pointer)
        {
            SkipWhitespace(json, ref pointer);
            switch (json[pointer])
            {
                case '{':
                    pointer++;
                    return ParseObject(json, ref pointer);
                case '[':
                    pointer++;
                    return ParseList(json, ref pointer);
                case '"':
                    pointer++;
                    return ParseString(json, ref pointer);
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                case '-':
                    return ParseNumber(json, ref pointer);
                case 't':
                case 'f':
                    return ParseBoolean(json, ref pointer);
                case 'n':
                    return ParseNull(json, ref pointer);
                default:
                    throw new JsonException("Could not parse JSON. Unexpected character '" + json[pointer] +
                                            "' at position " + pointer);
            }
        }

        private static List<object> ParseList(string json, ref int pointer)
        {
            SkipWhitespace(json, ref pointer);
            VerifyNotAtEnd(json, pointer);
            List<object> list = new List<object>();
            while (json[pointer] != ']')
            {
                list.Add(ParseItem(json, ref pointer));
                SkipWhitespace(json, ref pointer);
                if (json[pointer] != ',' && json[pointer] != ']')
                {
                    throw new JsonException("Could not parse JSON. Encountered '" + json[pointer] + "' at position " +
                                            pointer + ". Expected ',' or ']'.");
                }

                if (json[pointer] == ']')
                {
                    break;
                }

                pointer++;
                SkipWhitespace(json, ref pointer);
            }

            pointer++;
            SkipWhitespace(json, ref pointer);
            return list;
        }

        private static Dictionary<string, object> ParseObject(string json, ref int pointer)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            while (json[pointer] != '}')
            {
                SkipWhitespace(json, ref pointer);
                if (json[pointer] != '"')
                {
                    throw new JsonException("Could not parse JSON. Expected '\"', encountered '" + json[pointer] + "' at position " + pointer);
                }

                pointer++;
                string key = ParseString(json, ref pointer);
                SkipWhitespace(json, ref pointer);
                if (json[pointer] != ':')
                {
                    throw new JsonException("Could not parse JSON. Expected ':', encountered '" + json[pointer] +
                                            "' at position " + pointer);
                }

                pointer++;
                SkipWhitespace(json, ref pointer);
                object value = ParseItem(json, ref pointer);

                map[key] = value;

                SkipWhitespace(json, ref pointer);
                if (json[pointer] != ',' && json[pointer] != '}')
                {
                    throw new JsonException("Could not parse JSON. Encountered '" + json[pointer] + "' at position " +
                                            pointer + ". Expected ',' or '}'.");
                }

                if (json[pointer] == '}')
                {
                    break;
                }

                pointer++;
                SkipWhitespace(json, ref pointer);
            }

            pointer++;
            SkipWhitespace(json, ref pointer);
            return map;
        }

        private static string ParseString(string json, ref int pointer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (json[pointer] != '"')
            {
                if (json[pointer] == '\\')
                {
                    pointer++;
                    VerifyNotAtEnd(json, pointer);
                    stringBuilder.Append(GetEscapedCharacter(json, ref pointer));
                }
                else
                {
                    stringBuilder.Append(json[pointer]);
                    pointer++;
                }
                VerifyNotAtEnd(json, pointer);
            }

            pointer++;
            return stringBuilder.ToString();
        }

        private static decimal ParseNumber(string json, ref int pointer)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (json[pointer] == '-')
            {
                stringBuilder.Append('-');
                pointer++;
                VerifyNotAtEnd(json, pointer);
            }

            if (json[pointer] == '0')
            {
                stringBuilder.Append('0');
                pointer++;
                VerifyNotAtEnd(json, pointer);
            }
            else
            {
                if (!char.IsDigit(json[pointer]))
                {
                    throw new JsonException("Could not parse JSON. Expected number, encountered '" + json[pointer] + "' at position " + pointer);
                }

                stringBuilder.Append(json[pointer]);
                pointer++;
                while (pointer < json.Length && char.IsDigit(json[pointer]))
                {
                    stringBuilder.Append(json[pointer]);
                    pointer++;
                }
            }

            if (pointer < json.Length && json[pointer] == '.')
            {
                stringBuilder.Append('.');
                pointer++;
                VerifyNotAtEnd(json, pointer);
                if (!char.IsDigit(json[pointer]))
                {
                    throw new JsonException("Could not parse JSON. Expected number, encountered '" + json[pointer] + "' at position " + pointer);
                }

                stringBuilder.Append(json[pointer]);
                pointer++;
                while (pointer < json.Length && char.IsDigit(json[pointer]))
                {
                    stringBuilder.Append(json[pointer]);
                    pointer++;
                }
            }

            if (pointer < json.Length && (json[pointer] == 'e' || json[pointer] == 'E'))
            {
                stringBuilder.Append('E');
                pointer++;
                VerifyNotAtEnd(json, pointer);
                if (json[pointer] == '+' || json[pointer] == '-')
                {
                    stringBuilder.Append(json[pointer]);
                    pointer++;
                    VerifyNotAtEnd(json, pointer);
                }

                if (!char.IsDigit(json, pointer))
                {
                    throw new JsonException("Could not parse JSON. Expected number, encountered '" + json[pointer] + "' at position " + pointer);
                }

                stringBuilder.Append(json[pointer]);
                pointer++;
                while (pointer < json.Length && char.IsDigit(json[pointer]))
                {
                    stringBuilder.Append(json[pointer]);
                    pointer++;
                }
            }

            string numberString = stringBuilder.ToString();
            return decimal.Parse(numberString, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign);
        }

        private static bool ParseBoolean(string json, ref int pointer)
        {
            if (json[pointer] == 'f')
            {
                if (pointer + 5 >= json.Length)
                {
                    throw new JsonException("Could not parse JSON. Reached end of string");
                }
                string substring = json.Substring(pointer, 5);
                if (!"false".Equals(substring))
                {
                    throw new JsonException("Could not parse JSON. Expected false, encountered '" + substring + "' at position " + pointer);
                }
                pointer += 5;
                return false;
            }
            else
            {
                if (pointer + 4 >= json.Length)
                {
                    throw new JsonException("Could not parse JSON. Reached end of string");
                }
                string substring = json.Substring(pointer, 4);
                if (!"true".Equals(substring))
                {
                    throw new JsonException("Could not parse JSON. Expected true, encountered '" + substring + "' at position " + pointer);
                }

                pointer += 4;
                return true;
            }
        }

        private static object ParseNull(string json, ref int pointer)
        {
            if (pointer + 4 >= json.Length)
            {
                throw new JsonException("Could not parse JSON. Reached end of string");
            }

            string substring = json.Substring(pointer, 4);
            if (!"null".Equals(substring))
            {
                throw new JsonException("Could not parse JSON. Expected null, encountered '" + substring + "' at position " + pointer);
            }

            pointer += 4;
            return null;
        }

        private static void VerifyNotAtEnd(string json, int pointer)
        {
            if (pointer >= json.Length)
            {
                throw new JsonException("Could not parse JSON. Reached end of string");
            }
        }

        private static void SkipWhitespace(string json, ref int pointer)
        {
            while (pointer < json.Length && char.IsWhiteSpace(json[pointer]))
            {
                pointer++;
            }
        }

        private static char GetEscapedCharacter(string json, ref int pointer)
        {
            char c;
            switch (json[pointer])

            {
                // --- Simple character escapes

                case '\'':
                    c = '\'';
                    break;

                case '\"':
                    c = '\"';
                    break;

                case '\\':
                    c = '\\';
                    break;

                case '0':
                    c = '\0';
                    break;

                case 'a':
                    c = '\a';
                    break;

                case 'b':
                    c = '\b';
                    break;

                case 'f':
                    c = '\f';
                    break;

                case 'n':
                    c = ' ';
                    break;

                case 'r':
                    c = ' ';
                    break;

                case 't':
                    c = '\t';
                    break;

                case 'v':
                    c = '\v';
                    break;

                case 'x':

                    // --- Hexa escape (1-4 digits)

                    StringBuilder hexa = new StringBuilder(10);

                    pointer++;

                    VerifyNotAtEnd(json, pointer);

                    c = json[pointer];

                    if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))

                    {
                        hexa.Append(c);

                        pointer++;

                        if (pointer < json.Length)

                        {
                            c = json[pointer];

                            if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))

                            {
                                hexa.Append(c);

                                pointer++;

                                if (pointer < json.Length)

                                {
                                    c = json[pointer];

                                    if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                                        (c >= 'A' && c <= 'F'))

                                    {
                                        hexa.Append(c);

                                        pointer++;

                                        if (pointer < json.Length)

                                        {
                                            c = json[pointer];

                                            if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                                                (c >= 'A' && c <= 'F'))

                                            {
                                                hexa.Append(c);

                                                pointer++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    c = (char) Int32.Parse(hexa.ToString(), NumberStyles.HexNumber);

                    pointer--;

                    break;

                case 'u':

                    // Unicode hexa escape (exactly 4 digits)

                    pointer++;

                    if (pointer + 3 >= json.Length)

                        throw new JsonException("Unrecognized escape sequence");

                    try

                    {
                        uint charValue = UInt32.Parse(json.Substring(pointer, 4),
                            NumberStyles.HexNumber);

                        c = (char) charValue;

                        pointer += 3;
                    }

                    catch (SystemException)

                    {
                        throw new JsonException("Unrecognized escape sequence");
                    }

                    break;

                case 'U':

                    // Unicode hexa escape (exactly 8 digits, first four must be 0000)

                    pointer++;
                    try

                    {
                        uint charValue = UInt32.Parse(json.Substring(pointer, 8),
                            NumberStyles.HexNumber);

                        if (charValue > 0xffff)

                            throw new JsonException("Unrecognized escape sequence");

                        c = (char) charValue;

                        pointer += 7;
                    }

                    catch (SystemException)

                    {
                        throw new JsonException("Unrecognized escape sequence");
                    }

                    break;

                default:

                    throw new JsonException("Unrecognized escape sequence");
            }

            pointer++;
            return c;
        }
    }
}