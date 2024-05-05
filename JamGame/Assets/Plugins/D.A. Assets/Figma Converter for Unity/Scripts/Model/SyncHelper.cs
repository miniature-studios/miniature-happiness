using DA_Assets.FCU.Extensions;
using UnityEngine;
using System;

namespace DA_Assets.FCU.Model
{
    //TODO: set current GameObject to SyncHelper
    [Serializable]
    public class SyncHelper : MonoBehaviour
    {
        [SerializeField] SyncData data;
        public SyncData Data { get => data; set => data = value; }

    }

    public static class TempExtensions
    {

        public static void SetData(this FObject fobject, SyncHelper syncHelper, FigmaConverterUnity fcu)
        {
            fobject.Data.Id = fobject.Id;
            fobject.Data.FigmaConverterUnity = fcu;
            fobject.Data.GameObject = syncHelper.gameObject;
            fobject.Data.GameObject.name = fobject.Data.NewName;

            syncHelper.Data = fobject.Data;

            if (fobject.Type == NodeType.TEXT)
            {
                fobject.Data.HumanizedTextPrefabName = fcu.NameHumanizer.GetHumanizedTextPrefabName(fobject);
            }
        }

        public static bool TryGetChild<T>(this Transform parent, int index, out T child) where T : MonoBehaviour
        {
            try
            {
                Transform childTransform = parent.GetChild(index);
                return childTransform.TryGetComponent(out child);
            }
            catch
            {
                child = default;
                return false;
            }
        }
    }
}