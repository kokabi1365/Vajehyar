using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Animation;
using Octokit;
using Vajehdan.Properties;
using Application = System.Windows.Application;

namespace Vajehdan
{
    public static class Helper
    {
        public static async void CheckNewVersion()
        {
            try
            {
                var client = new GitHubClient(new ProductHeaderValue("Vajehdan"));

                Release lastRelease =
                    await client.Repository.Release.GetLatest(GetSettings().GithubId, GetSettings().GithubRepo);

                var latestVersion = new Version(lastRelease.TagName);
                var currentVersion = Assembly.GetEntryAssembly()?.GetName().Version;

                GetSettings().HasNewVersion = latestVersion > currentVersion;
            }
            catch (Exception ex)
            {
                ex.Log();
            }
        }

        public static void OpenAppOrWebsite(string path)
        {
            try
            {
                using var compiler = new Process
                {
                    StartInfo =
                    {
                        FileName = path, UseShellExecute = true
                    }
                };
                compiler.Start();
            }
            catch (Exception ex)
            {
                ex.Log();
            }

        }

        public static  void Log(this Exception ex)
        {
            var text = $"\n\n{DateTime.Now}\n{ex.ToStringDemystified()}";
            string path = Path.Combine(GetAppFolderPath(), "log.txt");
            File.AppendAllText(path, text);
        }

        public static string ToSemVersion(this Version ver)
        {
            int major = ver.Major;
            int minor = ver.Minor;
            int patch = ver.Build;

            if (major != 0 && minor != 0 && patch == 0)
                return $"{major}.{minor}";

            if (major != 0 && minor == 0 && patch == 0)
                return $"{major}";

            return $"{major}.{minor}.{patch}";
        }

        public static string RemoveDiacritics(this string str)
        {
            return str
                .Replace("\u064B", "") // ـً
                .Replace("\u064C", "") // ـٌ
                .Replace("\u064D", "") // ـٍ
                .Replace("\u064E", "") // ـَ
                .Replace("\u064F", "") // ـُ
                .Replace("\u0650", "") // ـِ
                .Replace("\u0651", "") // ـّ
                .Replace("\u0652", "") // ـْ
                .Replace("\u0654", "") // ـٔ
                .Replace("\u0647\u0654", "ه") // هٔ in standard persian keyboard
                .Replace("\u06c0", "ه") // ۀ in non-standard persian keyboard
                .Replace("ة", "ه")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                .Replace("ؤ", "و")
                .Replace("آ", "ا")
                .Replace("أ", "ا")
                .Replace("إ", "ا");
        }

        public static bool SearchWholeWord(this string input, string textToFind)
        {
            return Regex.IsMatch(input, $"\\b{textToFind}\\b", RegexOptions.CultureInvariant);
        }

        public static IEnumerable<T> GetWindow<T>() where T : Window, new()
        {
            return Application.Current.Windows.OfType<T>();
        }

        public static void ShowWindow<T>() where T : Window, new()
        {
            var existingWindow = Application.Current.Windows.OfType<T>()
                .SingleOrDefault();

            if (existingWindow == null)
            {
                new T().Show();
                return;
            }

            existingWindow.WindowState = WindowState.Normal;
            existingWindow.Activate();
        }

        public static bool IsAllCharEnglish(this string input)
        {
            foreach (var item in input.ToCharArray())
            {
                if (!char.IsLower(item) && !char.IsUpper(item) && !char.IsDigit(item) && !char.IsWhiteSpace(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<string> Combinations(string template, char placeholder)
        {
            int firstPlaceHolder = template.IndexOf(placeholder);
            if (firstPlaceHolder == -1)
                return new[] { template };

            var prefix = template.Substring(0, firstPlaceHolder);
            var suffix = template.Substring(firstPlaceHolder + 1);

            var recursiveCombinations = Combinations(suffix, placeholder);


            return
                from chr in new[] { " ", "\u200c", string.Empty }
                from recSuffix in recursiveCombinations
                select prefix + chr + recSuffix;
        }

        public static Settings GetSettings() => Settings.Default;

        public static string GetAppFolderPath() => AppDomain.CurrentDomain.BaseDirectory;

        public static string[] SplitAll(this string str) => str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitTwo(this string str) => str.Split(new[] { "\r\n", "\n" }, 2, StringSplitOptions.RemoveEmptyEntries);

        public static void MakeWindowCenterScreen(Window window)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = window.Width;
            double windowHeight = window.Height;
            window.Left = screenWidth / 2 - windowWidth / 2;
            window.Top = screenHeight / 2 - windowHeight / 2;
        }

        public static void SlideUpAnimation(Window window)
        {
            var anim1 = new DoubleAnimation
            {
                From = window.Top + 100,
                To = window.Top,
                SpeedRatio = 8,
                EasingFunction = new QuadraticEase()
            };
            window.BeginAnimation(Window.TopProperty, anim1);

            var anim2 = new DoubleAnimation
            {
                From = 0,
                To = 1,
                SpeedRatio = 4,
                EasingFunction = new QuadraticEase()
            };
            window.BeginAnimation(UIElement.OpacityProperty, anim2);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var r = new Random();
            return enumerable.OrderBy(_ => r.Next()).ToList();
        }

        public static T GetActiveWindow<T>() where T:Window
        {
            return Application.Current.Windows.OfType<T>().SingleOrDefault(x => x.IsActive);
        }
    }
}
