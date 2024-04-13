/*MIT License

Copyright (c) 2019 Kirill Evdokimov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using DA_Assets.Shared.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.Shared
{
    public class CornerRounder : MonoBehaviour
    {
        private static readonly int prop_halfSize = Shader.PropertyToID("_halfSize");
        private static readonly int prop_radiuses = Shader.PropertyToID("_r");
        private static readonly int prop_rect2props = Shader.PropertyToID("_rect2props");

        // Vector2.right rotated clockwise by 45 degrees
        private static readonly Vector2 wNorm = new Vector2(.7071068f, -.7071068f);
        // Vector2.right rotated counter-clockwise by 45 degrees
        private static readonly Vector2 hNorm = new Vector2(.7071068f, .7071068f);

        public bool independent = false;

        public Vector4 radiiSerialized = new Vector4(40f, 40f, 40f, 40f);
        private Vector4 _radii => GetNormalizedRadii();

        private Vector4 GetNormalizedRadii()
        {
            float width = GetComponent<RectTransform>().GetWidth();
            float height = GetComponent<RectTransform>().GetHeight();

            Vector4 r = new Vector4();
            r.x = radiiSerialized.x.NormalizeAngleToSize(width, height);
            r.y = radiiSerialized.y.NormalizeAngleToSize(width, height);
            r.z = radiiSerialized.z.NormalizeAngleToSize(width, height);
            r.w = radiiSerialized.w.NormalizeAngleToSize(width, height);
            return r;
        }

        private Material material;

        // xy - position,
        // zw - halfSize
        [HideInInspector, SerializeField] private Vector4 rect2props;
        [HideInInspector, SerializeField] private MaskableGraphic image;

        public void SetRadii(Vector4 r)
        {
            if (r.x == r.y &&
                r.x == r.z &&
                r.x == r.w)
            {
                independent = false;
            }
            else
            {
                independent = true;
            }

            radiiSerialized = r;
            Refresh();
        }

        private void OnValidate()
        {
            Validate();
            Refresh();
        }

        private void OnEnable()
        {
            Validate();
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (enabled && material != null)
            {
                Refresh();
            }
        }

        private void OnDestroy()
        {
            image.material = null;      //This makes so that when the component is removed, the UI material returns to null

            material.Destroy();
            image = null;
            material = null;
        }

        public void Validate()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("UI/RoundedCorners/CornerRounder"));
            }

            if (image == null)
            {
                TryGetComponent(out image);
            }

            if (image != null)
            {
                image.material = material;
            }
        }

        public void Refresh()
        {
            Rect rect = ((RectTransform)transform).rect;
            RecalculateProps(rect.size);
            material.SetVector(prop_rect2props, rect2props);
            material.SetVector(prop_halfSize, rect.size * 0.5f);
            material.SetVector(prop_radiuses, _radii);
        }

        private void RecalculateProps(Vector2 size)
        {
            // Vector that goes from left to right sides of rect2
            Vector2 aVec = new Vector2(size.x, -size.y + _radii.x + _radii.z);

            // Project vector aVec to wNorm to get magnitude of rect2 width vector
            float halfWidth = Vector2.Dot(aVec, wNorm) * .5f;
            rect2props.z = halfWidth;


            // Vector that goes from bottom to top sides of rect2
            Vector2 bVec = new Vector2(size.x, size.y - _radii.w - _radii.y);

            // Project vector bVec to hNorm to get magnitude of rect2 height vector
            float halfHeight = Vector2.Dot(bVec, hNorm) * .5f;
            rect2props.w = halfHeight;


            // Vector that goes from left to top sides of rect2
            Vector2 efVec = new Vector2(size.x - _radii.x - _radii.y, 0);

            // Vector that goes from point E to point G, which is top-left of rect2
            Vector2 egVec = hNorm * Vector2.Dot(efVec, hNorm);

            // Position of point E relative to center of coord system
            Vector2 ePoint = new Vector2(_radii.x - (size.x / 2), size.y / 2);

            // Origin of rect2 relative to center of coord system
            // ePoint + egVec == vector to top-left corner of rect2
            // wNorm * halfWidth + hNorm * -halfHeight == vector from top-left corner to center
            Vector2 origin = ePoint + egVec + wNorm * halfWidth + hNorm * -halfHeight;
            rect2props.x = origin.x;
            rect2props.y = origin.y;
        }
    }
}
