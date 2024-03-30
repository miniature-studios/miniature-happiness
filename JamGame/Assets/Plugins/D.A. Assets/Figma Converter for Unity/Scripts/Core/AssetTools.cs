using DA_Assets.FCU.Extensions;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class AssetTools : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [HideInInspector, SerializeField] bool needShowRateMe;
        public bool NeedShowRateMe
        {
            get
            {
                if (needShowRateMe)
                {
#if UNITY_EDITOR
                    if (UnityEditor.EditorPrefs.GetInt(FcuConfig.RATEME_PREFS_KEY, 0) == 1)
                        return false;
#else
                    return false;
#endif
                }

                return needShowRateMe;
            }
            set => needShowRateMe = value;
        }

        private ResolutionData resolutionData;
        public ResolutionData ResolutionData { get => resolutionData; set => resolutionData = value; }

        public void CacheResolutionData()
        {
            bool received = monoBeh.DelegateHolder.GetGameViewSize(out Vector2 gameViewSize);

            this.ResolutionData = new ResolutionData
            {
                GameViewSizeReceived = received,
                GameViewSize = gameViewSize
            };
        }

        public IEnumerator DestroyChilds()
        {
            int count = monoBeh.transform.ClearChilds();
            DALogger.Log(FcuLocKey.log_current_canvas_childs_destroy.Localize(monoBeh.Guid, count));
            yield return null;
        }

        public IEnumerator DestroyLastImportedFrames()
        {
            foreach (var item in monoBeh.CurrentProject.LastImportedFrames)
            {
                item.GameObject.Destroy();
            }

            monoBeh.CurrentProject.LastImportedFrames.Clear();
            yield return null;
        }

        public static void CreateFcuOnScene()
        {
            GameObject go = MonoBehExtensions.CreateEmptyGameObject();

            go.TryAddComponent(out FigmaConverterUnity fcu);
            go.name = string.Format(FcuConfig.Instance.CanvasGameObjectName, fcu.Guid);

            fcu.CanvasDrawer.AddCanvasComponent();
        }

        public void StopImport()
        {
            monoBeh.StopDARoutines();
        }

        public void RestoreResolutionData()
        {
            if (this.ResolutionData.GameViewSizeReceived)
            {
                monoBeh.DelegateHolder.SetGameViewSize(this.ResolutionData.GameViewSize);
            }
        }

        internal void ShowRateMe()
        {
            try
            {
                int componentsCount = monoBeh.TagSetter.TagsCounter.Values.Sum();
                int importErrorCount = monoBeh.AssetTools.GetConsoleErrorCount();

                if (importErrorCount > 0 || componentsCount < 1)
                {
                    needShowRateMe = false;
                    return;
                }

                needShowRateMe = true;
            }
            catch (Exception ex)
            {
                needShowRateMe = false;
                monoBeh.Log(ex);
            }
        }

        public static void MakeActiveSceneDirty()
        {
#if UNITY_EDITOR
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
#endif
        }

        public int GetConsoleErrorCount()
        {
#if UNITY_EDITOR
            try
            {
                Type logEntriesType = System.Type.GetType("UnityEditor.LogEntries, UnityEditor");
                if (logEntriesType == null)
                {
                    return 0;
                }

                MethodInfo getCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
                if (getCountsByTypeMethod == null)
                {
                    return 0;
                }

                int errorCount = 0;
                int warningCount = 0;
                int logCount = 0;
                object[] args = new object[] { errorCount, warningCount, logCount };

                getCountsByTypeMethod.Invoke(null, args);

                errorCount = (int)args[0];
                warningCount = (int)args[1];
                logCount = (int)args[2];

                return errorCount;
            }
            catch (Exception)
            {
                return 1;
            }
#else
            return 1;
#endif
        }
    }

    public struct ResolutionData
    {
        public bool GameViewSizeReceived { get; set; }
        public Vector2 GameViewSize { get; set; }
    }
}