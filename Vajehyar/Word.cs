using System.Collections.Generic;

namespace Vajehyar
{
    public class Word
    {
        public string Name { get; set; }
        public List<string> Meanings { get; set; } = new List<string>();
    }
}