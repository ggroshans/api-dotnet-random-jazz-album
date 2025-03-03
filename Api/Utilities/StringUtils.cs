using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Api.Utilities
{
    public class StringUtils
    {
        public static string ConvertToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var stringBuilder = new StringBuilder(input.Length + (input.Length / 2));

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLower(c));
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        public static string CapitalizeAndFormat(string input)
        {
            if (string.IsNullOrEmpty(input)) 
            {
                return input;
            }

            string[] words = Regex.Split(input, @"([- ])");

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string[] capitalizedWords = words.Select(word =>
            {
                if (word == "-" || word == " ") 
                {
                    return word;
                }
                return textInfo.ToTitleCase(word.ToLower());
            }).ToArray();

            return string.Concat(capitalizedWords); 
        }

        public static string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty; 
            }
            string normalized = name.ToLower();

            normalized = Regex.Replace(normalized, @"[\s-]+", "-");
            normalized = Regex.Replace(normalized, @"[^a-z0-9-]+", "");
            normalized = Regex.Replace(normalized, @"^-+|-+$", "");

            return normalized;
        }
    }
}

