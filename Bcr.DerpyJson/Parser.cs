using System;
using System.Runtime.Serialization;
using System.Text;

namespace Bcr.DerpyJson
{
    public class Parser
    {
        private static void SkipWhitespace(Span<byte> json, ref int index)
        {
            while (index < json.Length)
            {
                var thisByte = json[index];
                if ((thisByte != ' ') && (thisByte != '\r') && (thisByte != '\n') && (thisByte != '\t'))
                {
                    break;
                }
            }
        }

        private static object ParseObject(Span<byte> json, ref int index, Type typeHint)
        {
            object destination = FormatterServices.GetUninitializedObject(typeHint);

            // Skip the opening {
            ++index;

            SkipWhitespace(json, ref index);

            while (json[index] != '}')
            {
                // Skip the opening quote
                ++index;

                var startNameIndex = index;

                // Skip until the closing quote
                while (json[index] != '\"')
                {
                    ++index;
                }

                var endNameIndex = index - 1;

                // Skip the closing quote
                ++index;

                SkipWhitespace(json, ref index);

                // Skip the :
                ++index;

                SkipWhitespace(json, ref index);

                // Now we are on a Value

                // Find a property with the right name and see if we can jam the value in there
                var property = destination.GetType().GetProperty(Encoding.UTF8.GetString(json.Slice(startNameIndex, endNameIndex - startNameIndex + 1)));

                var value = ParseValue(json, ref index, property?.PropertyType);

                property?.SetValue(destination, value);

                SkipWhitespace(json, ref index);

                if (json[index] == ',')
                {
                    ++index;
                    SkipWhitespace(json, ref index);
                }
            }

            // Skip the closing '}'
            ++index;

            return destination;
        }

        private static object ParseNumber(Span<byte> json, ref int index, Type typeHint)
        {
            var startIndex = index;
            while ((index < json.Length) && ("-+0123456789eE.".Contains((char) json[index])))
            {
                index += 1;
            }
            var endIndex = index - 1;

            var rawNumber = Encoding.UTF8.GetString(json.Slice(startIndex, endIndex - startIndex + 1));
            var parseMethod = typeHint?.GetMethod("Parse", new Type[] { typeof(string) });

            return parseMethod?.Invoke(null, new object[] { rawNumber })!;
        }

        private static string ParseString(Span<byte> json, ref int index)
        {
            // Skip leading "
            ++index;

            var startIndex = index;
            int endEscapeIndex = -1;

            // endEscapeSequence is "the index at which the current escape
            // sequence will end." Most importantly, if there is a QUOTATION MARK
            // that is currently being escaped then the loop will not terminate,
            // but if it is not escaped, it will.

            while (true)
            {
                if (index > endEscapeIndex)
                {
                    if (json[index] == '\\')
                    {
                        endEscapeIndex = index + 1 + ((json[index + 1] == 'u') ? 4 : 0);
                    }
                    else if (json[index] == '"')
                    {
                        break;
                    }
                }
                ++index;
            }
            var endIndex = index;

            // Skip trailing "
            ++index;

            var startingString = Encoding.UTF8.GetString(json.Slice(startIndex, endIndex - startIndex));

            StringBuilder finalString = new StringBuilder(startingString.Length);

            for (int stringIndex = 0;stringIndex < startingString.Length;++stringIndex)
            {
                if (startingString[stringIndex] == '\\')
                {
                    var thisChar = startingString[stringIndex + 1];
                    switch (thisChar)
                    {
                        case '\"':
                        case '\\':
                        case '/':
                            finalString.Append(thisChar);
                            stringIndex += 1;
                            break;
                        case 'b':
                            finalString.Append('\b');
                            stringIndex += 1;
                            break;
                        case 'f':
                            finalString.Append('\f');
                            stringIndex += 1;
                            break;
                        case 'n':
                            finalString.Append('\n');
                            stringIndex += 1;
                            break;
                        case 'r':
                            finalString.Append('\r');
                            stringIndex += 1;
                            break;
                        case 't':
                            finalString.Append('\t');
                            stringIndex += 1;
                            break;
                        case 'u':
                            var charValue = Convert.ToChar(Convert.ToInt32(startingString.Substring(stringIndex + 2, 4), 16));

                            finalString.Append(charValue);
                            stringIndex += 5;
                            break;
                    }
                }
                else
                {
                    finalString.Append(startingString[stringIndex]);
                }
            }
            return finalString.ToString();
        }

        private static bool ParseTrue(Span<byte> json, ref int index)
        {
            // Skip "true"
            index += 4;

            return true;
        }

        private static bool ParseFalse(Span<byte> json, ref int index)
        {
            // Skip "false"
            index += 5;

            return false;
        }

        private static object ParseValue(Span<byte> json, ref int index, Type typeHint)
        {
            SkipWhitespace(json, ref index);
            byte thisByte = json[index];

            if (thisByte == '{')
            {
                return ParseObject(json, ref index, typeHint!);
            }
            else if (thisByte == '"')
            {
                return ParseString(json, ref index);
            }
            else if (thisByte == 't')
            {
                return ParseTrue(json, ref index);
            }
            else if (thisByte == 'f')
            {
                return ParseFalse(json, ref index);
            }
            else if ((char.IsDigit((char) thisByte)) || (thisByte == '-'))
            {
                return ParseNumber(json, ref index, typeHint!);
            }

            return null;
        }

        public static T Parse<T>(Span<byte> json)
        {
            int index = 0;

            return (T) ParseValue(json, ref index, typeof(T));
        }
    }
}
