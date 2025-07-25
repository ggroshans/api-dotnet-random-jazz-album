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

        public static string CapitalizeSentences(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            string[] sentences = Regex.Split(input, @"(?<=[.!?])\s+");
            StringBuilder result = new StringBuilder();

            foreach (string sentence in sentences)
            {
                if (sentence.Length == 0)
                {
                    continue;
                }

                string trimmedSentence = sentence.Trim();
                if (trimmedSentence.Length == 0)
                {
                    continue;
                }

                result.Append(char.ToUpper(trimmedSentence[0]));
                if (trimmedSentence.Length > 1)
                {
                    result.Append(trimmedSentence.Substring(1));
                }

                result.Append(" "); 
            }
            return result.ToString().TrimEnd(); 
        }

        public static int? ParseReleaseDate(string releaseDate, string releaseDatePrecision)
        {
            if (string.IsNullOrWhiteSpace(releaseDate) || string.IsNullOrWhiteSpace(releaseDatePrecision))
            {
                return null;
            }

            int? ReleaseYear = 0;
            int? ReleaseMonth = 0;
            int? ReleaseDay = 0;

            var dateParts = releaseDate.Split('-');
            if (dateParts.Length >=1 && int.TryParse(dateParts[0], out int year))
            {
                ReleaseYear = year;
            }

            if (releaseDatePrecision == "month" || releaseDatePrecision == "day")
            {
                if (dateParts.Length == 3)
                {
                    int.TryParse(dateParts[2], out int day);
                    ReleaseDay = day;

                    int.TryParse(dateParts[1], out int month);
                    ReleaseMonth = month;
                }
                else if (dateParts.Length == 2)
                {
                    int.TryParse(dateParts[1], out int month);
                    ReleaseMonth = month;
                    ReleaseDay = 00;
                }
                else
                {
                    ReleaseMonth = 00;
                    ReleaseDay = 00;
                }
            }

            return (ReleaseYear) * 10000 + (ReleaseMonth) * 100 + (ReleaseDay);
        }
    }
}

