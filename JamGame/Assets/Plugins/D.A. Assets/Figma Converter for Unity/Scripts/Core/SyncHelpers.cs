using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SyncHelpers : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator DestroySyncHelpers()
        {
            SyncHelper[] syncHelpers = GetAllSyncHelpers();

            for (int i = 0; i < syncHelpers.Length; i++)
            {
                syncHelpers[i].Destroy();
                yield return null;
            }

            DALogger.Log(FcuLocKey.log_current_canvas_metas_destroy.Localize(
                monoBeh.GetInstanceID(),
                syncHelpers.Length,
                nameof(SyncHelper)));
        }

        public SyncHelper[] GetAllSyncHelpers()
        {
            List<SyncHelper> childs = monoBeh.gameObject.GetComponentsInReverseOrder<SyncHelper>();
            return childs.ToArray();
        }

        public bool IsExistsOnCurrentCanvas(FObject fobject, out SyncHelper syncObject)
        {
            SyncHelper[] syncHelpers = GetAllSyncHelpers();

            foreach (SyncHelper sh in syncHelpers)
            {
                if (sh.Data.Id == fobject.Id)
                {
                    syncObject = sh;
                    return true;
                }
            }

            syncObject = null;
            return false;
        }

        /// <summary>
        /// Goes up the hierarchy until it finds RootFrame.
        /// </summary>
        public SyncData GetRootFrame(SyncData syncData)
        {
            GameObject currentGameObject = syncData.GameObject;

            while (currentGameObject != null)
            {
                SyncHelper syncHelper = currentGameObject.GetComponent<SyncHelper>();
                if (syncHelper != null && syncHelper.ContainsTag(FcuTag.Frame))
                {
                    return syncHelper.Data;
                }

                currentGameObject = currentGameObject.transform.parent?.gameObject;
            }

            return null;
        }

        /// <summary>
        /// Restore RootFrame in all SyncHelpers.
        /// </summary>
        public void RestoreRootFrames(SyncHelper[] syncHelpers)
        {
            List<SyncHelper> frames = new List<SyncHelper>();
            foreach (SyncHelper syncHelper in syncHelpers)
            {
                if (syncHelper.ContainsTag(FcuTag.Frame))
                {
                    frames.Add(syncHelper);
                }
            }

            foreach (SyncHelper frame in frames)
            {
                SetRootFrameToAllChilds(frame.gameObject, frame.Data);
            }
        }

        public void SetRootFrameToAllChilds(GameObject @object, SyncData rootFrame)
        {
            if (@object == null)
                return;

            foreach (Transform child in @object.transform)
            {
                if (child == null)
                    continue;

                if (child.TryGetComponent(out SyncHelper syncObject))
                {
                    syncObject.Data.RootFrame = rootFrame;
                }

                SetRootFrameToAllChilds(child.gameObject, rootFrame);
            }
        }

        /// <summary>
        /// For 3.0.0-3.1.3
        /// </summary>
        public IEnumerator OptimizeSyncHelpers()
        {
            int counter = 0;
            OptimizeAllChilds(monoBeh.gameObject);

            yield return WaitFor.Delay01();

            DALogger.Log(FcuLocKey.log_fcu_assigned.Localize(
                counter,
                nameof(FigmaConverterUnity),
                monoBeh.GetInstanceID()));

            void OptimizeAllChilds(GameObject @object)
            {
                if (@object == null)
                    return;

                foreach (Transform child in @object.transform)
                {
                    if (child == null)
                        continue;

                    if (child.TryGetComponent(out SyncHelper syncObject))
                    {
                        counter++;
                        syncObject.Data.HashData = null;
                        syncObject.Data.HashDataTree = null;
                    }

                    OptimizeAllChilds(child.gameObject);
                }
            }
        }

        public IEnumerator SetFcuToAllSyncHelpers()
        {
            int counter = 0;
            SetFcuToAllChilds(monoBeh.gameObject, ref counter);

            yield return WaitFor.Delay01();

            DALogger.Log(FcuLocKey.log_fcu_assigned.Localize(
                counter,
                nameof(FigmaConverterUnity),
                monoBeh.GetInstanceID()));
        }

        public void SetFcuToAllChilds(GameObject @object, ref int counter)
        {
            if (@object == null)
                return;

            foreach (Transform child in @object.transform)
            {
                if (child == null)
                    continue;

                if (child.TryGetComponent(out SyncHelper syncObject))
                {
                    counter++;
                    syncObject.Data.FigmaConverterUnity = monoBeh;
                }

                SetFcuToAllChilds(child.gameObject, ref counter);
            }
        }
    }
}