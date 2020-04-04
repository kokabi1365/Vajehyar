using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Vajehdan
{
    internal class CustomSorter : IComparer
    {
        private MainWindow mainWindow;

        public CustomSorter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public int Compare(object x, object y)
        {
            //string item1 = string.Join("", (x as List<string>)?.ToArray());
            //string item2 = string.Join("", (y as List<string>)?.ToArray());
            var item1 = (x as Entry).Meanings;
            var item2 = (y as Entry).Meanings;
            string filter = mainWindow.FilterString;

            /*if (Regex.Match(s, @"\b" + $"{filter}" + @"\b", RegexOptions.IgnoreCase).Success)
            {
                return 1;
            }*/

            int a = item1?.IndexOf(filter) ?? -1;
            int b = item2?.IndexOf(filter) ?? -1;
            return a.CompareTo(b);
        }

    }
}