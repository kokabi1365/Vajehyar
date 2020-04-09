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
        public static string[][] GetWords(DatabaseType type)
        {
            string[] lines = { };

            switch (type)
            {
                case DatabaseType.Motaradef:
                    lines= Properties.Resources.Motaradef_Motazad.Split('\n');
                    break;
                case DatabaseType.Teyfi:
                    lines = Properties.Resources.Teyfi.Split('\n');
                    break;
                case DatabaseType.Emlaei:
                    lines = Properties.Resources.Emlaei.Split('\n');
                    break;
            }
            
            var words = new List<string[]>();
            lines.ForEach(l => words.Add(l.Split('،')));
            return words.ToArray();
        }

    }

    public enum DatabaseType{
        Motaradef,
        Teyfi,
        Emlaei
    }

    
}
