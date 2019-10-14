using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Octokit;

namespace Vajehyar
{
    public sealed class Database
    {
        public static Database Instance { get; } = new Database();

        public List<Word> MotaradefMotazadList { get; }
        public List<Word> TeyfiList { get; }
        public List<Word> EmlaeiList { get; }=new List<Word>();

        private Database()
        {
            string dict_motaradef_Motazad = Properties.Resources.Motaradef_Motazad;
            string dict_Teyfi = Properties.Resources.Teyfi;
            string dict_emlaei = Properties.Resources.Emlaei;

            string[] _lines1 = dict_motaradef_Motazad.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            string[] _lines2 = dict_Teyfi.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            string[] _lines3 = dict_emlaei.Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            MotaradefMotazadList = GetWords(_lines1);
            TeyfiList = GetWords(_lines2);

            _lines3.ToList().ForEach(line =>
            {
                EmlaeiList.Add(new Word() { Name = line});
            });

        }

        private List<Word> GetWords(string[] lines)
        {
            var words = new List<Word>();

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
            MotaradefMotazadList.ForEach(word => { count += word.Meanings.Count; });
            TeyfiList.ForEach(word => { count += word.Meanings.Count; });
            count += EmlaeiList.Count;
            return count;
        }
    }
}
