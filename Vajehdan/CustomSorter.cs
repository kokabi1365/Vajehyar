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
            string item1 = string.Join("", x);
            string item2 = string.Join("", y);
          
            string filter = mainWindow.FilterString;

            int a = item1?.IndexOf(filter) ?? -1;
            int b = item2?.IndexOf(filter) ?? -1;
            return a.CompareTo(b);
        }

    }
}