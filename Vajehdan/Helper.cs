using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Octokit;
using Vajehdan.Properties;
using Application = System.Windows.Application;

namespace Vajehdan
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

        public static async Task CheckUpdate()
        {
            string changes = string.Empty;
            try
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("App"));
                Release lastRelease = await client.Repository.Release.GetLatest(Settings.Default.GithubId, Settings.Default.GithubRepo);
                List<Release> allReleases = (await client.Repository.Release.GetAll(Settings.Default.GithubId, Settings.Default.GithubRepo)).ToList();
                Version latestVersion = new Version(lastRelease.TagName);
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (latestVersion > currentVersion)
                {
                    foreach (Release release in allReleases)
                    {
                        Version ver = new Version(release.TagName);
                        if (ver > currentVersion)
                        {
                            string v = release.TagName;
                            if (ver.IsBeta())
                            {
                                v += " (آزمایشی)";
                            }
                            changes += $"نسخۀ {v} -------------------------------" +
                                       Environment.NewLine + release.Body + Environment.NewLine + Environment.NewLine;
                        }
                    }

                    new ChangelogWindow(changes).Show();
                }
            }
            catch { }
        }

        public static bool IsBeta(this Version version)
        {
            return version < new Version(1, 0, 0);
        }

        public static string ToSemVersion(this Version ver)
        {
            int major = ver.Major;
            int minor = ver.Minor;
            int patch = ver.Build;
            return $"{major}.{minor}.{patch}";
        }

        public static async Task<bool> GetIdle(this TextBox txb, int whatMillisecond)
        {
            string txt = txb.Text;
            await Task.Delay(whatMillisecond);
            return txt == txb.Text;
        }

        public static String RemoveDiacritics(this String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }


}
