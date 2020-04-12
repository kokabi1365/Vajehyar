using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Vajehdan.Properties;
using Vajehdan.Views;

namespace Vajehdan
{
    public static class Helper
    {
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

        public static bool IsBeta(this Version version) => version < new Version(1, 0, 0);

        public static string ToSemVersion(this Version ver)
        {
            int major = ver.Major;
            int minor = ver.Minor;
            int patch = ver.Build;
            return $"{major}.{minor}.{patch}";
        }

        public static string ToPlainText(this string s)
        {
            return s.Trim().RemoveDiacritics().Replace("ي", "ی").Replace("ك", "ک"); }

        public static string RemoveDiacritics(this string s)
        {
            string normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}
