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
    public class TransformSetter : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator SetTransformPos(List<FObject> fobjects)
        {
            DALogger.Log(FcuLocKey.log_start_setting_transform.Localize());

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                fobject.Data.Angle = fobject.GetFigmaRotationAngle(monoBeh);
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                RectTransform rt = fobject.Data.GameObject.GetComponent<RectTransform>();
                rt.SetSmartAnchor(AnchorType.TopLeft);
                rt.SetSmartPivot(PivotType.TopLeft);

                Rect rect = GetGlobalRect(fobject);
                fobject.Data.Size = rect.size;
                fobject.Data.Position = rect.position;

                rt.sizeDelta = rect.size;
                rt.position = rect.position;

                fobject.Data.GameObject.transform.localScale = Vector3.one;
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (!fobject.IsNeedRotate(monoBeh))
                    continue;

                RectTransform rt = fobject.Data.GameObject.GetComponent<RectTransform>();

                Rect rect = GetRotatedRect(fobject);

                if (fobject.Type == NodeType.RECTANGLE)
                {
                    rt.SetSmartPivot(PivotType.BottomLeft);
                }
                //TODO: Figma API Bug: if an object has a Rotation field,
                //but its matrix looks as though the object has no rotation angle,
                //the object needs to change its Pivot.
                else if (fobject.Type == NodeType.GROUP)
                {
                    float fa = fobject.GetAngleFromField(monoBeh);
                    float ma = fobject.GetAngleFromMatrix(monoBeh);

                    if (fa != 0 && ma == 0)
                    {
                        rt.SetSmartPivot(PivotType.BottomLeft);
                    }
                }

                rt.localPosition = rect.position;
                fobject.SetFigmaRotation(monoBeh);
            }

            yield return null;
        }

        public IEnumerator SetTransformPosAndAnchors(List<FObject> fobjects)
        {
            DALogger.Log(FcuLocKey.log_start_setting_transform.Localize());

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (!fobject.IsInsideAutoLayout(out FObject _, out var __, monoBeh))
                    continue;

                RectTransform rt = fobject.Data.GameObject.GetComponent<RectTransform>();
                Rect rect = GetAutolayoutRect(fobject);
                fobject.Data.Size = rect.size;
                rt.sizeDelta = fobject.Data.Size;
            }

            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                RectTransform rt = fobject.Data.GameObject.GetComponent<RectTransform>();

                rt.SetSmartPivot(monoBeh.Settings.MainSettings.PivotType);

                if (!fobject.ContainsTag(FcuTag.Frame) && !fobject.Data.Parent.ContainsTag(FcuTag.AutoLayoutGroup))
                {
                    rt.SetSmartAnchor(fobject.GetFigmaAnchor());
                }
            }

            yield return SetRootFramesPosition(fobjects);

            yield return monoBeh.ReEnableRectTransform();
        }

        public Rect GetAutolayoutRect(FObject fobject)
        {
            Rect rect = new Rect();
            Vector2 position = new Vector2();
            Vector2 size = new Vector2();

            size = fobject.Data.Size;

            if (fobject.TryFixSizeWithStroke(size.y, out float newY))
            {
                size.y = newY;
            }

            monoBeh.Log($"{nameof(GetAutolayoutRect)} | {fobject.Data.Hierarchy} | {size} | {position}", FcuLogType.Transform);

            rect.size = size;
            rect.position = position;

            return rect;
        }

        public Rect GetRotatedRect(FObject fobject)
        {
            Rect rect = new Rect();
            Vector2 position = new Vector2();
            Vector2 size = new Vector2();

            bool hasLocalPos = fobject.TryGetLocalPosition(out Vector3 lPos);

            position.x = lPos.x;
            position.y = lPos.y;

            size.x = fobject.Size.x;
            size.y = fobject.Size.y;

            if (fobject.TryFixSizeWithStroke(size.y, out float newY))
            {
                size.y = newY;
            }

            monoBeh.Log($"{nameof(GetRotatedRect)} | {fobject.Data.Hierarchy} | {size} | {position}", FcuLogType.Transform);

            rect.size = size;
            rect.position = position;

            return rect;
        }

        public Rect GetGlobalRect(FObject fobject)
        {
            Rect rect = new Rect();
            Vector2 position = new Vector2();
            Vector2 size = new Vector2();

            bool hasBoundingSize = fobject.GetBoundingSize(out Vector2 bSize);
            bool hasBoundingPos = fobject.GetBoundingPosition(out Vector2 bPos);

            bool hasRenderSize = fobject.GetRenderSize(out Vector2 rSize);
            bool hasRenderPos = fobject.GetRenderPosition(out Vector2 rPos);

            bool hasLocalPos = fobject.TryGetLocalPosition(out Vector3 lPos);

            int state = 0;

            if (fobject.Data.FcuImageType == FcuImageType.Downloadable && rPos != bPos)
            {
                bool hasScaleInName = fobject.Data.SpritePath.TryParseSpriteName(out float scale, out var _);

                if (hasRenderPos)
                {
                    state = 1;

                    position.x = rPos.x;
                    position.y = monoBeh.IsUGUI() ? -rPos.y : rPos.y;
                }
                else
                {
                    state = 2;

                    position.x = bPos.x;
                    position.y = monoBeh.IsUGUI() ? -bPos.y : bPos.y;
                }

                size.x = fobject.Data.SpriteSize.x / scale;
                size.y = fobject.Data.SpriteSize.y / scale;
            }
            else
            {
                state = 4;

                position.x = bPos.x;
                position.y = monoBeh.IsUGUI() ? -bPos.y : bPos.y;

                size.x = fobject.Size.x;
                size.y = fobject.Size.y;
            }

            if (fobject.TryFixSizeWithStroke(size.y, out float newY))
            {
                size.y = newY;
            }

            monoBeh.Log($"{nameof(GetGlobalRect)} | {fobject.Data.Hierarchy} | state: {state} | {size} | {position}", FcuLogType.Transform);

            rect.size = size;
            rect.position = position;

            return rect;
        }

        private IEnumerator SetRootFramesPosition(List<FObject> fobjects)
        {
            IEnumerable<FrameGroup> fobjectsByFrame = fobjects
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new FrameGroup
                {
                    Childs = g.Select(x => x).ToList(),
                    RootFrame = g.First()
                });

            if (monoBeh.Settings.MainSettings.PositioningMode == PositioningMode.Absolute)
            {
                foreach (FrameGroup rootFrame in fobjectsByFrame)
                {
                    if (rootFrame.RootFrame.Data.GameObject == null)
                        continue;

                    RectTransform rt = rootFrame.RootFrame.Data.GameObject.GetComponent<RectTransform>();
                    rt.SetSmartAnchor(AnchorType.TopLeft);
                }
                /*
                List<float> posXs = new List<float>();
                List<float> posYs = new List<float>();

                foreach (FrameGroup rootFrame in fobjectsByFrame)
                {
                    if (rootFrame.RootFrame.Data.GameObject == null)
                        continue;

                    RectTransform rt = rootFrame.RootFrame.Data.GameObject.GetComponent<RectTransform>();
                    posXs.Add(rt.anchoredPosition.x);
                    posYs.Add(rt.anchoredPosition.y);
                }
                
                float minX = posXs.Min();
                float maxY = posYs.Max();

                Debug.LogError($"MIN | {minX} | {maxY}");

                foreach (FrameGroup rootFrame in fobjectsByFrame)
                {
                    if (rootFrame.RootFrame.Data.GameObject == null)
                        continue;

                    RectTransform rt = rootFrame.RootFrame.Data.GameObject.GetComponent<RectTransform>();
                    rt.position = new Vector3(rt.position.x - minX, rt.position.y - maxY, 0);
                }*/
            }
            else
            {
                monoBeh.AssetTools.CacheResolutionData();

                foreach (FrameGroup rootFrame in fobjectsByFrame)
                {
                    if (rootFrame.RootFrame.Data.GameObject == null)
                        continue;

                    yield return WaitFor.Delay001();
                    monoBeh.DelegateHolder.SetGameViewSize(rootFrame.RootFrame.Size);
                    yield return WaitFor.Delay01();

                    RectTransform rt = rootFrame.RootFrame.Data.GameObject.GetComponent<RectTransform>();
                    rt.SetSmartAnchor(AnchorType.StretchAll);
                    rt.offsetMin = new Vector2(0, 0);
                    rt.offsetMax = new Vector2(0, 0);
                    rt.localScale = Vector3.one;
                }

                monoBeh.AssetTools.RestoreResolutionData();
            }
        }
    }

    public struct FrameGroup
    {
        public FObject RootFrame { get; set; }
        public List<FObject> Childs { get; set; }
    }
}