using System.Collections.Generic;

namespace Vajehyar
{
    public class Word
    {
        public string Name { get; set; }
        public List<MeaningGroup> SynAcros { get; set; } = new List<MeaningGroup>();
    }
}