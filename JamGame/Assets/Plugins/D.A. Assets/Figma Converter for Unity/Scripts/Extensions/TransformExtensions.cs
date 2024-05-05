using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class TransformExtensions
    {
        public static bool IsNeedRotate(this FObject fobject, FigmaConverterUnity fcu)
        {
            float angle = fobject.Data.Angle;
            bool needRotate = angle != 0 && fobject.Data.FcuImageType != FcuImageType.Downloadable;
            fcu.Log($"IsNeedRotate | {fobject.Data.Hierarchy} | needRotate: {needRotate} | angle: {angle} | {fobject.Data.FcuImageType}");
            return needRotate;
        }

        public static void SetFigmaRotation(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotationAngle;

            RectTransform rect = fobject.Data.GameObject.GetComponent<RectTransform>();

            if (fobject.IsNeedRotate(fcu))
            {
                rotationAngle = fobject.Data.Angle;
            }
            else
            {
                rotationAngle = 0;
            }

            rect.SetRotation(rotationAngle);
        }

        public static float GetAngleFromMatrix(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotRad;
            float rotDeg;
            float a;
            float b;

            if (fobject.RelativeTransform.IsEmpty() ||
                fobject.RelativeTransform.Count < 2 ||
                fobject.RelativeTransform[1].Count < 2 ||
                fobject.RelativeTransform[1][0] == null ||
                fobject.RelativeTransform[1][1] == null)
            {
                fcu.Log($"GetFigmaRotationAngle | {fobject.Data.Hierarchy} | wrong relative transform.", FcuLogType.Error);
                return 0;
            }
            else
            {
                a = (float)fobject.RelativeTransform[1][0];
                b = (float)fobject.RelativeTransform[1][1];
                rotRad = Mathf.Atan2(a, b);
            }

            rotRad = -1 * rotRad;

            rotDeg = rotRad * (180f / (float)Mathf.PI);

            fcu.Log($"GetFigmaRotationAngle | {fobject.Data.Hierarchy} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}");

            return rotDeg;
        }

        public static float GetAngleFromField(this FObject fobject, FigmaConverterUnity fcu)
        {
            float rotRad;
            float rotDeg;
            float a = 0f;
            float b = 0f;

            if (fobject.Rotation != null)
            {
                rotRad = fobject.Rotation.Value;
                fcu.Log($"GetFigmaRotationAngle | 1 | {fobject.Data.Hierarchy} | rotRad: {rotRad}");
            }
            else
            {
                return 0;
            }

            rotRad = -1 * rotRad;

            rotDeg = rotRad * (180f / (float)Mathf.PI);

            fcu.Log($"GetFigmaRotationAngle | {fobject.Data.Hierarchy} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}");

            return rotDeg;
        }

        public static float GetFigmaRotationAngle(this FObject fobject, FigmaConverterUnity fcu)
        {
            float angle = fobject.GetAngleFromField(fcu);

            if (angle == 0)
                angle = fobject.GetAngleFromMatrix(fcu);

            return angle;
        }

        public static AnchorType GetFigmaAnchor(this FObject fobject)
        {
            string anchor = fobject.Constraints.Vertical + " " + fobject.Constraints.Horizontal;

            AnchorType anchorPreset;

            switch (anchor)
            {
                ////////////////LEFT////////////////
                case "TOP LEFT":
                    anchorPreset = AnchorType.TopLeft;
                    break;
                case "BOTTOM LEFT":
                    anchorPreset = AnchorType.BottomLeft;
                    break;
                case "TOP_BOTTOM LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                case "CENTER LEFT":
                    anchorPreset = AnchorType.MiddleLeft;
                    break;
                case "SCALE LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                ////////////////RIGHT////////////////
                case "TOP RIGHT":
                    anchorPreset = AnchorType.TopRight;
                    break;
                case "BOTTOM RIGHT":
                    anchorPreset = AnchorType.BottomRight;
                    break;
                case "TOP_BOTTOM RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                case "CENTER RIGHT":
                    anchorPreset = AnchorType.MiddleRight;
                    break;
                case "SCALE RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                ////////////////LEFT_RIGHT////////////////
                case "TOP LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "CENTER LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                case "SCALE LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                ////////////////CENTER////////////////
                case "TOP CENTER":
                    anchorPreset = AnchorType.TopCenter;
                    break;
                case "BOTTOM CENTER":
                    anchorPreset = AnchorType.BottomCenter;
                    break;
                case "TOP_BOTTOM CENTER":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER CENTER":
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
                case "SCALE CENTER":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////SCALE////////////////
                case "TOP SCALE":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM SCALE":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM SCALE":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "SCALE SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////DEFAULT////////////////
                default:
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
            }

            return anchorPreset;
        }
    }
}