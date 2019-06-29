using System.Collections.Generic;

namespace Vajehyar
{
    public class Word
    {
        public string Name { get; set; }
        public List<MeaningGroup> MeaningGroups { get; set; } = new List<MeaningGroup>();
    }
}