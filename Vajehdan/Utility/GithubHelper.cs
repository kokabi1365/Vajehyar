using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Vajehdan.Properties;
using Vajehdan.Windows;


namespace Vajehdan.Utility
{
    public class GithubHelper
    {
        private static readonly string GithubUserName = Settings.Default.GithubId;
        private static readonly string GithubRepoName = Settings.Default.GithubRepo;

        public static async Task CheckUpdate()
        {
            string changes = string.Empty;
            try
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("App"));
                Release lastRelease = await client.Repository.Release.GetLatest(GithubUserName, GithubRepoName);
                List<Release> allReleases = (await client.Repository.Release.GetAll(GithubUserName, GithubRepoName)).ToList();
                Version latestVersion = new Version(lastRelease.TagName);
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (latestVersion > currentVersion)
                {
                    foreach (Release release in allReleases)
                    {
                        Version ver=new Version(release.TagName);
                        if (ver>currentVersion)
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
       
    }
}

