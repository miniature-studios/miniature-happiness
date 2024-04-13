using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU
{
    public class PreImportDataCreator
    {
        public const string TO_IMPORT_MENU_ID = "-1000000";
        public const string TO_REMOVE_MENU_ID = "-1000001";

        internal static PreImportInput GetPreImportData(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            SelectableObject<DiffInfo> toImport = GetToImport(fobjects, syncHelpers);
            toImport.SetAllSelected(true);

            SelectableObject<SyncData> toRemove = GetToRemove(fobjects, syncHelpers);
            toRemove.SetAllSelected(false);

            DALogger.Log($"GetPreImportData | toImport: {toImport.Childs.Count} | toRemove: {toRemove.Childs.Count}");

            return new PreImportInput
            {
                ToImport = toImport,
                ToRemove = toRemove,
            };
        }

        private static SelectableObject<DiffInfo> GetToImport(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            SelectableObject<DiffInfo> toImport = new SelectableObject<DiffInfo>
            {
                Object = new DiffInfo
                {
                    Id = TO_IMPORT_MENU_ID,
                    Name = TO_IMPORT_MENU_ID
                }
            };

            Dictionary<string, DiffInfo> allObjects = new Dictionary<string, DiffInfo>();

            foreach (SyncHelper syncHelper in syncHelpers)
            {
                if (syncHelper.Data.RootFrame == null)
                {
                    DALogger.LogError($"RootFrame is null for '{syncHelper.gameObject.name}', skip.");
                    continue;
                }

                allObjects[syncHelper.Data.Id] = new DiffInfo
                {
                    Id = syncHelper.Data.Id,
                    IsFrame = syncHelper.ContainsTag(FcuTag.Frame),
                    RootFrame = syncHelper.Data.RootFrame,
                    Name = syncHelper.gameObject.name,
                    OldData = syncHelper.Data
                };
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data?.RootFrame == null)
                {
                    DALogger.LogError($"RootFrame is null for '{fobject.Name}', skip.");
                    continue;
                }

                if (allObjects.TryGetValue(fobject.Id, out DiffInfo diffModel))
                {
                    diffModel.NewData = fobject;
                }
                else
                {
                    diffModel = new DiffInfo();
                    diffModel.Name = fobject.Name;
                    diffModel.IsFrame = fobject.ContainsTag(FcuTag.Frame);
                    diffModel.Id = fobject.Id;
                    diffModel.RootFrame = fobject.Data.RootFrame;
                    diffModel.IsNew = true;
                    diffModel.NewData = fobject;
                }

                allObjects[fobject.Id] = diffModel;
            }

            allObjects = allObjects.Where(x => !x.Value.NewData.IsDefault()).ToDictionary(x => x.Key, x => x.Value);

            Dictionary<string, DiffInfo> allObjectsWithDiffFlags = new Dictionary<string, DiffInfo>();

            foreach (KeyValuePair<string, DiffInfo> obj in allObjects)
            {
                DiffInfo di = obj.Value;

                if (obj.Value.OldData != null && !obj.Value.NewData.IsDefault())
                {
                    if (di.OldData.HashData == di.NewData.Data.HashData)
                    {
                        di.HasFigmaDiff = false;
                    }
                    else
                    {
                        di.HasFigmaDiff = true;
                    }

                    if (di.OldData.GameObject.TryGetComponent(out RectTransform rectTransform))
                    {
                        Vector2 oldSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

                        if (oldSize.Round(2).Equals(di.NewData.Size.Round(2)) == false)
                        {
                            di.Size = new TProp<Vector2, Vector2>(true, oldSize, di.NewData.Size);
                        }
                        else
                        {
                            di.Size = new TProp<Vector2, Vector2>(false, default, default);
                        }
                    }

                    if (di.OldData.GameObject.TryGetComponent(out Graphic oldGraphic))
                    {
                        if (!di.NewData.Fills.IsEmpty() && oldGraphic.color != di.NewData.Fills[0].Color)
                        {
                            di.Color = new TProp<Color, Color>(true, oldGraphic.color, di.NewData.Fills[0].Color);
                        }
                        else
                        {
                            di.Color = new TProp<Color, Color>(false, default, default);
                        }
                    }
                }

                allObjectsWithDiffFlags.Add(obj.Key, di);
            }

            allObjects = allObjectsWithDiffFlags;

            Dictionary<string, SelectableObject<DiffInfo>> selectableObjects = new Dictionary<string, SelectableObject<DiffInfo>>();

            foreach (DiffInfo obj in allObjects.Values)
            {
                if (!obj.IsFrame)
                    continue;

                selectableObjects.Add(obj.RootFrame.Id, new SelectableObject<DiffInfo>
                {
                    Object = obj,
                    Childs = new List<SelectableObject<DiffInfo>>()
                });
            }

            foreach (DiffInfo obj in allObjects.Values)
            {
                if (obj.IsFrame)
                    continue;

                selectableObjects[obj.RootFrame.Id].Childs.Add(new SelectableObject<DiffInfo>
                {
                    Object = obj,
                    Childs = new List<SelectableObject<DiffInfo>>()
                });
            }

            toImport.Childs = selectableObjects.Values.ToList();

            return toImport;
        }

        private static SelectableObject<SyncData> GetToRemove(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            var toRemove = new SelectableObject<SyncData>
            {
                Object = new SyncData
                {
                    Id = TO_REMOVE_MENU_ID
                }
            };

            fobjects = fobjects.Where(x => x.Data?.RootFrame != null).ToList();
            syncHelpers = syncHelpers.Where(x => x.Data?.RootFrame != null).ToList();

            IEnumerable<SelectableObject<SyncData>> syncHelpersByFrame = syncHelpers
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new SelectableObject<SyncData>
                {
                    Childs = g.Select(x => new SelectableObject<SyncData>
                    {
                        Object = x.Data
                    }).ToList(),
                    Object = g.First().Data
                });

            IEnumerable<FrameGroup> fobjectsByFrame = fobjects
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new FrameGroup
                {
                    Childs = g.Select(x => x).ToList(),
                    RootFrame = g.First()
                });

            foreach (SelectableObject<SyncData> syncGroup in syncHelpersByFrame)
            {
                SelectableObject<SyncData> selectableObj = new SelectableObject<SyncData>();
                selectableObj.Object = syncGroup.Object;
                selectableObj.Childs = new List<SelectableObject<SyncData>>();

                foreach (SelectableObject<SyncData> onSceneObj in syncGroup.Childs)
                {
                    if (onSceneObj.Object.Tags.Contains(FcuTag.Frame))
                        continue;

                    bool isMissing = true;
                    foreach (FrameGroup frameGroup in fobjectsByFrame)
                    {
                        if (frameGroup.RootFrame.Id != syncGroup.Object.Id)
                            continue;

                        foreach (FObject fobject in frameGroup.Childs)
                        {
                            if (fobject.Id == onSceneObj.Object.Id)
                            {
                                isMissing = false;
                                break;
                            }
                        }
                    }

                    if (isMissing)
                    {
                        selectableObj.Childs.Add(onSceneObj);
                    }
                }

                if (!selectableObj.Childs.IsEmpty())
                {
                    toRemove.Childs.Add(selectableObj);
                }
            }

            return toRemove;
        }
    }

    public struct PreImportInput
    {
        public SelectableObject<DiffInfo> ToImport { get; set; }
        public SelectableObject<SyncData> ToRemove { get; set; }
    }

    public struct PreImportOutput
    {
        public IEnumerable<string> ToImport { get; set; }
        public IEnumerable<SyncData> ToRemove { get; set; }
    }

    public class SelectableObject<T>
    {
        [SerializeField] bool selected;
        public bool Selected { get => selected; set => selected = value; }

        [SerializeField] List<SelectableObject<T>> childs = new List<SelectableObject<T>>();
        public List<SelectableObject<T>> Childs { get => childs; set => childs = value; }

        public T Object { get; set; }

        public void SetAllSelected(bool value)
        {
            selected = value;

            foreach (SelectableObject<T> child in childs)
            {
                child.SetAllSelected(value);
            }
        }
    }

    public class DiffInfo : IHaveId
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsFrame { get; set; }
        public SyncData RootFrame { get; set; }

        public FObject NewData { get; set; }
        public SyncData OldData { get; set; }
        public bool IsNew { get; set; }
        public bool HasFigmaDiff { get; set; }
        public TProp<Color, Color> Color { get; set; }
        public TProp<Vector2, Vector2> Size { get; set; }
    }

    public interface IHaveId
    {
        string Id { get; set; }
    }
}
