using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Lucene.Net.Analysis.Fa;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Microsoft.Win32;
using Octokit;
using SpellChecker.Net.Search.Spell;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Application = System.Windows.Application;
using Version = Lucene.Net.Util.Version;

namespace Vajehdan.Utility
{
    public static class Helper
    {
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
                ? Application.Current.Windows.OfType<T>().Any()
                : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public static bool isValueExist(RegistryKey basedKey, string value)
        {
            return basedKey.GetValue(value, null) != null;
        }

        public static async Task<bool> IsIdle(this TextBox textBox)
        {
            string txt = textBox.Text;
            await Task.Delay(2000);
            return txt == textBox.Text;
        }

        public static void MakeIndex()
        {
            var indexLocation = @"Index";
            var dir = FSDirectory.Open(indexLocation);
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            var writer = new IndexWriter(dir, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (var list in Database.Instance.words_motaradef)
            {
                Document doc = new Document();
                foreach (var word in list)
                {
                    doc.Add(new Field("word", word, Field.Store.YES, Field.Index.ANALYZED));
                }

                writer.AddDocument(doc);
            }
        }

        public static List<string> SimilarWords(string term)
        {
            MakeIndex();
            string indexPath = "Index";
            var indexReader = IndexReader.Open(FSDirectory.Open(indexPath), readOnly: true);

            // Create the SpellChecker
            var spellChecker = new SpellChecker.Net.Search.Spell.SpellChecker(FSDirectory.Open(indexPath));

            // Create SpellChecker Index
            spellChecker.CreateSearcher(new SimpleFSDirectory(new DirectoryInfo(indexPath))); 

            //Suggest Similar Words
            var results = spellChecker.SuggestSimilar(term, 5, null, null, true);
            return results.ToList();
        }
    }


}
