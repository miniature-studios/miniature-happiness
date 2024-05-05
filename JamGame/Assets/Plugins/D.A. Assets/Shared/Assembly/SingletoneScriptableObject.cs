using UnityEngine;

namespace DA_Assets.Shared
{
    ///  [CreateAssetMenu(menuName = DaGlobalVariables.Publisher + "/SingletoneScriptableObject")]
    /// <summary>
    /// Creates a single instance of your ScriptableObject in "Assets/Resources" folder and makes it accessible via YourClass.Instance.
    /// </summary>
    public class SingletoneScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        public static T instance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null)
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");

                    if (guids.Length == 0)
                    {
                        Debug.LogWarning($"ScriptableObject '{typeof(T).Name}' not found in your project.");
                        return default(T);
                    }

                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);

                    instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);

                    var x = (instance as SingletoneScriptableObject<T>);

                    if (x != null)
                        (instance as SingletoneScriptableObject<T>).OnCreateInstance();
                }
#endif
                return instance;
            }
        }

        public virtual void OnCreateInstance() { }
    }
}
