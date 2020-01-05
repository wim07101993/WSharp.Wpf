using System;
using System.Globalization;
using System.Linq;

namespace WSharp.Wpf.Extensions
{
    internal static class StringExtensions
    {
        public static string ToTitleCase(this string text, CultureInfo culture, string separator = " ")
        {
            var textInfo = culture.TextInfo;

            var lowerText = textInfo.ToLower(text);
            var words = lowerText.Split(new[] { separator }, StringSplitOptions.None);

            return string.Join(separator, words.Select(v => textInfo.ToTitleCase(v)));
        }
    }
}
