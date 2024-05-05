using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace DA_Assets.FCU
{
    [Serializable]
    public class PrefabCreator : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] string prefabsPath = "Assets/Prefabs";
        public string PrefabsPath { get => prefabsPath; set => SetValue(ref prefabsPath, value); }

        private List<GameObject> toDestroy;
        [SerializeField] List<PrefabStruct> pstructs;

        public IEnumerator CreatePrefabs()
        {
#if UNITY_EDITOR
            DALogger.Log(FcuLocKey.log_start_creating_prefabs.Localize());
            yield return WaitFor.Delay01();

            SceneBackuper.BackupActiveScene();
            AssetTools.MakeActiveSceneDirty();

            pstructs = new List<PrefabStruct>();
            toDestroy = new List<GameObject>();
            int prefabCount = 0;

            SyncHelper[] syncHelpers = monoBeh.SyncHelpers.GetAllSyncHelpers();

            foreach (SyncHelper syncObject in syncHelpers)
            {
                if (syncObject.IsPartOfAnyPrefab())
                {
                    continue;
                }

                PrefabStruct ps = CreatePrefabStruct(syncObject);
                pstructs.Add(ps);
            }

            int iout = 1;

            for (int i = 0; i < pstructs.Count(); i++)
            {
                for (int j = 0; j < pstructs.Count(); j++)
                {
                    if (i == j)
                        continue;

                    if (pstructs[i].Hash != pstructs[j].Hash)
                        continue;

                    if (pstructs[j].PrefabNumber == 0)
                    {
                        PrefabStruct ps = pstructs[i];
                        ps.PrefabNumber = iout;
                        pstructs[i] = ps;
                        iout++;
                        break;
                    }
                    else
                    {
                        PrefabStruct ps = pstructs[i];
                        ps.PrefabNumber = pstructs[j].PrefabNumber;
                        pstructs[i] = ps;
                        break;
                    }
                }
            }

            for (int i = 0; i < pstructs.Count(); i++)
            {
                PrefabStruct psi = pstructs[i];

                for (int j = 0; j < pstructs.Count(); j++)
                {
                    PrefabStruct psj = pstructs[j];

                    if (psi.Hash != psj.Hash)
                        continue;

                    if (psi.PrefabNumber != psj.PrefabNumber)
                        continue;

                    if (psi.Current.Data.CanBePrefab() == false)
                        continue;

                    if (psj.Prefab == null)
                    {
                        CreatePrefab(ref pstructs, i);
                        prefabCount++;
                    }
                    else
                    {
                        InstantiatePrefab(ref psi, i, psj.Prefab);
                    }

                    break;
                }

            }

            foreach (GameObject item in toDestroy)
            {
                try
                {
                    item.Destroy();
                }
                catch (Exception ex)
                {
                    DALogger.LogException(ex);
                }
            }

            foreach (var ps in pstructs)
            {
                if (ps.PrefabPath == null)
                    continue;

                if (ps.InstantiatedPrefab == null)
                    continue;

                if (UnityEditor.PrefabUtility.GetPrefabAssetType(ps.InstantiatedPrefab.gameObject) == UnityEditor.PrefabAssetType.NotAPrefab)
                    continue;

                ps.InstantiatedPrefab.gameObject.SaveAsPrefabAsset(ps.PrefabPath, out SyncHelper savedPrefab, out Exception ex);
            }

            foreach (var ps in pstructs)
            {
                if (ps.Current.Data.CanBePrefab() == false)
                {
                    try
                    {
                        ps.Current.gameObject.Destroy();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }

                }
            }

            yield return monoBeh.SyncHelpers.SetFcuToAllSyncHelpers();

            syncHelpers = monoBeh.SyncHelpers.GetAllSyncHelpers();
            monoBeh.SyncHelpers.RestoreRootFrames(syncHelpers);

            DALogger.Log(FcuLocKey.log_prefabs_created.Localize(prefabCount));
#endif
            yield return null;
        }

        private PrefabStruct CreatePrefabStruct(SyncHelper syncObject)
        {
            PrefabStruct ps = new PrefabStruct();

            ps.Current = syncObject;

            try
            {
                ps.Parent = syncObject.transform.parent.GetComponent<SyncHelper>();
            }
            catch
            {

            }

            List<SyncHelper> childs = syncObject.GetComponentsInChildren<SyncHelper>(true).ToList();
            childs.RemoveAt(0);

            ps.Hash = syncObject.Data.Hash;
            ps.Id = syncObject.Data.Id;
            ps.Childs = childs.ToArray();

            ps.Current.Data.TransformData = new TransformData(syncObject.GetComponent<RectTransform>());
            ps.SiblingIndex = syncObject.transform.GetSiblingIndex();
            ps.PrefabNumber = 0;

            return ps;
        }

        private void CreatePrefab(ref List<PrefabStruct> pstructs, int i)
        {
            try
            {
                PrefabStruct ps = pstructs[i];

                RemoveParent(ref ps);

                ps.PrefabPath = GetPrefabPath(ps);

                if (ps.Current.gameObject.SaveAsPrefabAsset(ps.PrefabPath, out SyncHelper savedPrefab, out Exception ex))
                {
                    ps.Prefab = savedPrefab;
                    InstantiatePrefab(ref ps, i, ps.Prefab);
                }
                else
                {
                    DALogger.LogException(ex);
                }

                RestoreParent(ref ps);
                pstructs[i] = ps;
            }
            catch (Exception ex)
            {
                DALogger.LogException(ex);
            }
        }

        private void RestoreParent(ref PrefabStruct ps)
        {
            if (ps.Current.Data.Tags.Contains(FcuTag.Frame))
            {
                ps.Current.transform.SetParent(monoBeh.transform);
            }
            else
            {
                ps.Current.transform.SetParent(ps.Parent.transform);
            }

            foreach (SyncHelper childTransform in ps.Childs)
            {
                if (childTransform == null)
                    continue;

                childTransform.transform.SetParent(ps.InstantiatedPrefab.transform);
            }
        }

        private void RemoveParent(ref PrefabStruct ps)
        {
            ps.Current.transform.SetParent(null);

            foreach (SyncHelper childTransform in ps.Childs)
            {
                if (childTransform == null)
                    continue;

                childTransform.transform.SetParent(null);
            }
        }

        private void InstantiatePrefab(ref PrefabStruct ps, int i, UnityEngine.Object pref)
        {
#if UNITY_EDITOR
            try
            {
                if (ps.Current.Data.CanBePrefab() == false)
                {
                    ps.InstantiatedPrefab = ps.Current;
                }
                else
                {

                    ps.InstantiatedPrefab = (SyncHelper)UnityEditor.PrefabUtility.InstantiatePrefab(pref);

                    if (ps.Current.Data.Tags.Contains(FcuTag.Frame))
                    {
                        ps.InstantiatedPrefab.transform.SetParent(monoBeh.transform);
                    }
                    else
                    {
                        ps.InstantiatedPrefab.transform.SetParent(ps.Parent.transform);
                    }

                    ps.InstantiatedPrefab.name = ps.Current.name;
                    ps.InstantiatedPrefab.transform.SetSiblingIndex(pstructs[i].SiblingIndex);
                    ps.Current.Data.TransformData.ApplyTo(ps.InstantiatedPrefab.GetComponent<RectTransform>());
                    ps.InstantiatedPrefab.Data.Id = ps.Id;

                    if (ps.Current.Data.Tags.Contains(FcuTag.Text))
                    {
                        ps.InstantiatedPrefab.gameObject.SetText(ps.Current.gameObject.GetText());
                    }

                    toDestroy.Add(ps.Current.gameObject);
                }

                pstructs[i] = ps;
            }
            catch (Exception ex)
            {
                DALogger.LogException(ex);
            }
#endif
        }
        string GetPrefabName(SyncHelper sh)
        {
            string prefabName;

            if (monoBeh.Settings.PrefabSettings.TextPrefabNameType != TextPrefabNameType.Figma &&
                sh.Data.Tags.Contains(FcuTag.Text))
            {
                prefabName = $"{sh.Data.HumanizedTextPrefabName.Trim()} {sh.Data.Hash}";
            }
            else
            {
                prefabName = $"{sh.gameObject.name.Trim()} {sh.Data.Hash}";
            }

            return prefabName;
        }
        private string GetPrefabPath(PrefabStruct ps)
        {


            string GetSubDir(SyncData sd)
            {
                try
                {
                    if (sd.Tags.Contains(FcuTag.Text))
                        return "Texts";
                    else if (sd.Tags.Contains(FcuTag.Image))
                        return "Images";
                    else if (sd.Tags.Contains(FcuTag.Button))
                        return "Buttons";
                    else if (sd.Tags.Contains(FcuTag.InputField))
                        return "InputFields";
                    else
                        return "Other";
                }
                catch
                {
                    return "Other";
                }
            }

            if (ps.Current.Data.RootFrame == null)
            {
                SyncData myRootFrame = monoBeh.SyncHelpers.GetRootFrame(ps.Current.Data);
                ps.Current.Data.RootFrame = myRootFrame;
            }

            string frameDir = Path.Combine(prefabsPath, ps.Current.Data.RootFrame.NewName);
            string subDir = GetSubDir(ps.Current.Data);
            string fullDir = Path.Combine(frameDir, subDir);
            fullDir.CreateFolderIfNotExists();

            string prefabName = GetPrefabName(ps.Current);
            string prefabPath = Path.Combine(frameDir, subDir, $"{prefabName}.prefab");

            string result = prefabPath.GetPathRelativeToProjectDirectory();
            return result;
        }

    }

    [Serializable]
    public struct PrefabStruct
    {
        [SerializeField] SyncHelper current;
        public SyncHelper Current { get => current; set => current = value; }

        [SerializeField] SyncHelper parent;
        public SyncHelper Parent { get => parent; set => parent = value; }

        [Space]

        [SerializeField] SyncHelper prefab;
        public SyncHelper Prefab { get => prefab; set => prefab = value; }
        [SerializeField] SyncHelper instantiatedPrefab;
        public SyncHelper InstantiatedPrefab { get => instantiatedPrefab; set => instantiatedPrefab = value; }

        [Space]

        [SerializeField] SyncHelper[] childs;
        public SyncHelper[] Childs { get => childs; set => childs = value; }

        [Space]

        [SerializeField] int siblingIndex;
        public int SiblingIndex { get => siblingIndex; set => siblingIndex = value; }

        [SerializeField] int hash;
        public int Hash { get => hash; set => hash = value; }

        [SerializeField] string id;
        public string Id { get => id; set => id = value; }

        [SerializeField] int prefabNumber;
        public int PrefabNumber { get => prefabNumber; set => prefabNumber = value; }

        [SerializeField] string prefabPath;
        public string PrefabPath { get => prefabPath; set => prefabPath = value; }
    }

    [Serializable]
    public struct TransformData
    {
        [SerializeField] Vector2 anchorMin;
        [SerializeField] Vector2 anchorMax;
        [SerializeField] Vector2 anchoredPosition;
        [SerializeField] Vector3 sizeDelta;
        [SerializeField] Quaternion localRotation;
        [SerializeField] Vector3 localScale;

        public TransformData(RectTransform source)
        {
            anchorMin = source.anchorMin;
            anchorMax = source.anchorMax;
            anchoredPosition = source.anchoredPosition;
            sizeDelta = source.sizeDelta;
            localRotation = source.localRotation;
            localScale = source.localScale;
        }

        public void ApplyTo(RectTransform target)
        {
            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
            target.anchoredPosition = anchoredPosition;
            target.sizeDelta = sizeDelta;
            target.localRotation = localRotation;
            target.localScale = localScale;
        }
    }

    public static class PrefabCreatorExtensions
    {
        public static bool CanBePrefab(this SyncData sd)
        {
            bool onlyCont = sd.Tags.Contains(FcuTag.Container) && sd.Tags.Count == 1;
            return !onlyCont;
        }
    }
}
