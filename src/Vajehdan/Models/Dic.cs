using System.Collections.Generic;

namespace Vajehdan.Models
{
    public class Dic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Entry> Entries { get; set; } = new();
    }
}