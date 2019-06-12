using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vajehyar
{
    public sealed class Database
    {
        public static Database Instance { get; } = new Database();

        private string[] _lines;
        public List<Word> Words { get; }

        private Database()
        {
            string dict_motaradef_Motazad = Properties.Resources.Motaradef_Motazad;
            //string dict2 = Properties.Resources.Teyfi;
            //string data = dict1 + Environment.NewLine + dict2;

            _lines = dict_motaradef_Motazad.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            Words = GetWords(_lines);
        }

        private List<Word> GetWords(string[] lines)
        {
            List<Word> words=new List<Word>();
            Word word = new Word();
            MeaningGroup group = new MeaningGroup();

            foreach (var line in lines)
            {
                word = new Word();
                if (!line.Contains('؛'))
                {
                    word.Name = line;
                    word.SynAcros = new List<MeaningGroup>();
                    continue;
                }

                string[] split1 = line.Split('؛');

                word.Name = split1[0];

                List<string> split2 = split1[1].Split('|').ToList();

                foreach (var part in split2)
                {
                    group = new MeaningGroup();
                    if (!part.Contains("≠"))
                    {
                        group.Syns = part.Split('،').ToList();
                        group.Acros = new List<string>();
                    }
                    else
                    {
                        group.Syns = part.Split('≠')[0].Split('،').ToList();
                        group.Acros = part.Split('≠')[1].Split('،').ToList();
                    }

                    word.SynAcros.Add(group);
                }

                words.Add(word);
            }

            return words;
        }


        public int GetCount()
        {
            int count = 0;
            foreach (var word in Words)
            {
                foreach (var group in word.SynAcros)
                {
                    count += group.Syns.Count + group.Acros.Count;
                }
            }
            return count;
        }
    }
}
