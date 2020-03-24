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

        public static List<string[]> Motaradef()
        {
            string[] lines = Properties.Resources.Motaradef_Motazad.Split('\n');
            List<string[]> words=new List<string[]>();
            lines.ForEach(l => words.Add(l.Split('،')));
            return words;
        }

        public static List<string[]> Teyfi()
        {
            string[] lines = Properties.Resources.Motaradef_Motazad.Split('\n');
            List<string[]> words = new List<string[]>();
            lines.ForEach(l => words.Add(l.Split('،')));
            return words;
        }

        public static List<string> Emlaei()
        {
            string[] lines = Properties.Resources.Motaradef_Motazad.Split('\n');
            List<string> words = new List<string>();
            words.AddRange(lines);
            return words;
        }

        public static string[] Motaradef1()
        {
            return Properties.Resources.Motaradef_Motazad.Split('\n');

        }

    }

    
}
