using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using Octokit;
using Application = System.Windows.Application;

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

        public static async Task<bool> UserKeepsTyping(TextBox textBox)
        {
            async Task<bool> UserKeepsTyping()
            {
                string txt = textBox.Text;
                await Task.Delay(200);
                return txt != textBox.Text;
            }

            return await UserKeepsTyping();
        }
    }


}
