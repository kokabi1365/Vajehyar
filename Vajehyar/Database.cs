using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vajehyar
{
    public sealed class Database
    {
        public static Database Instance { get; } = new Database();

        public List<Word> MotaradefMotazadList { get; }
        public List<Word> TeyfiList { get; }

        private Database()
        {
            string dict_motaradef_Motazad = Properties.Resources.Motaradef_Motazad;
            string dict_Teyfi = Properties.Resources.Teyfi;

            string[]_lines1 = dict_motaradef_Motazad.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            string[] _lines2 = dict_Teyfi.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            MotaradefMotazadList = GetWords(_lines1);
            TeyfiList = GetWords(_lines2);
        }

        private List<Word> GetWords(string[] lines)
        {
            var words=new List<Word>();

            foreach (var line in lines)
            {
                var word = new Word();
                if (!line.Contains('؛'))
                {
                    word.Name = line;
                    word.Meanings = new List<string>();
                    continue;
                }

                string[] splitted = line.Split('؛');
                word.Name = splitted[0];
                word.Meanings = splitted[1].Split('،').ToList();
                words.Add(word);
            }

            return words;
        }


        public int GetCount()
        {
            int count = 0;
            MotaradefMotazadList.ForEach(word => { count += word.Meanings.Count;});
            TeyfiList.ForEach(word => { count += word.Meanings.Count;});
            return count;
        }
    }
}
