using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class DebugToolsTab : ScriptableObjectBinder<FcuSettingsWindow, FigmaConverterUnity>
    {

        private Editor fcuConfigEditor;

        public override void Init()
        {
            fcuConfigEditor = Editor.CreateEditor(FcuConfig.Instance);
        }

        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_debug_tools.Localize());
            gui.Space15();

            monoBeh.Settings.DebugSettings.DebugMode = gui.Toggle(
                new GUIContent(FcuLocKey.label_debug_mode.Localize(), FcuLocKey.tooltip_debug_mode.Localize()),
                monoBeh.Settings.DebugSettings.DebugMode);

            if (monoBeh.Settings.DebugSettings.DebugMode)
            {
                monoBeh.Settings.DebugSettings.LogDefault = gui.Toggle(new GUIContent(FcuLocKey.label_log_default.Localize()),
                    monoBeh.Settings.DebugSettings.LogDefault);

                monoBeh.Settings.DebugSettings.LogSetTag = gui.Toggle(new GUIContent(FcuLocKey.label_log_set_tag.Localize()),
                    monoBeh.Settings.DebugSettings.LogSetTag);

                monoBeh.Settings.DebugSettings.LogIsDownloadable = gui.Toggle(new GUIContent(FcuLocKey.label_log_downloadable.Localize()),
                    monoBeh.Settings.DebugSettings.LogIsDownloadable);

                monoBeh.Settings.DebugSettings.LogTransform = gui.Toggle(new GUIContent(FcuLocKey.label_log_transform.Localize()),
                    monoBeh.Settings.DebugSettings.LogTransform);

                monoBeh.Settings.DebugSettings.LogGameObjectDrawer = gui.Toggle(new GUIContent(FcuLocKey.label_log_go_drawer.Localize()),
                    monoBeh.Settings.DebugSettings.LogGameObjectDrawer);
            }

            gui.Space15();

            if (gui.OutlineButton("Open logs folder"))
            {
                FcuConfig.LogPath.OpenFolderInOS();
            }

            gui.Space15();

            if (gui.OutlineButton("Open cache folder"))
            {
                FcuConfig.CachePath.OpenFolderInOS();
            }

            gui.Space15();

            if (gui.OutlineButton("Open backup folder"))
            {
                List<string> sceneBackupPath = SceneBackuper.GetSceneBackupPath();
                string _libraryPath = SceneBackuper.GetLibraryPath(sceneBackupPath);
                _libraryPath = _libraryPath.Replace('/', '\\');

                _libraryPath.OpenFolderInOS();
            }

            gui.Space15();

            if (gui.OutlineButton("Test Button"))
            {
                var shs =  new List<SyncHelper>().ToArray();

             /*   foreach (var item in syncHelpers)
                {
                    Debug.Log(item.name);
                }*/
            }

            if (monoBeh.Settings.DebugSettings.DebugMode)
            {
                gui.Space30();

                fcuConfigEditor.OnInspectorGUI();

                gui.Space30();
                scriptableObject.Inspector.DrawBaseOnInspectorGUI();
            }
        }
    }
}