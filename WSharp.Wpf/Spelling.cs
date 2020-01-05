using System.Windows;

namespace WSharp.Wpf
{
    public static class Spelling
    {
        public static ResourceKey SuggestionMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), EResourceKeyId.SpellingSuggestionMenuItemStyle);
        public static ResourceKey IgnoreAllMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), EResourceKeyId.SpellingIgnoreAllMenuItemStyle);
        public static ResourceKey NoSuggestionsMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), EResourceKeyId.SpellingNoSuggestionsMenuItemStyle);
        public static ResourceKey SeparatorStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), EResourceKeyId.SpellingSeparatorStyle);
    }
}
