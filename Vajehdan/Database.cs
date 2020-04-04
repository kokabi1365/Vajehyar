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
        public static string[][] Motaradef()
        {
            string[] lines = Properties.Resources.Motaradef_Motazad.Split('\n');
            var words = new List<string[]>();
            lines.ForEach(l => words.Add(l.Split('،')));
            return words.ToArray();
        }

        public static string[][] Teyfi()
        {
            string[] lines = Properties.Resources.Teyfi.Split('\n');
            var words = new List<string[]>();
            lines.ForEach(l => words.Add(l.Split('،')));
            return words.ToArray();
        }

        public static string[] Emlaei()
        {
            return Properties.Resources.Emlaei.Split('\n');
        }

    }

    
}
