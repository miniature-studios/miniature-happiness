using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0162

namespace DA_Assets.Shared.Extensions
{
    public static class MonoBehExtensions
    {
        public static List<T> GetComponentsInReverseOrder<T>(this GameObject parent) where T : Component
        {
            List<T> results = new List<T>();
            AddComponentsInReverseOrder(parent.transform);
            return results;

            void AddComponentsInReverseOrder(Transform current)
            {
                for (int i = current.childCount - 1; i >= 0; i--)
                {
                    AddComponentsInReverseOrder(current.GetChild(i));
                }

                T component = current.GetComponent<T>();
                if (component != null/* && !results.Contains(component)*/)
                {
                    results.Add(component);
                }
            }
        }

        /// <summary>
        /// Removes all components from the GameObject that have a 'RequireComponent' attribute pointing to the given component.
        /// </summary>
        /// <param name="component">The target component which other components might depend on.</param>
        /// <returns>True if any dependent components were removed, false otherwise.</returns>
        public static bool RemoveComponentsDependingOn(this UnityEngine.Component component)
        {
            bool removedAny = false; // Flag to indicate if we've removed any components.
            Type type = component.GetType();
            // Get all components present on the GameObject where the target component resides.
            Component[] componentsOnObject = component.gameObject.GetComponents<Component>();

            // Iterate through each component on the GameObject.
            foreach (Component comp in componentsOnObject)
            {
                // Fetch all the RequireComponent attributes associated with the component.
                object[] requireAttributes = comp.GetType().GetCustomAttributes(typeof(RequireComponent), true);

                // Iterate through each RequireComponent attribute to check if our target component type is listed.
                foreach (RequireComponent attribute in requireAttributes)
                {
                    // Check if any of the required types match the type of our target component.
                    if (attribute.m_Type0 == type ||
                        attribute.m_Type1 == type ||
                        attribute.m_Type2 == type)
                    {
                        // Remove the component that depends on the target component.
                        UnityEngine.Object.Destroy(comp);
                        removedAny = true; // Set the flag since we've removed a component.
                        break; // Exit the inner loop since we've made a modification.
                    }
                }
            }

            return removedAny; // Return the flag indicating if any components were removed.
        }

        /// <summary>
        /// Checks if the given component is required by any other components on the same GameObject via the RequireComponent attribute.
        /// </summary>
        /// <param name="component">The component to check for.</param>
        /// <returns>True if the component is required by another component on the same GameObject, otherwise false.</returns>
        public static bool IsRequiredByAnotherComponents(this UnityEngine.Component component)
        {
            Type type = component.GetType();
            // Get all components present on the GameObject where the target component resides.
            Component[] componentsOnObject = component.gameObject.GetComponents<Component>();

            // Iterate through each component on the GameObject.
            foreach (Component comp in componentsOnObject)
            {
                // Fetch all the RequireComponent attributes associated with the component.
                object[] requireAttributes = comp.GetType().GetCustomAttributes(typeof(RequireComponent), true);

                // Iterate through each RequireComponent attribute to check if our target component type is listed.
                foreach (RequireComponent attribute in requireAttributes)
                {
                    // Check if any of the required types match the type of our target component.
                    if (attribute.m_Type0 == type ||
                        attribute.m_Type1 == type ||
                        attribute.m_Type2 == type)
                    {
                        // If a match is found, return true indicating the component is required by another.
                        return true;
                    }
                }
            }

            // If no matches were found, return false.
            return false;
        }

        /// <summary>
        /// Saves the GameObject as a prefab asset at the specified local path and tries to get the component of type T from the prefab.
        /// </summary>
        /// <typeparam name="T">The type of the MonoBehaviour to retrieve from the prefab.</typeparam>
        /// <param name="go">The GameObject to be saved as a prefab.</param>
        /// <param name="localPath">The local path within the project where the prefab should be saved.</param>
        /// <param name="savedPrefab">The component of type T retrieved from the prefab, or null if the operation failed.</param>
        /// <param name="ex">Any exceptions that occurred during the process.</param>
        /// <returns>True if the prefab was saved and the component of type T was successfully retrieved, otherwise false.</returns>
        public static bool SaveAsPrefabAsset<T>(this GameObject go, string localPath, out T savedPrefab, out Exception ex) where T : MonoBehaviour
        {
            // Check for null GameObject
            if (go == null)
            {
                ex = new NullReferenceException("GameObject is null.");
                savedPrefab = null;
                return false;
            }

#if UNITY_EDITOR
            // Save the GameObject as a prefab asset in Editor mode.
            GameObject prefabGo = UnityEditor.PrefabUtility.SaveAsPrefabAsset(go, localPath, out bool success);

            // Attempt to get the component of type T from the saved prefab.
            if (prefabGo.TryGetComponent<T>(out T prefabComponent))
            {
                ex = null;
                savedPrefab = prefabComponent;
                return true;
            }
            else
            {
                // Handle the case where the component of type T can't be found on the prefab.
                ex = new Exception($"Can't get Type '{typeof(T).Name}' from GameObject '{prefabGo.name}'.");
                savedPrefab = null;
                return false;
            }
#endif

            // Handle cases outside of Editor mode.
            ex = new Exception("Unsupported in not-Editor mode.");
            savedPrefab = null;
            return false;
        }

