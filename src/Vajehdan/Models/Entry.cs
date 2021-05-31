using System;

namespace Vajehdan.Models
{
    public class Entry : IComparable<Entry>
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Entry(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public int CompareTo(Entry other)
        {
            var str1 = Title;
            var str2 = other.Title;
            return string.Compare(str1, str2, StringComparison.Ordinal);
        }
    }
}
