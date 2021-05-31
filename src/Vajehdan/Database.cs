using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Vajehdan.Controls;
using Vajehdan.Models;
using Vajehdan.Properties;
using Application = System.Windows.Application;

namespace Vajehdan
{
    [SupportedOSPlatform("windows7.0")]
    public sealed class Database
    {
        public static List<Dic> Dictionaries { get; set; } = new();
        public static List<string> EmlaeiEntries { get; set; } = new();

        public static void LoadData()
        {
            Dictionaries.Add(GetDic(Resources.Motaradef));
            Dictionaries.Add(GetDic(Resources.Teyfi));
            Dictionaries.Add(GetDic(Resources.Sereh));
            Dictionaries.Add(GetDic(Resources.Farhangestan));
            Dictionaries.Add(GetDic(Resources.Ganjvar));

            EmlaeiEntries = Resources.Emlaei
                .SplitAll()
                .ToList();
        }

        public static Dic GetDic(string fileContent)
        {
            try
            {
                using var sr = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));

                var dic = new Dic
                {
                    Name = (sr.ReadLine())?.Trim(),
                    Description = (sr.ReadLine())?.Trim()
                };

                if (string.IsNullOrWhiteSpace(dic.Name) || string.IsNullOrWhiteSpace(dic.Description))
                    return null;


                var remaining = (sr.ReadToEnd()).Trim().Split('*');


                foreach (var e in remaining.AsParallel())
                {
                    string[] splitted = e.SplitTwo();
                    var entry = new Entry(splitted[0], splitted[1]);
                    dic.Entries.Add(entry);
                }

                if (dic.Entries.Count > 10)
                    return dic;
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return null;
        }

        public static SearchResult Search(Dic dic, string query)
        {

            var searchTerms = query.Split('،').Select(t => t.Trim());


            var searchResult = new SearchResult
            {
                DicName = dic.Name,
                DicDescription = dic.Description
            };


            var titleEqual = new SortedSet<Entry>(dic.Entries)
                .Where(entry => string.Join(' ', searchTerms) == entry.Title);


            var titleContain = new List<Entry>(dic.Entries)
                .Where(entry =>
                    searchTerms.All(term => entry.Title.SearchWholeWord(term)));

            var definitionContain = new List<Entry>(dic.Entries)
                .Where(entry =>
                    searchTerms.All(term => entry.Title.SearchWholeWord(term) || entry.Content.SearchWholeWord(term)));

            if (Helper.GetSettings().RefreshResult)
            {
                definitionContain = new List<Entry>(definitionContain.Shuffle());
            }

            var finalSortedList = titleEqual
                .Concat(titleContain)
                .Concat(definitionContain);

            finalSortedList = finalSortedList.Distinct().Take(Helper.GetSettings().NumberOfResult);

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                searchResult.ResultDoc = ConvertToFlowDocument(finalSortedList);
            });

            return searchResult;
        }


        public static SearchResult SearchGanjvar(Dic dic, string query)
        {
            var splitted = query.Split('|');
            var searchTerms = splitted[0].Trim().Split('،').Select(t => t.Trim());
            var searchParameters = splitted[1].Trim().Split('،').Select(p => p.Trim());

            var searchResult = new SearchResult
            {
                DicName = dic.Name,
                DicDescription = dic.Description
            };

            var foundedItems = dic.Entries
                .Where(entry => searchParameters.All(p => entry.Title.SearchWholeWord(p)))
                .Where(entry => searchTerms.All(term => entry.Content.SearchWholeWord(term)));

            if (Helper.GetSettings().RefreshResult)
            {
                foundedItems = foundedItems.Shuffle();
            }


            foundedItems = foundedItems.Distinct().Take(Helper.GetSettings().NumberOfResult).ToList();

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                searchResult.ResultDoc = ConvertToFlowDocument(foundedItems);
            });

            return searchResult;


        }

        public static FlowDocument ConvertToFlowDocument(IEnumerable<Entry> entries)
        {
            var flowDocument = new FlowDocument
            {
                ContextMenu = new CustomContextMenu()
            };

            foreach (var entry in entries)
            {
                var p = new Paragraph();
                var title = new Run
                {
                    FontWeight = FontWeights.Regular,
                    Foreground = new SolidColorBrush(Colors.Red),
                    Text = entry.Title + "\n"
                };
                var content = new Run { FontWeight = FontWeights.Thin, Text = entry.Content };
                p.Inlines.Add(title);
                p.Inlines.Add(content);
                flowDocument.Blocks.Add(p);
            }

            return flowDocument;
        }
    }
}


