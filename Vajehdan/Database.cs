using System.Collections.Generic;
using System.Linq;
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
                    lines= Properties.Resources.Motaradef.Split('\n');
                    break;
                case DatabaseType.Teyfi:
                    lines = Properties.Resources.Teyfi.Split('\n');
                    break;
                case DatabaseType.Emlaei:
                    lines = Properties.Resources.Emlaei.Split('\n');
                    break;
            }
            
            var words = new List<string[]>();
            foreach (var l in lines)
            {
                string[] ws = l.Split('،');
                ws.ForEach(s => s.Trim());
                words.Add(ws);

            } 
                
            return words.ToArray();
        }

        public static List<string> GetAllWords()
        {
            List<string> words = new List<string>();
            
            var lines = Properties.Resources.Motaradef.Split('\n')
                .Concat(Properties.Resources.Teyfi.Split('\n'))
                .Concat(Properties.Resources.Emlaei.Split('\n'));

            foreach (string line in lines)
            {
                words.AddRange(line.Split('،'));
            }

            return words.Select(s => s.Trim()).Distinct()
                .ToList();
        }

    }

    public enum DatabaseType{
        Motaradef,
        Teyfi,
        Emlaei
    }

    
}
