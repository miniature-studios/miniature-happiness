using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class HashGenerator : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void SetHashes(List<FObject> fobjects)
        {
            Parallel.ForEach(fobjects, fobject =>
            {
                List<FObjectHashData> hashes = new List<FObjectHashData>();
                GetHashRecursive(fobject, hashes, 0);

                List<FObjectHashData> sortedHashes = hashes.OrderBy(x => x.Indent).ToList();

                string hashToCalculate = ConvertToTreeString(hashes, false);
                string hashDataTree = ConvertToTreeString(sortedHashes, true);

                fobject.Data.Hash = hashToCalculate.GetDeterministicHashCode();
                if (fobject.IsDownloadableType())
                {
                    fobject.Data.HashDataTree = hashDataTree;
                }

                StringBuilder sb = new StringBuilder();
                AddFirstLevel(hashes.First(), true, sb);
                fobject.Data.HashData = sb.ToString();
            });
        }

        private void GetHashRecursive(FObject fobject, List<FObjectHashData> values, int indent)
        {
            FObjectHashData fobjectHash = new FObjectHashData(fobject, new List<FieldHashData>(), new List<EffectHashData>(), indent);

            /*  try
              {*/
            fobjectHash.FieldHashes.Add(new FieldHashData("isVisible", fobject.IsVisible()));

            fobject.Data.InsideDownloadable = fobject.IsInsideDownloadable(monoBeh, out FObject downloadableFObject);

            monoBeh.Log($"IsInsideDownloadable | {fobject.Data.Hierarchy} | {downloadableFObject.Name}", FcuLogType.HashGenerator);

            if (fobject.Data.InsideDownloadable)
            {
                if (fobject.TryGetLocalPosition(out Vector3 rtPos))
                {
                    fobjectHash.FieldHashes.Add(new FieldHashData(nameof(rtPos), rtPos));
                }
            }

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.Type), fobject.Type));
            fobjectHash.FieldHashes.Add(new FieldHashData("GetFigmaRotationAngle", fobject.GetFigmaRotationAngle(monoBeh)));

            if (fobject.FillGeometry.IsEmpty() == false)
            {
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.FillGeometry), fobject.FillGeometry[0].Path.GetDeterministicHashCode()));
            }

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeWeight), fobject.StrokeWeight));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeAlign), fobject.StrokeAlign));

            if (fobject.CornerRadiuses.IsEmpty())
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.CornerRadius), fobject.CornerRadius));
            else
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.CornerRadiuses), string.Join(" ", fobject.CornerRadiuses)));

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.BlendMode), fobject.BlendMode));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.Opacity), fobject.Opacity));

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.ClipsContent), fobject.ClipsContent));

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.LayoutMode), fobject.LayoutMode));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.ItemSpacing), fobject.ItemSpacing));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.PrimaryAxisAlignItems), fobject.PrimaryAxisAlignItems));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.CounterAxisAlignItems), fobject.CounterAxisAlignItems));

            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.PaddingLeft), fobject.PaddingLeft));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.PaddingRight), fobject.PaddingRight));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.PaddingTop), fobject.PaddingTop));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.PaddingBottom), fobject.PaddingBottom));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.HorizontalPadding), fobject.HorizontalPadding));
            fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.VerticalPadding), fobject.VerticalPadding));

            if (fobject.Type == NodeType.VECTOR)
            {
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeCap), fobject.StrokeCap));
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeJoin), fobject.StrokeJoin));
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeMiterAngle), fobject.StrokeMiterAngle));

                if (fobject.StrokeDashes.IsEmpty() == false)
                    fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.StrokeDashes), string.Join(" ", fobject.StrokeDashes)));
            }

            if (fobject.Type == NodeType.TEXT)
            {
                fobjectHash.EffectDatas.Add(GetHash("TextStyle", fobject.Style));
            }
            else
            {
                fobjectHash.FieldHashes.Add(new FieldHashData(nameof(fobject.Size), fobject.Size));
            }

            fobjectHash.EffectDatas.Add(GetHash(nameof(fobject.Fills), fobject, fobject.Fills));
            fobjectHash.EffectDatas.Add(GetHash(nameof(fobject.Effects), fobject, fobject.Effects));
            fobjectHash.EffectDatas.Add(GetHash(nameof(fobject.Strokes), fobject, fobject.Strokes));
            fobjectHash.EffectDatas.Add(GetHash(nameof(fobject.ArcData), fobject.ArcData));

            fobjectHash.FieldHashes.AddRange(fobjectHash.FieldHashes);
            /*  }
              catch (Exception ex)
              {
                  monoBeh.Log(ex, FcuLogType.Error);
              }*/

            values.Add(fobjectHash);

            if (fobject.Children.IsEmpty() == false)
            {
                for (int i = 0; i < fobject.Children.Count; i++)
                {
                    GetHashRecursive(fobject.Children[i], values, fobjectHash.Indent + i);
                }
            }
        }

        private EffectHashData GetHash(string effectName, Style style)
        {
            EffectHashData effectData = new EffectHashData(effectName, new List<FieldHashData>());

            if (style.IsDefault())
                return effectData;

            try
            {
                effectData.Data.Add(new FieldHashData(nameof(style.FontFamily), style.FontFamily));
                effectData.Data.Add(new FieldHashData(nameof(style.FontWeight), style.FontWeight));
                effectData.Data.Add(new FieldHashData(nameof(style.FontSize), style.FontSize));
                effectData.Data.Add(new FieldHashData(nameof(style.TextAlignHorizontal), style.TextAlignHorizontal));
                effectData.Data.Add(new FieldHashData(nameof(style.TextAlignVertical), style.TextAlignVertical));
                //effectData.Data.Add(style.LetterSpacing);
                //effectData.Data.Add(style.LineHeightPx);
            }
            catch (Exception ex)
            {
                monoBeh.Log(ex, FcuLogType.Error);
            }

            return effectData;
        }

        private EffectHashData GetHash(string effectName, FObject fobject, List<Paint> fills)
        {
            EffectHashData effectData = new EffectHashData(effectName, new List<FieldHashData>());

            if (fills.IsEmpty())
                return effectData;

            foreach (Paint item in fills)
            {
                try
                {
                    effectData.Data.Add(new FieldHashData("IsVisible", item.IsVisible()));

                    effectData.Data.Add(new FieldHashData(nameof(item.Type), item.Type));

                    effectData.Data.Add(new FieldHashData(nameof(item.Opacity), item.Opacity));

                    if (fobject.Data.SingleColor.IsDefault())
                    {
                        effectData.Data.Add(new FieldHashData(nameof(item.Color), item.Color));
                    }
                    else
                    {
                        effectData.Data.Add(new FieldHashData(nameof(item.Color), Color.white));
                    }

                    effectData.Data.Add(new FieldHashData(nameof(item.BlendMode), item.BlendMode));

                    effectData.Data.Add(new FieldHashData(nameof(item.ScaleMode), item.ScaleMode));
                    effectData.Data.Add(new FieldHashData(nameof(item.ScalingFactor), item.ScalingFactor));
                    effectData.Data.Add(new FieldHashData(nameof(item.Rotation), item.Rotation));

                    effectData.Data.Add(new FieldHashData(nameof(item.ImageRef), item.ImageRef));
                    effectData.Data.Add(new FieldHashData(nameof(item.GifRef), item.GifRef));

                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Exposure), item.Filters.Exposure));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Contrast), item.Filters.Contrast));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Saturation), item.Filters.Saturation));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Temperature), item.Filters.Temperature));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Tint), item.Filters.Tint));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Highlights), item.Filters.Highlights));
                    effectData.Data.Add(new FieldHashData(nameof(item.Filters.Shadows), item.Filters.Shadows));

                    if (item.ImageTransform.IsEmpty() == false)
                    {
                        List<float> floats = item.ImageTransform.SelectMany(x => x).ToList();
                        effectData.Data.Add(new FieldHashData(nameof(item.ImageTransform), floats));
                    }

                    if (item.GradientStops.IsEmpty() == false)
                    {
                        foreach (GradientStop gs in item.GradientStops)
                        {
                            if (fobject.Data.SingleColor.IsDefault())
                            {
                                effectData.Data.Add(new FieldHashData(nameof(gs.Color), gs.Color));
                            }
                            else
                            {
                                effectData.Data.Add(new FieldHashData(nameof(gs.Color), Color.white));
                            }

                            effectData.Data.Add(new FieldHashData(nameof(gs.Position), gs.Position));
                        }
                    }

                    if (item.GradientHandlePositions.IsEmpty() == false)
                    {
                        effectData.Data.Add(new FieldHashData(nameof(item.GradientHandlePositions), string.Join(" ", item.GradientHandlePositions)));
                    }
                }
                catch (Exception ex)
                {
                    monoBeh.Log(ex, FcuLogType.Error);
                }
            }

            return effectData;
        }

        private EffectHashData GetHash(string effectName, ArcData arcData)
        {
            EffectHashData effectData = new EffectHashData(effectName, new List<FieldHashData>());

            try
            {
                effectData.Data.Add(new FieldHashData(nameof(arcData.StartingAngle), arcData.StartingAngle));
                effectData.Data.Add(new FieldHashData(nameof(arcData.EndingAngle), arcData.EndingAngle));
                effectData.Data.Add(new FieldHashData(nameof(arcData.InnerRadius), arcData.InnerRadius));
            }
            catch (Exception ex)
            {
                monoBeh.Log(ex, FcuLogType.Error);
            }

            return effectData;
        }

        private EffectHashData GetHash(string effectName, FObject fobject, List<Effect> effects)
        {
            EffectHashData effectData = new EffectHashData(effectName, new List<FieldHashData>());

            if (effects.IsEmpty())
                return effectData;

            foreach (Effect item in effects)
            {
                try
                {
                    effectData.Data.Add(new FieldHashData("IsVisible", item.IsVisible()));

                    effectData.Data.Add(new FieldHashData(nameof(item.Type), item.Type));
                    effectData.Data.Add(new FieldHashData(nameof(item.Radius), item.Radius));

                    if (fobject.Data.SingleColor.IsDefault())
                    {
                        effectData.Data.Add(new FieldHashData(nameof(item.Color), item.Color));
                    }
                    else
                    {
                        effectData.Data.Add(new FieldHashData(nameof(item.Color), Color.white));
                    }

                    effectData.Data.Add(new FieldHashData(nameof(item.BlendMode), item.BlendMode));
                    effectData.Data.Add(new FieldHashData(nameof(item.Offset), item.Offset));
                    effectData.Data.Add(new FieldHashData(nameof(item.Spread), item.Spread));
                    effectData.Data.Add(new FieldHashData(nameof(item.ShowShadowBehindNode), item.ShowShadowBehindNode));
                }
                catch (Exception ex)
                {
                    monoBeh.Log(ex, FcuLogType.Error);
                }
            }

            return effectData;
        }

        static void Dash(StringBuilder sb, int indentLevel)
        {
            sb.Append(new string('—', indentLevel * 4));
        }

        static void Indent(StringBuilder sb, int indentLevel)
        {
            sb.Append(new string(' ', indentLevel * 4));
        }

        private string ConvertToTreeString(List<FObjectHashData> objectHashes, bool includeNames)
        {
            StringBuilder sb = new StringBuilder();

            foreach (FObjectHashData objectHash in objectHashes)
            {
                AddFirstLevel(objectHash, includeNames, sb);
            }

            return sb.ToString();
        }
        private void AddFirstLevel(FObjectHashData objectHash, bool includeNames, StringBuilder sb)
        {
            if (includeNames)
            {
                Indent(sb, objectHash.Indent);
                Dash(sb, 1);
                sb.AppendLine($"{objectHash.FObject.Data.Hierarchy}");
            }

            foreach (FieldHashData fd in objectHash.FieldHashes)
            {
                if ($"{fd.Data}".IsEmpty())
                    continue;

                if (includeNames)
                {
                    Indent(sb, objectHash.Indent + 1);
                }

                sb.AppendLine($"{fd.Name} | {fd.Data}");
            }

            foreach (EffectHashData effectData in objectHash.EffectDatas)
            {
                if (effectData.Data.IsEmpty())
                    continue;

                if (includeNames)
                {
                    Indent(sb, objectHash.Indent + 1);
                    Dash(sb, 1);
                    sb.AppendLine($"{effectData.Name}");
                }

                foreach (FieldHashData fd in effectData.Data)
                {
                    if ($"{fd.Data}".IsEmpty())
                        continue;

                    if (includeNames)
                    {
                        Indent(sb, objectHash.Indent + 2);
                    }

                    sb.AppendLine($"{fd.Name} | {fd.Data}");
                }
            }
        }

    }
    public struct FObjectHashData
    {
        private int indent;
        private FObject fobject;
        private List<FieldHashData> hashes;
        public List<EffectHashData> effectDatas;

        public FObjectHashData(FObject fobject, List<FieldHashData> hashes, List<EffectHashData> effectDatas, int indent)
        {
            this.fobject = fobject;
            this.hashes = hashes;
            this.indent = indent;
            this.effectDatas = effectDatas;
        }

        public int Indent => indent;
        public FObject FObject => fobject;
        public List<FieldHashData> FieldHashes => hashes;
        public List<EffectHashData> EffectDatas => effectDatas;
    }

    public struct FieldHashData
    {
        private string name;
        private object data;

        public FieldHashData(string name, object data)
        {
            this.name = name;
            this.data = data;
        }

        public string Name => name;
        public object Data => data;
    }

    public struct EffectHashData
    {
        private string name;
        private List<FieldHashData> data;

        public EffectHashData(string name, List<FieldHashData> data)
        {
            this.name = name;
            this.data = data;
        }

        public string Name => name;
        public List<FieldHashData> Data => data;
    }
}