        /// <summary>
        /// Checks if the provided UnityEngine.Object is part of any prefab.
        /// </summary>
        /// <param name="go">The UnityEngine.Object to check.</param>
        /// <returns>True if the object is part of a prefab, otherwise false.</returns>
        public static bool IsPartOfAnyPrefab(this UnityEngine.Object go)
        {
            if (go == null)
                return false;
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.IsPartOfAnyPrefab(go);
#endif
            return false;
        }

        /// <summary>
        /// Checks if any instance of the provided MonoBehaviour type exists on the scene.
        /// </summary>
        /// <typeparam name="T">Type of MonoBehaviour to check for.</typeparam>
        /// <returns>True if at least one instance of T exists on the scene, otherwise false.</returns>
        public static bool IsExistsOnScene<T>() where T : MonoBehaviour
        {
            int count = MonoBehaviour.FindObjectsOfType<T>().Length;
            return count != 0;
        }

        /// <summary>
        /// Waits for a specified number of frames.
        /// </summary>
        /// <param name="object">@object: The calling UnityEngine.Object.</param>
        /// <param name="count">Number of frames to wait.</param>
        /// <returns>An IEnumerator suitable for use in a coroutine.</returns>
        public static IEnumerator WaitForFrames(this UnityEngine.Object @object, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForEndOfFrame();
            }
        }
        /// <summary>
        /// Removes all childs from Transform.
        /// <para><see href="https://www.noveltech.dev/unity-delete-children/"/></para>
        /// </summary>
        public static int ClearChilds(this Transform transform)
        {
            int childCount = transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).gameObject.Destroy();
            }

            return childCount;
        }

        /// <summary>
        /// Destroying Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        /// <summary>
        public static bool Destroy(this UnityEngine.Object unityObject)
        {
            try
            {
#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(unityObject);
#else
                UnityEngine.Object.Destroy(unityObject);
#endif
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Destroying script of Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        public static bool Destroy(this UnityEngine.Component unityComponent)
        {
            try
            {
                if (unityComponent.IsRequiredByAnotherComponents())
                    return false;

#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(unityComponent);
#else
                UnityEngine.Object.Destroy(unityComponent);
#endif
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="component"></param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddComponent<T>(this GameObject gameObject, out T component) where T : UnityEngine.Component
        {
            if (gameObject.TryGetComponent(out component))
            {
                return true;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return false;
            }
        }

        public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : UnityEngine.Component
        {
            try
            {
                component = gameObject.GetComponent<T>();
                string _ = component.name;
                return true;
            }
            catch
            {
                component = default;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="graphic">Found or added graphic component.</param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddGraphic<T>(this GameObject gameObject, out T graphic) where T : Graphic
        {
            if (gameObject.TryGetComponent(out graphic))
            {
                return false;
            }
            else if (gameObject.TryGetComponent(out Graphic _graphic))
            {
                return false;
            }
            else
            {
                graphic = gameObject.AddComponent<T>();
                return true;
            }
        }
        public static bool TryDestroyComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            if (gameObject.TryGetComponent(out T component))
            {
                component.Destroy();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Marks target object as dirty, but as an extension.
        /// </summary>
        /// <param name="target">The object to mark as dirty.</param>
        public static void SetDirty(this UnityEngine.Object target)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(target);
#endif
        }

        public static GameObject CreateEmptyGameObject(Transform parent = null)
        {
            GameObject tempGO = new GameObject();
            GameObject emptyGO;

            if (parent == null)
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO);
            }
            else
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO, parent);
            }

            tempGO.Destroy();
            return emptyGO;
        }
    }
}