using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DA_Assets.FCU
{
    public class SceneBackuper
    {
        public static List<string> GetSceneBackupPath()
        {
            List<string> sceneBackupPath = Application.dataPath.Split(char.Parse("/")).ToList();
            sceneBackupPath[sceneBackupPath.Count - 1] = "Library";
            sceneBackupPath.Add("Backup");
            sceneBackupPath.Add("Scene");
            return sceneBackupPath;
        }

        public static string GetLibraryPath(List<string> sceneBackupPath)
        {
            string _libraryPath = string.Join("/", sceneBackupPath);
            _libraryPath.CreateFolderIfNotExists();
            return _libraryPath;
        }

        public static void BackupActiveScene()
        {
            try
            {
                List<string> sceneBackupPath = GetSceneBackupPath();
                string _libraryPath = GetLibraryPath(sceneBackupPath);

                Scene activeScene = SceneManager.GetActiveScene();

                string newName = $"{DateTime.Now.ToString(FcuConfig.Instance.DateTimeFormat2)}_{activeScene.name}.unity";
                sceneBackupPath.Add(newName);

                string finalPath = string.Join("/", sceneBackupPath);

                File.Copy(activeScene.path, finalPath);

                DALogger.Log(FcuLocKey.log_scene_backup_created.Localize(finalPath));
            }
            catch (Exception ex)
            {
                DALogger.LogError(FcuLocKey.log_scene_backup_creation_error.Localize(ex));
            }
        }
    }
}