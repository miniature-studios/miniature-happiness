using DA_Assets.FCU.Model;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;

#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [CreateAssetMenu(menuName = DAConstants.Publisher + "/FcuConfig")]
    public class FcuConfig : SingletoneScriptableObject<FcuConfig>
    {
        [SerializeField] string productVersion;
        public string ProductVersion => productVersion;

        [Space]

        [SerializeField] List<TagConfig> tags;
        public List<TagConfig> TagConfigs => tags;

        [SerializeField] List<DependencyItem> dependencies;
        public List<DependencyItem> Dependencies => dependencies;

        [Header("File names")]
        [SerializeField] string webLogFileName;
        public string WebLogFileName => webLogFileName;

        [SerializeField] string localizationFileName;
        public string LocalizationFileName => localizationFileName;

        [Header("Formats")]
        [SerializeField] string dateTimeFormat1;
        public string DateTimeFormat1 => dateTimeFormat1;
        [SerializeField] string dateTimeFormat2;
        public string DateTimeFormat2 => dateTimeFormat2;

        [Header("GameObject names")]
        [SerializeField] string canvasGameObjectName;
        public string CanvasGameObjectName => canvasGameObjectName;
        [SerializeField] string i2LocGameObjectName;
        public string I2LocGameObjectName => i2LocGameObjectName;

        [Header("Values")]
        [SerializeField] int maxCachedFilesCount;
        public int MaxCachedFilesCount => maxCachedFilesCount;

        [SerializeField] int maxLogFilesCount;
        public int MaxLogFilesCount => maxLogFilesCount;

#if UNITY_EDITOR
        [Header("TextureImporter Settings")]
        [SerializeField] bool generateMipMaps = false;
        public bool GenerateMipMaps => generateMipMaps;

        [SerializeField] bool crunchedCompression;
        public bool CrunchedCompression => crunchedCompression;

        [Tooltip("This value is used only when flag 'CrunchedCompression' is active.")]
        [SerializeField] int crunchedCompressionQuality;
        public int CrunchedCompressionQuality => crunchedCompressionQuality;

        [SerializeField] UnityEditor.TextureImporterCompression textureImporterCompression;
        public UnityEditor.TextureImporterCompression TextureImporterCompression => textureImporterCompression;
#endif

        [Header("Api")]
        [SerializeField] bool https;
        public bool Https => https;

        [SerializeField] int apiRequestsCountLimit = 2;
        public int ApiRequestsCountLimit => apiRequestsCountLimit;

        [SerializeField] int apiTimeoutSec = 5;
        public int ApiTimeoutSec => apiTimeoutSec;

        [SerializeField] int chunkSizeGetNodes;
        public int ChunkSizeGetNodes => chunkSizeGetNodes;

        [SerializeField] int chunkSizeGetSpriteLinks;
        public int ChunkSizeGetSpriteLinks => chunkSizeGetSpriteLinks;

        [SerializeField] int chunkSizeDownloadSprites;
        public int ChunkSizeDownloadSprites => chunkSizeDownloadSprites;

        [SerializeField] int inspectorDepth = 2;
        public int InspectorDepth => inspectorDepth;

        [SerializeField] string gFontsApiKey;
        public string GoogleFontsApiKey { get => gFontsApiKey; set => gFontsApiKey = value; }

        [Header("Other")]
        [SerializeField] char realTagSeparator = '-';
        public char RealTagSeparator => realTagSeparator;

        [SerializeField] int maxRenderSize = 4096;
        public int MaxRenderSize => maxRenderSize;

        [SerializeField] string rateMePrefsKey = "fcuRateMeShown";
        public string RateMePrefsKey => rateMePrefsKey;

        [SerializeField] int renderUpscaleFactor = 2;
        public int RenderUpscaleFactor => renderUpscaleFactor;

        [SerializeField] Sprite spriteX32;
        public Sprite SpriteX32 => spriteX32;

#if DABUTTON_EXISTS
        [SerializeField] DATargetGraphic defaultTargetGraphic;
        public DATargetGraphic DefaultTargetGraphic => defaultTargetGraphic;
#endif

        /////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////

        public const string ProductName = "Figma Converter for Unity";
        public const string DestroyChilds = "Destroy childs";
        public const string SetFcuToSyncHelpers = "Set current FCU to SyncHelpers";
        public const string CompareTwoObjects = "Compare two selected objects";
        public const string DestroyLastImported = "Destroy last imported frames";
        public const string DestroySyncHelpers = "Destroy SyncHelpers";
        public const string CreatePrefabs = "Create Prefabs";
        public const string ResetToPrefabState = "Reset to prefab state";
        public const string ResetAllComponents = "Reset all components to prefab state";
        public const string Create = "Create";
        public const string RATEME_PREFS_KEY = "DONT_SHOW_RATEME";
        public const char HierarchyDelimiter = '/';
        public const string OptimizeSyncHelpers = "Optimize SyncHelpers";

        public static string ClientId => "LaB1ONuPoY7QCdfshDbQbT";
        public static string ClientSecret => "E9PblceydtAyE7Onhg5FHLmnvingDp";
        public static string RedirectUri => "http://localhost:1923/";
        public static string AuthUrl => "https://www.figma.com/api/oauth/token?client_id={0}&client_secret={1}&redirect_uri={2}&code={3}&grant_type=authorization_code";
        public static string OAuthUrl => "https://www.figma.com/oauth?client_id={0}&redirect_uri={1}&scope=file_read&state={2}&response_type=code";

        private static string logPath;
        public static string LogPath
        {
            get
            {
                if (logPath.IsEmpty())
                    logPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");

                if (logPath.Contains("/"))
                    logPath = logPath.Replace("/", "\\");

                logPath.CreateFolderIfNotExists();

                return logPath;
            }
        }

        private static string cachePath;
        public static string CachePath
        {
            get
            {
                if (cachePath.IsEmpty())
                {
                    string tempFolder = Path.GetTempPath();
                    cachePath = Path.Combine(tempFolder, "FCU Cache");
                    cachePath = cachePath.Replace("/", "\\");
                }

                cachePath.CreateFolderIfNotExists();

                return cachePath;
            }
        }
    }
}