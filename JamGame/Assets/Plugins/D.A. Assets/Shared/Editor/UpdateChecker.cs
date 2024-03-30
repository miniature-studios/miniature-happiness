using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DA_Assets.Shared
{
    public class UpdateChecker
    {
        private static Dictionary<AssetType, VersionCache> cachedVersionData = new Dictionary<AssetType, VersionCache>();
        private static DAInspector gui = DAInspector.Instance;

        public static void DrawVersionLine(AssetType assetType, string currentVersion)
        {
            if (!DAWebConfig.IsWebConfigRequested)
                return;

            if (!cachedVersionData.TryGetValue(assetType, out var vc))
            {
                WriteVersionData(assetType, Version.Parse(currentVersion));
            }

            VersionCache cache = cachedVersionData[assetType];

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.FlexibleSpace();

                    if (gui.LinkLabel(
                        new GUIContent(cache.CurrentVersionText, cache.CurrentVersionTooltip),
                        WidthType.Option,
                        cache.CurrentVersionStyle))
                    {
                        UnityEditor.PackageManager.UI.Window.Open(cache.Asset.Name);
                    }

                    if (cache.IsLatestVersion == false)
                    {
                        gui.Space5();
                        gui.Label10px("—", widthType: WidthType.Option);
                        gui.Space5();

                        if (gui.LinkLabel(
                            new GUIContent(cache.LatestVersionText, cache.LatestVersionTooltip),
                            WidthType.Option,
                            cache.LatestVersionStyle))
                        {
                            UnityEditor.PackageManager.UI.Window.Open(cache.Asset.Name);
                        }
                    }
                }
            });
        }

        private static void WriteVersionData(AssetType assetType, Version currentVersion)
        {
            try
            {
                if (DAWebConfig.WebConfig.IsDefault())
                {
                    throw new NullReferenceException("WebConfig is empty.");
                }

                Asset assetInfo = DAWebConfig.WebConfig.Assets.FirstOrDefault(x => x.Type == assetType);

                if (assetInfo.IsDefault())
                {
                    throw new NullReferenceException($"AssetInfo is empty for {assetType}");
                }

                VersionCache cache = new VersionCache
                {
                    Asset = assetInfo
                };

                AssetVersion? current = null;
                AssetVersion? last = null;

                try
                {
                    current = GetVersion(assetType, currentVersion);

                    if (current == null)
                    {
                        throw new NullReferenceException();
                    }

                    if (current.Value.VersionType == VersionType.buggy)
                    {
                        cache.CurrentVersionStyle = GuiStyle.RedLabel10px;
                    }
                    else
                    {
                        cache.CurrentVersionStyle = GuiStyle.Label10px;
                    }

                    cache.CurrentVersionText = $"{currentVersion} [current, {current.Value.VersionType}]";
                    cache.CurrentVersionTooltip = current.Value.Description;
                }
                catch
                {
                    SetDefaultCurrentVersionData(ref cache, currentVersion);
                }

                try
                {
                    last = assetInfo.Versions.Last();

                    if (last == null)
                    {
                        throw new NullReferenceException();
                    }

                    Version version2 = new Version(last.Value.Version);

                    int comparisonResult = currentVersion.CompareTo(version2);

                    if (comparisonResult > 0)//Version 1 is greater than Version 2
                    {
                        cache.IsLatestVersion = true;
                    }
                    else if (comparisonResult < 0)//Version 1 is less than Version 2
                    {
                        cache.IsLatestVersion = false;
                    }
                    else//Both versions are equal
                    {
                        cache.IsLatestVersion = true;
                    }

                    DateTime lastDt = DateTime.ParseExact(last.Value.ReleaseDate, "MMM d, yyyy", new System.Globalization.CultureInfo("en-US"));
                    DateTime nowDt = DateTime.Now;

                    TimeSpan timeDifference = nowDt - lastDt;
                    int differenceInDays = (int)timeDifference.TotalDays;
                    int differenceInHours = (int)timeDifference.TotalHours;
                    int differenceInMinutes = (int)timeDifference.TotalMinutes;

                    string dateStr = "";

                    if (differenceInMinutes < 0)
                    {
                        dateStr = $"near future";
                    }
                    else if (differenceInMinutes == 0)
                    {
                        dateStr = "just now";
                    }
                    else if (differenceInHours == 0)
                    {
                        dateStr = $"{differenceInMinutes} {Pluralize("minute", differenceInMinutes)} ago";
                    }
                    else if (differenceInDays == 0)
                    {
                        dateStr = $"{differenceInHours} {Pluralize("hour", differenceInHours)} ago";
                    }
                    else
                    {
                        dateStr = $"{differenceInDays} {Pluralize("day", differenceInDays)} ago";
                    }

                    cache.LatestVersionText = $"{last.Value.Version} [latest, {dateStr}, {last.Value.VersionType}]";

                    if (last.Value.VersionType == VersionType.buggy)
                    {
                        cache.LatestVersionStyle = GuiStyle.RedLabel10px;
                    }
                    else if (differenceInDays >= assetInfo.OldVersionDaysCount)
                    {
                        cache.LatestVersionStyle = GuiStyle.BlueLabel10px;
                    }
                    else
                    {
                        cache.LatestVersionStyle = GuiStyle.Label10px;
                    }

                    cache.LatestVersionTooltip = last.Value.Description;
                }
                catch
                {
                    SetDefaultLatestData(ref cache);
                }

                cachedVersionData[assetType] = cache;
            }
            catch (Exception)
            {
                VersionCache newCache = new VersionCache();
                SetDefaultCurrentVersionData(ref newCache, currentVersion);
                SetDefaultLatestData(ref newCache);
                cachedVersionData[assetType] = newCache;
            }
        }

        private static void SetDefaultCurrentVersionData(ref VersionCache cache, Version currentVersion)
        {
            cache.CurrentVersionText = $"{currentVersion} [current]";
            cache.CurrentVersionStyle = GuiStyle.LinkLabel10px;
            cache.CurrentVersionTooltip = "No description.";
        }

        private static void SetDefaultLatestData(ref VersionCache cache)
        {
            cache.LatestVersionText = $"no data [latest]";
            cache.LatestVersionStyle = GuiStyle.LinkLabel10px;
            cache.LatestVersionTooltip = "No description.";
        }

        private static AssetVersion? GetVersion(AssetType assetType, Version currentVersion)
        {
            try
            {
                Asset assetInfo = DAWebConfig.WebConfig.Assets.First(x => x.Type == assetType);

                foreach (AssetVersion assetVersion in assetInfo.Versions)
                {
                    Version ver = Version.Parse(assetVersion.Version);
                    int res = currentVersion.CompareTo(ver);

                    if (res == 0)
                    {
                        return assetVersion;
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        private static string Pluralize(string source, int count)
        {
            if (count == 1)
            {
                return source;
            }
            else
            {
                return source + "s";
            }
        }
    }

    internal struct VersionCache
    {
        internal Asset Asset { get; set; }
        internal string CurrentVersionText { get; set; }
        internal string CurrentVersionTooltip { get; set; }
        internal string LatestVersionText { get; set; }
        internal string LatestVersionTooltip { get; set; }
        internal GuiStyle CurrentVersionStyle { get; set; }
        internal GuiStyle LatestVersionStyle { get; set; }
        internal bool IsLatestVersion { get; set; }
    }
}