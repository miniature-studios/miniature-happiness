using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class DALocalizator<T> : SingletoneScriptableObject<T> where T : ScriptableObject
    {
        [SerializeField] TextAsset locFile;
        private List<LocItem> localizations;

        internal List<LocItem> ConvertFileToLocItems(string csvText)
        {
            List<LocItem> locItems = new List<LocItem>();
            using (var reader = new StringReader(csvText))
            {
                string line;
                LocItem currentLocItem = null;
                StringBuilder currentText = null;

                while ((line = reader.ReadLine()) != null)
                {
                    if (currentLocItem == null)
                    {
                        string[] columns = line.Split(';');
                        if (columns.Length >= 2)
                        {
                            currentLocItem = new LocItem
                            {
                                key = columns[0],
                                en = columns[1]
                            };

                            if (currentLocItem.en.StartsWith("\"") && !currentLocItem.en.EndsWith("\""))
                            {
                                currentText = new StringBuilder();
                                currentText.AppendLine(currentLocItem.en.TrimStart('\"'));
                            }
                            else
                            {
                                locItems.Add(currentLocItem);
                                currentLocItem = null;
                            }
                        }
                    }
                    else if (currentText != null)
                    {
                        if (line.EndsWith("\"") && !line.EndsWith(";\""))
                        {
                            currentText.AppendLine(line.TrimEnd('\"'));
                            currentLocItem.en = currentText.ToString();
                            locItems.Add(currentLocItem);
                            currentLocItem = null;
                            currentText = null;
                        }
                        else
                        {
                            currentText.AppendLine(line);
                        }
                    }
                }
            }

            return locItems;
        }

        public override void OnCreateInstance()
        {
            Init();
        }

        public string Localize(string key, Language lang, params object[] args)
        {
            foreach (LocItem item in localizations)
            {
                if (item.key == key)
                {
                    string txt = "";

                    switch (lang)
                    {
                        case Language.en:
                            txt = item.en;
                            break;
                    }

                    if (txt == "")
                    {
                        return key;
                    }

                    try
                    {
                        return string.Format(txt, args);
                    }
                    catch
                    {
                        return txt;
                    }
                }
            }

            return key;
        }

        internal void Init()
        {
            localizations = ConvertFileToLocItems(locFile.text);
        }
    }

    internal class LocItem
    {
        public string key { get; set; }
        public string en { get; set; }
    }
    public enum Language
    {
        en = 0,
    }
}
