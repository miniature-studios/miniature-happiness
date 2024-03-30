using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class DependenciesTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_asset_dependencies.Localize());
            gui.Space15();

            if (FcuConfig.Instance.Dependencies.IsEmpty())
                return;

            for (int i = 0; i < FcuConfig.Instance.Dependencies.Count; i++)
            {
                DependencyItem ac = FcuConfig.Instance.Dependencies[i];
                ac.Enabled = gui.Toggle(new GUIContent(ac.Name), ac.Enabled);
                FcuConfig.Instance.Dependencies[i] = ac;
            }

            gui.Space15();

            if (gui.OutlineButton("Apply"))
            {
                DefineModifier.Apply();
            }
        }
    }

    internal class DefineModifier
    {
        [DidReloadScripts]
        private static void OnScriptsReload()
        {
            SearchAssets().StartDARoutine(null);
        }

        internal static void Apply()
        {
            List<string> enabled = new List<string>();
            List<string> disabled = new List<string>();

            foreach (DependencyItem assemblyConfig in FcuConfig.Instance.Dependencies)
            {
                if (assemblyConfig.Enabled)
                {
                    enabled.Add(assemblyConfig.ScriptingDefineName);
                }
                else
                {
                    disabled.Add(assemblyConfig.ScriptingDefineName);
                }
            }

            enabled.Add("FCU_EXISTS");

            Modify(enabled.ToArray(), disabled.ToArray());
        }

        internal static IEnumerator SearchAssets()
        {
            List<DependencyItem> l = FcuConfig.Instance.Dependencies;

            for (int i = 0; i < l.Count; i++)
            {
                Type t = Type.GetType(l[i].Type);

                if (t == null)
                    continue;

                DependencyItem ac = l[i];

                //Ignoring Unity.Plastic.Newtonsoft.Json.dll
                if (t.Assembly.Location.EndsWith("Editor\\Data\\Managed\\Newtonsoft.Json.dll"))
                    continue;

                ac.Enabled = true;
                l[i] = ac;

                yield return null;
            }

            Apply();
        }

        internal static void Modify(string[] addDefines, string[] removeDefines)
        {
            List<string> finalDefines = GetDefines();

            finalDefines.AddRange(addDefines);
            finalDefines.RemoveAll(x => removeDefines.Contains(x));
            finalDefines = finalDefines.Distinct().ToList();

            SetDefines(finalDefines);
        }

        internal static List<string> GetDefines()
        {
            string rawDefs;

#if UNITY_2022_3_OR_NEWER
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            UnityEditor.Build.NamedBuildTarget namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
            rawDefs = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
#else
            rawDefs = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif

            if (string.IsNullOrWhiteSpace(rawDefs) == false)
            {
                return rawDefs.Split(';').ToList();
            }

            return new List<string>();
        }

        internal static void SetDefines(List<string> defines)
        {
            string joinedDefs = string.Join(";", defines);

#if UNITY_2022_3_OR_NEWER
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
            UnityEditor.Build.NamedBuildTarget namedBuildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, joinedDefs);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, joinedDefs);
#endif
        }
    }
}
