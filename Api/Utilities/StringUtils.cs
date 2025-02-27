using System.Text;

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
            string formattedString = "";

            if (input.Contains('-'))
            {
                var splitString = input.Split('-');
                List<string> capitalizedWords = new List<string>();
                foreach (var item in splitString)
                {
                    var capitalizedWord = char.ToUpper(item[0]) + item.Substring(1).ToLower();
                    capitalizedWords.Add(capitalizedWord);
                }
                formattedString = string.Join(" ", capitalizedWords);
            }

            else if (input.Contains(' '))
            {
                var splitString = input.Split(' ');
                List<string> capitalizedWords = new List<string>();
                foreach (var item in splitString)
                {
                    var capitalizedWord = char.ToUpper(item[0]) + item.Substring(1).ToLower();
                    capitalizedWords.Add(capitalizedWord);
                }
                formattedString = string.Join(" ", capitalizedWords);
            }

            else if (!input.Contains('-') && !input.Contains(' '))
            {
                formattedString = char.ToUpper(input[0]) + input.Substring(1).ToLower();
            }

            return formattedString;
        }
    }
}
