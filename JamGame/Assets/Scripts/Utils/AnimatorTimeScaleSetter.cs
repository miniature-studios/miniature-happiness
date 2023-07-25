using UnityEngine;

#if UNITY_EDITOR 

using System.IO;
using UnityEditor;

namespace Utils
{
    public class AnimatorTimeScaleSetterTool
    {
        [MenuItem("Tools/AnimatorTimeScaleSetter/Set time scale for hierarchy animators")]
        private static void Hierarchy()
        {
            Animator[] animators = GameObject.FindObjectsOfType<Animator>();
            foreach (Animator animator in animators)
            {
                AnimatorTimeScaleSetter ignore = animator.GetComponent<AnimatorTimeScaleSetter>();
                if (ignore != null)
                {
                    continue;
                }

                Canvas canvas = animator.GetComponentInParent<Canvas>(true);
                animator.updateMode = canvas == null ? AnimatorUpdateMode.Normal : AnimatorUpdateMode.UnscaledTime;
            }

            Debug.Log("Hierarchy animators setup done");
        }

        [MenuItem("Tools/AnimatorTimeScaleSetter/Set time scale for prefab animators")]
        private static void Prefabs()
        {
            string[] files = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                bool dirty = false;
                GameObject prefab = PrefabUtility.LoadPrefabContents(file);
                Animator[] animators = prefab.GetComponentsInChildren<Animator>();
                foreach (Animator animator in animators)
                {
                    AnimatorTimeScaleSetter ignore = animator.GetComponent<AnimatorTimeScaleSetter>();
                    if (ignore != null)
                    {
                        continue;
                    }

                    bool ui_folder = file.Contains("Assets\\Prefabs\\UI");

                    animator.updateMode = ui_folder ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
                    dirty = true;
                }

                if (dirty)
                {
                    _ = PrefabUtility.SaveAsPrefabAsset(prefab, file);
                }

                PrefabUtility.UnloadPrefabContents(prefab);
            }

            Debug.Log("Prefab animators setup done");
        }
    }
}

#endif

namespace Utils
{
    [AddComponentMenu("Utils.IgnoreAnimatorTimeScaleSetter")]
    public class AnimatorTimeScaleSetter : MonoBehaviour
    {
        // Not called IgnoreAnimatorTimeScaleSetter because file name should match the class name.
    }
}
