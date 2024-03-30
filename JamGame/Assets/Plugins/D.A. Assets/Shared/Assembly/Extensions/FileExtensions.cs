using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DA_Assets.Shared.Extensions
{
    public static class FileExtensions
    {
        public static void OpenFolderInOS(this string path)
        {
            try
            {
                string folderViewerName = null;

#if UNITY_EDITOR_WIN
                folderViewerName = "explorer.exe";
#elif UNITY_EDITOR_OSX
                folderViewerName = "open";
#elif UNITY_EDITOR_LINUX
                folderViewerName = "xdg-open";
#endif

                if (folderViewerName == null)
                {
                    throw new NullReferenceException();
                }

                System.Diagnostics.Process.Start(folderViewerName, path);
            }
            catch
            {
                DALogger.LogError($"This feature is not supported on this OS.");
            }
        }

        public static string TrimTrailingSlashes(this string path)
        {
            return path.TrimEnd('/', '\\');
        }

        public static string ToUnitySeparators(this string path)
        {
            return path.Replace('\\', '/');
        }

        public static string GetPathRelativeToProjectDirectory(this string path)
        {
            string formated = path.ToUnitySeparators().TrimTrailingSlashes();

            string result = string.Join("/", formated.Split(new char[1] { '/' })
                .SkipWhile((string s) => s != "assets" && s != "Assets")
                .ToArray());

            return result;
        }

        public static string ReplaceSeparatorChars(this string value, string newChar = "")
        {
            string _value = value;

            string[] chars = new string[]
            {
                "_", "-" , " ", "+", "&"
            };

            foreach (string @char in chars)
            {
                _value = _value.Replace(@char, newChar);
            }

            return _value;
        }
        /// <summary>
        /// Replaces text in a file.
        /// <para><see href="https://stackoverflow.com/a/58377834/8265642"/></para>
        /// </summary>
        /// <param name="filePath">Path of the text file.</param>
        /// <param name="searchText">Text to search for.</param>
        /// <param name="replaceText">Text to replace the search text.</param>
        static public void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }
        public static void CreateFolderIfNotExists(this string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Checks for the presence of folders in the input path, and if it's are not there, creates it.
        /// </summary>
        /// <param name="pathFolders">Full path to the destination folder, including the 'Assets' folder in begin, and destination in end.</param>
        public static string CreateAllPathFolders(params string[] path)
        {
#if UNITY_EDITOR
            for (int i = 0; i < path.Count(); i++)
            {
                if (i == 0)
                {
                    continue;
                }

                IEnumerable<string> parentFolders = path[i].GetBetweenElement(path);
                string parentPath = Path.Combine(parentFolders.ToArray());
                string newFolderPath = Path.Combine(parentPath, path[i]);

                if (UnityEditor.AssetDatabase.IsValidFolder(newFolderPath) == false)
                {
                    UnityEditor.AssetDatabase.CreateFolder(parentPath, path[i]);
                }
            }
#endif
            return Path.Combine(path);
        }
        public static bool TryReadAllText(this string path, out string text)
        {
            try
            {
                text = File.ReadAllText(path);
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }
        public static string GetFullAssetPath(this string assetPath)
        {
            string p2 = assetPath.Substring("Assets/".Length);
            string fullPath = Path.Combine(Application.dataPath, p2);
            return fullPath;
        }
        public static bool IsPathInsideAssetsPath(this string path)
        {
            if (path.IndexOf(Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// http://answers.unity.com/answers/787336/view.html
        /// </summary>
        public static string ToRelativePath(this string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            return absolutePath;
        }
        private static char[] invalidFileNameChars = new char[] { '“', '”', '"', '^', '<', '>', ';', '|', '/', ',', '\\', ':', '=', '?', '\"', '*', '\'', '’', '#' };
        public static string GetInvalidFileNameChars(this string filename)
        {
            List<char> invalidChars = new List<char>();

            foreach (char c in filename)
            {
                if (invalidFileNameChars.Contains(c))
                {
                    invalidChars.Add(c);
                }
            }

            string result = "";

            if (invalidChars.Count() > 0)
            {
                result = string.Join(" ", invalidChars);
            }

            return result;
        }

        //TODO: ?
        public static string ReplaceInvalidFileNameChars(this string fileName, char newChar = '-')
        {
            string newName = "";

            try
            {
                for (int i = 0; i < fileName.Length; i++)
                {
                    if (invalidFileNameChars.Contains(fileName[i]))
                    {
                        newName += newChar;
                    }
                    else
                    {
                        newName += fileName[i];
                    }
                }

                newName = newName.Replace("(", "[").Replace(")", "]");
                newName = Regex.Replace(newName, @"\t|\n|\r", newChar.ToString());

                if (!char.IsLetter(newName[0]))
                {
                    newName = '_' + newName.Remove(0, 1);
                }

                return newName.TrimEnd();
            }
            catch
            {
                return "";
            }
        }
    }
}