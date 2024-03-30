using DA_Assets.Shared.Extensions;
using System;

namespace DA_Assets.Shared
{
    public static class DALogger
    {
        public static string redColor = "#ff6e40";
        public static string blackColor = "black";
        public static string whiteColor = "white";
        public static string violetColor = "#8b00ff";
        public static string orangeColor = "#ffa500";

        public static void LogException(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }

        public static void LogError(string log)
        {
            log = log.SubstringSafe(15000);
            UnityEngine.Debug.LogError(log);
        }

        public static void LogWarning(string log)
        {
            log = log.SubstringSafe(15000);
            UnityEngine.Debug.LogWarning(log.TextColor(orangeColor).TextBold());
        }

        public static void Log(string log)
        {
            log = log.SubstringSafe(15000);
            string color = whiteColor;
#if UNITY_EDITOR
            color = UnityEditor.EditorGUIUtility.isProSkin ? whiteColor : blackColor;
#endif
            UnityEngine.Debug.Log(log.TextColor(color).TextBold());
        }

        public static void LogSuccess(string log)
        {
            UnityEngine.Debug.Log(log.TextColor(violetColor).TextBold());
        }
    }
}