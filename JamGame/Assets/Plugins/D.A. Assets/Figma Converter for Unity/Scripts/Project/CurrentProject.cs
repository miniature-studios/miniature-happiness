using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class CurrentProject : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public string GetInstanceKey(string id)
        {
            foreach (var item in figmaProject.Components)
            {
                if (item.Key == id)
                {
                    return item.Value.Key;
                }
            }

            return null;
        }

        public bool TryGetById(string id, out FObject fobject)
        {
            foreach (var item in this.CurrentPage)
            {
                if (item.Id == id)
                {
                    fobject = item;
                    return true;
                }
            }

            fobject = default;
            return false;
        }

        public bool TryGetByIndex(int index, out FObject fobject)
        {
            if (index < 0)
            {
                fobject = default;
                return false;
            }

            fobject = this.CurrentPage[index];
            return true;
        }

        public bool TryGetParent(FObject fobject, out FObject parent)
        {
            if (fobject.Data.ParentIndex < 0)
            {
                parent = default;
                return false;
            }

            parent = this.CurrentPage[fobject.Data.ParentIndex];
            return true;
        }

        public FObject GetRootFrame(FObject fobject)
        {
        
            if (fobject.ContainsTag(FcuTag.Frame))
            {
                return fobject;
            }

            TryGetParent(fobject, out FObject parent);

            return GetRootFrame(parent);
        }

        public FObject GetNotDuplicate(FObject fobject)
        {
            if (fobject.Data.Parent.Equals(default(FObject)))
                return default;

            if (fobject.Data.Parent.Data.IsDuplicate == false)
                return fobject;

            TryGetParent(fobject, out FObject parent);
            return GetRootFrame(parent);
        }

        public bool HasLocalPrefab(SyncData fobject, out SyncHelper localPrefab)
        {
            foreach (SyncHelper lp in localPrefabs)
            {
                if (lp.Data.Hash == fobject.Hash)
                {
                    localPrefab = lp;
                    return true;
                }
            }
            localPrefab = default;
            return false;
        }

        public IEnumerator LoadLocalPrefabs()
        {
            DALogger.Log(FcuLocKey.log_search_local_prefabs.Localize());
            yield return monoBeh.PrefabCreator.PrefabsPath.LoadAssetFromFolder<SyncHelper>(@return => localPrefabs = @return, "t:Prefab");
            DALogger.Log(FcuLocKey.log_local_prefabs_found.Localize(localPrefabs.Count));
        }

        public void SetRootFrames(List<FObject> fobjects)
        {
            for (int i = 0; i < fobjects.Count; i++)
            {
                if (fobjects[i].ContainsTag(FcuTag.Page))
                    continue;

                FObject rootFrameFObject = monoBeh.CurrentProject.GetRootFrame(fobjects[i]);
                fobjects[i].Data.RootFrame = rootFrameFObject.Data;
            }
        }

        [SerializeField] List<SyncData> lastImportedFrames = new List<SyncData>();
        public List<SyncData> LastImportedFrames { get => lastImportedFrames; set => SetValue(ref lastImportedFrames, value); }

        [SerializeField] List<SyncHelper> localPrefabs = new List<SyncHelper>();
        public List<SyncHelper> LocalPrefabs { get => localPrefabs; set => SetValue(ref localPrefabs, value); }

        private FigmaProject figmaProject;
        public FigmaProject FigmaProject { get => figmaProject; set => figmaProject = value; }

        private List<FObject> currentPage = new List<FObject>();
        public List<FObject> CurrentPage { get => currentPage; set => currentPage = value; }
    }
}