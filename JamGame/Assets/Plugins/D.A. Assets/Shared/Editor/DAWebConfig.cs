using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEditor.Callbacks;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.Shared
{
    public class DAWebConfig
    {
        public static WebConfig WebConfig => webConfig;
        private static WebConfig webConfig = default;

        internal static bool IsWebConfigRequested => wcRequested;
        private static bool wcRequested = false;

        [DidReloadScripts]
        private static void OnScriptsReload()
        {
            GetWebConfig();
        }

        private static void GetWebConfig()
        {
            try
            {
                Thread t = new Thread(() =>
                {
                    string url = "https://da-assets.github.io/site/files/webConfig.json";
                    string json = new WebClient().DownloadString(url);

                    webConfig = JsonUtility.FromJson<WebConfig>(json);
                });

                t.Start();
            }
            catch (WebException ex)
            {
                Debug.LogError(ex);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                wcRequested = true;
            }
        }
    }

    [Serializable]
    public struct WebConfig
    {
        [SerializeField] List<Asset> assets;
        public List<Asset> Assets => assets;
    }

    [Serializable]
    public struct Asset
    {
        [SerializeField] string name;
        [SerializeField] AssetType assetType;
        [SerializeField] int oldVersionDaysCount;
        [SerializeField] List<AssetVersion> versions;

        public string Name => name;
        public AssetType Type => assetType;
        public int OldVersionDaysCount => oldVersionDaysCount;
        public List<AssetVersion> Versions => versions;
    }

    [Serializable]
    public struct AssetVersion
    {
        [SerializeField] string version;
        [SerializeField] VersionType versionType;
        [SerializeField] string releaseDate;
        [SerializeField] string description;

        public string Version => version;
        public VersionType VersionType => versionType;
        public string ReleaseDate => releaseDate;
        public string Description => description;
    }

    public enum AssetType
    {
        fcu = 1,
        dab = 2,
        uitk = 3
    }

    public enum VersionType
    {
        stable = 0,
        beta = 1,
        buggy = 2
    }
}
