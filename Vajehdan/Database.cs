using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Octokit;
using Syncfusion.Data.Extensions;

namespace Vajehdan
{
    public sealed class Database
    {
        public static List<Entry> Motaradef()
        {
            List<Entry> entries=new List<Entry>();
            
            foreach (var line in Properties.Resources.Motaradef_Motazad.Split('\n'))
            {
                Entry entry=new Entry();
                entry.Meanings = line;
                entry.MeaningsArray = line.Split('،');
                entries.Add(entry);
            }

            return entries;
        }

        public static List<Entry> Teyfi()
        {
            List<Entry> entries = new List<Entry>();

            foreach (var line in Properties.Resources.Teyfi.Split('\n'))
            {
                Entry entry = new Entry();
                entry.Meanings = line;
                entry.MeaningsArray = line.Split('،');
                entries.Add(entry);
            }

            return entries;
        }

        public static List<Entry> Emlaei()
        {
            List<Entry> entries = new List<Entry>();

            foreach (var line in Properties.Resources.Emlaei.Split('\n'))
            {
                Entry entry = new Entry();
                entry.Meanings = line;
                entry.MeaningsArray = line.Split('،');
                entries.Add(entry);
            }

            return entries;
        }

    }

    
}
