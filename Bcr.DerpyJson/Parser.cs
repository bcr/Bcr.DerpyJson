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

    private static object ParseObject(Span<byte> json, ref int index, object destination)
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

    public static object ParseNumber(Span<byte> json, ref int index)
    {
        var startIndex = index;
        while ("-+0123456789eE.".Contains((char) json[index]))
        {
            index += 1;
        }
        var endIndex = index - 1;

        var rawNumber = Encoding.UTF8.GetString(json.Slice(startIndex, endIndex - startIndex + 1));
        int integerResult;

        if (int.TryParse(rawNumber, out integerResult))
        {
            return integerResult;
        }
        else
        {
            decimal doubleResult;
            decimal.TryParse(rawNumber, out doubleResult);
            return doubleResult;
        }
    }

    public static string ParseString(Span<byte> json, ref int index)
    {
        // Skip leading "
        ++index;

        var startIndex = index;

        while (json[index] != '\"')
        {
            ++index;
        }
        var endIndex = index;

        // Skip trailing "
        ++index;

        return Encoding.UTF8.GetString(json.Slice(startIndex, endIndex - startIndex));
    }

    public static object ParseValue(Span<byte> json, ref int index, object destination)
    {
        SkipWhitespace(json, ref index);
        byte thisByte = json[index];

        if (thisByte == '{')
        {
            return ParseObject(json, ref index, destination);
        }
        else if (thisByte == '"')
        {
            return ParseString(json, ref index);
        }
        else if ((char.IsAsciiDigit((char) thisByte)) || (thisByte == '-'))
        {
            return ParseNumber(json, ref index);
        }

        return destination;
    }

    public static T Parse<T>(Span<byte> json)
    {
        object returnObject = FormatterServices.GetUninitializedObject(typeof(T));
        int index = 0;

        return (T) ParseValue(json, ref index, returnObject);
    }
}
