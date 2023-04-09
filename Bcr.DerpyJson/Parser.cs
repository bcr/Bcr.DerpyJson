using System.Runtime.Serialization;
using System.Text;

namespace Bcr.DerpyJson;

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

    private static void ParseObject(Span<byte> json, ref int index, object destination)
    {
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

            var value = ParseValue(json, ref index, destination);

            // Find a property with the right name and see if we can jam the value in there
            var property = destination.GetType().GetProperty(Encoding.UTF8.GetString(json.Slice(startNameIndex, endNameIndex - startNameIndex + 1)));
            property?.SetValue(destination, value);

            SkipWhitespace(json, ref index);
        }

        // Skip the closing '}'
        ++index;
    }

    public static int ParseNumber(Span<byte> json, ref int index)
    {
        int finalNumber = 0;
        while (char.IsAsciiDigit((char) json[index]))
        {
            finalNumber *= 10;
            finalNumber += json[index] - '0';
            index += 1;
        }

        return finalNumber;
    }

    public static object ParseValue(Span<byte> json, ref int index, object destination)
    {
        SkipWhitespace(json, ref index);
        byte thisByte = json[index];

        if (thisByte == '{')
        {
            ParseObject(json, ref index, destination);
        }
        else if (char.IsAsciiDigit((char) thisByte))
        {
            return ParseNumber(json, ref index);
        }

        return destination;
    }

    public static T Parse<T>(Span<byte> json)
    {
        object returnObject = FormatterServices.GetUninitializedObject(typeof(T));
        int index = 0;

        ParseValue(json, ref index, returnObject);

        return (T) returnObject;
    }
}
