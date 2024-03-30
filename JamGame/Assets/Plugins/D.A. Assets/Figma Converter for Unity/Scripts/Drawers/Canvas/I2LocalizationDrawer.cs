using DA_Assets.FCU.Model;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;
using System.Collections;
using System;
using DA_Assets.Shared.Extensions;
using DA_Assets.Shared;
using DA_Assets.FCU.Extensions;

#if I2LOC_EXISTS
using I2.Loc;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class I2LocalizationDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void AddLanguageSource()
        {
#if I2LOC_EXISTS && UNITY_EDITOR
            if (monoBeh.Settings.ComponentSettings.UseI2Localization == false)
                return;

            if (languageSource == null)
            {
                languageSource = MonoBehaviour.FindObjectOfType<LanguageSource>();

                if (languageSource == null)
                {
                    GameObject _gameObject = MonoBehExtensions.CreateEmptyGameObject();
                    _gameObject.name = FcuConfig.Instance.I2LocGameObjectName;
                    languageSource = _gameObject.AddComponent<LanguageSource>();
                }
            }

            ImportCSV(GetLocFilePath(), eSpreadsheetUpdateMode.Merge);
#endif
        }

        public void AddI2Localize(FObject fobject)
        {
#if I2LOC_EXISTS && UNITY_EDITOR
            fobject.Data.GameObject.TryAddComponent(out I2.Loc.Localize i2l);

            i2l.Source = languageSource;

            string subStr = fobject.Characters.SubstringSafe(24);
            string newKey = subStr.ReplaceInvalidFileNameChars().ToLower();
            string lfp = GetLocFilePath();

            if (TextExistsInFile(lfp, newKey) == false)
            {
                string text = EscapeQuotesForExel(fobject.GetText());

                string newLine = $"{newKey};;;\"{text}\"\n";

                using (StreamWriter writer = new StreamWriter(lfp, true, Encoding.UTF8))
                {
                    writer.WriteLine(newLine);
                }
            }

            i2l.Term = newKey;
#endif
        }

#if I2LOC_EXISTS && UNITY_EDITOR

        [SerializeField] LanguageSource languageSource;

        private string GetLocFilePath()
        {
            string path = $"{Application.dataPath}/Resources/{FcuConfig.Instance.LocalizationFileName}";
            $"{Application.dataPath}/Resources/".CreateFolderIfNotExists();
            CreateLocFile(path);
            return path;
        }
        private string EscapeQuotesForExel(string text)
        {
            return text.Replace("\"", "\"\"");
        }
        private void ImportCSV(string FileName, eSpreadsheetUpdateMode updateMode)
        {
            languageSource.mSource.Import_CSV(
                "",
                LocalizationReader.ReadCSVfile(FileName, Encoding.UTF8),
                updateMode,
                ';');

            languageSource.mSource.Awake();
        }
        private void CreateLocFile(string path)
        {
            if (File.Exists(path) == false)
            {
                FileStream oFileStream = new FileStream(path, FileMode.Create);
                oFileStream.Close();

                CheckLocHeader(path);
            }
            else
            {
                CheckLocHeader(path);
            }
        }

        private void CheckLocHeader(string path)
        {
            string fileHeader = "Key;Type;Desc;English";

            if (TextExistsInFile(path, fileHeader) == false)
            {
                string currentContent = File.ReadAllText(path);
                File.WriteAllText(path, $"{fileHeader}\n{currentContent}");
            }
        }
        private bool TextExistsInFile(string filePath, string text)
        {
            string[] lines = File.ReadAllLines(filePath);
            bool contains = false;
            Parallel.ForEach(lines, (line) =>
            {
                if (line.Contains(text))
                {
                    contains = true;
                }
            });

            return contains;
        }
#endif
    }
}
