using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DA_Assets.Shared;
using System.Collections.Generic;
using DA_Assets.Shared.Extensions;

namespace DA_Assets.DAG
{
    [AddComponentMenu("UI/Effects/D.A. Gradient")]
    public class DAGradient : BaseMeshEffect
    {
        private RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();
            rectTransform = GetComponent<RectTransform>();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            rectTransform = GetComponent<RectTransform>();
        }
#endif

        [SerializeField] DAColorBlendMode blendMode = DAColorBlendMode.Overlay;
        [SerializeProperty(nameof(blendMode))]
        public DAColorBlendMode BlendMode
        {
            get => blendMode;
            set
            {
                blendMode = value;
                graphic?.SetVerticesDirty();
            }
        }

        [SerializeField, Range(0, 1)] float intensity = 1f;
        [SerializeProperty(nameof(intensity))]
        public float Intensity
        {
            get => intensity;
            set
            {
                intensity = Mathf.Clamp01(value);
                graphic?.SetVerticesDirty();
            }
        }

        [SerializeField, Range(0, 360)] float angle = 0f;
        [SerializeProperty(nameof(angle))]
        public float Angle
        {
            get => angle;
            set
            {
                angle = value.NormalizeAngle360();
                graphic?.SetVerticesDirty();
            }
        }

        [SerializeField] Gradient gradient = new Gradient();
        [SerializeProperty(nameof(gradient))]
        public Gradient Gradient
        {
            get => gradient;
            set
            {
                gradient = value;
                graphic?.SetVerticesDirty();
            }
        }

        public override void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled == false)
                return;

            if (vertexHelper == null)
                return;

            UIVertex uiVertex = new UIVertex();

            Vector2 rectSize = rectTransform.rect.size;
            Vector2 rectCenter = rectTransform.rect.center;

            AdjustGradientPart(vertexHelper);

            for (int i = 0; i < vertexHelper.currentVertCount; i++)
            {
                // Populates uiVertex with data from the current vertex.
                vertexHelper.PopulateUIVertex(ref uiVertex, i);

                // Converts the angle from degrees to radians and calculates a scale factor.
                float a = Mathf.Deg2Rad * angle;
                float scale = Mathf.Abs(Mathf.Sin(a)) + Mathf.Abs(Mathf.Cos(a));
                // Calculates the relative position of the vertex within the RectTransform.
                float x = (uiVertex.position.x - rectCenter.x) / rectSize.x;
                float y = (uiVertex.position.y - rectCenter.y) / rectSize.y;

                Vector2 relativePosition = new Vector2(x, y);
                // Rotates the position based on the specified angle.
                Vector2 rotatedPosition = relativePosition.Rotate(angle);
                // Scales the position to adjust for the angle's effect on perceived size.
                Vector2 scaledPosition = rotatedPosition / scale;
                // Centers the position within a 0 to 1 range.
                Vector2 centeredPosition = new Vector2(0.5f, 0.5f);
                // Calculates the final position within the gradient.
                Vector2 gradientPosition = scaledPosition + centeredPosition;
                // Evaluates the color from the gradient based on the Y position.
                Color gradientColor = gradient.Evaluate(gradientPosition.y);
                // Blends the original vertex color with the calculated gradient color.
                uiVertex.color = DAColorBlender.Blend(uiVertex.color, gradientColor, blendMode, intensity);

                // Applies the modified color back to the vertex.
                vertexHelper.SetUIVertex(uiVertex, i);
            }
        }

        // Adjusts the gradient scale and positioning.
        private void AdjustGradientPart(VertexHelper vertexHelper)
        {
            List<UIVertex> originalVertices = new List<UIVertex>();
            // Extracts vertices from the mesh.
            vertexHelper.GetUIVertexStream(originalVertices);
            // Clears the current vertex data to prepare for modifications.
            vertexHelper.Clear();

            List<UIVertex> modifiedVertices = new List<UIVertex>();
            // Calculates the direction for the gradient based on the angle, adjusted for the RectTransform's size.
            Vector2 direction = Vector2.up.Rotate(-angle);
            direction.Scale(Vector2.one / rectTransform.rect.size);
            // Calculates a perpendicular direction to the gradient direction.
            Vector2 perpendicularDirection = direction.Rotate(90);

            // Processes each unique alpha and color key position in the gradient.
            gradient.alphaKeys
                .Select(alphaKey => alphaKey.time)
                .Union(gradient.colorKeys.Select(colorKey => colorKey.time))
                .ToList()
                .ForEach(pos =>
                {
                    modifiedVertices.Clear();
                    // Interpolates a slice position for the current key position.
                    Vector2 p = InterpolateSlice(pos);

                    // For each group of three vertices (forming a triangle), adjusts their positions.
                    Enumerable.Range(0, originalVertices.Count / 3)
                        .ToList()
                        .ForEach(i =>
                            GetVertices(originalVertices, i * 3, modifiedVertices, perpendicularDirection, p));

                    originalVertices.Clear();
                    originalVertices.AddRange(modifiedVertices);
                });

            // Reconstructs the mesh with the modified vertices.
            vertexHelper.AddUIVertexTriangleStream(originalVertices);
        }

        // Calculates the position for a gradient slice based on an interpolation factor.
        private Vector2 InterpolateSlice(float interpolationFactor)
        {
            Vector2 rectSize = rectTransform.rect.size;

            // Calculates a direction for the slice based on the angle and adjusts for the RectTransform's size.
            Vector2 rotatedDirection = Vector2.up.Rotate(-angle);
            rotatedDirection.Scale(Vector2.one / rectSize);

            // Determines the starting and ending multipliers for the slice based on the angle.
            Vector2 sliceStartMultiplier = angle % 180 < 90 ? Vector2.down + Vector2.left : Vector2.up + Vector2.left;
            Vector2 sliceEndMultiplier = angle % 180 < 90 ? Vector2.up + Vector2.right : Vector2.down + Vector2.right;

            // Projects the start and end points of the slice within the RectTransform.
            Vector3 sliceStartPoint = Vector3.Project(rectSize * sliceStartMultiplier * 0.5f, rotatedDirection);
            Vector3 sliceEndPoint = Vector3.Project(rectSize * sliceEndMultiplier * 0.5f, rotatedDirection);

            // Interpolates between the start and end points based on the factor, adjusting for the center of the RectTransform.
            Vector2 result = Vector2.Lerp(angle % 360 >= 180 ? sliceEndPoint : sliceStartPoint, angle % 360 >= 180 ? sliceStartPoint : sliceEndPoint, interpolationFactor) + rectTransform.rect.center;
            return result;
        }

        // Adjusts vertices of a triangle (defined by a start index) for gradient application based on a reference direction and point.
        private void GetVertices(List<UIVertex> triangleVertices, int startIndex, List<UIVertex> outputVertices, Vector2 direction, Vector2 referencePoint)
        {
            // Deconstructs the list of triangle vertices into individual vertices A, B, and C.
            var vertices = (
                A: triangleVertices[startIndex],
                B: triangleVertices[startIndex + 1],
                C: triangleVertices[startIndex + 2]
            );

            // Calculates the intersection factors for each edge of the triangle relative to the gradient direction and reference point.
            Vector3 intersectionFactors = new Vector3(
                vertices.A.position.CalculateIntersectionFactor(vertices.B.position, referencePoint, direction),
                vertices.B.position.CalculateIntersectionFactor(vertices.C.position, referencePoint, direction),
                vertices.C.position.CalculateIntersectionFactor(vertices.A.position, referencePoint, direction)
            );

            // Checks if the intersection factor for edge AB falls within the valid range (0 to 1, inclusive).
            if (intersectionFactors.x.IsBetween())
            {
                // Creates a new vertex between A and B based on the intersection factor.
                UIVertex newVertexAB = vertices.A.Lerp(vertices.B, intersectionFactors.x);
                // Further checks if the intersection factor for edge BC is also within the valid range.
                if (intersectionFactors.y.IsBetween())
                {
                    // Creates a new vertex between B and C.
                    UIVertex newVertexBC = vertices.B.Lerp(vertices.C, intersectionFactors.y);
                    // Adds vertices to 'outputVertices' to form new triangles, incorporating the new vertices to maintain mesh integrity.
                    outputVertices.AddRange(new[]
                    {
                        vertices.A,
                        newVertexAB,
                        vertices.C,
                        newVertexAB,
                        newVertexBC,
                        vertices.C,
                        newVertexAB,
                        vertices.B,
                        newVertexBC
                    });
                }
                else
                {
                    // If only AB intersects, creates a new vertex between C and A.
                    UIVertex newVertexCA = vertices.C.Lerp(vertices.A, intersectionFactors.z);
                    // Adjusts the triangle(s) to accommodate the new vertices for gradient effect.
                    outputVertices.AddRange(new[]
                    {
                        vertices.C,
                        newVertexCA,
                        vertices.B,
                        newVertexCA,
                        newVertexAB,
                        vertices.B,
                        newVertexCA,
                        vertices.A,
                        newVertexAB
                    });
                }
            }
            // If the edge AB doesn't intersect but BC does.
            else if (intersectionFactors.y.IsBetween())
            {
                // Creates new vertices for the intersecting edges BC and CA.
                UIVertex newVertexBC = vertices.B.Lerp(vertices.C, intersectionFactors.y);
                UIVertex newVertexCA = vertices.C.Lerp(vertices.A, intersectionFactors.z);

                // Reconfigures the triangle(s) based on the new vertices.
                outputVertices.AddRange(new[]
                {
                    vertices.B,
                    newVertexBC,
                    vertices.A,
                    newVertexBC,
                    newVertexCA,
                    vertices.A,
                    newVertexBC,
                    vertices.C,
                    newVertexCA
                });
            }
            // If no intersections occur, adds the original triangle vertices to 'outputVertices'.
            else
            {
                outputVertices.AddRange(new[]
                {
                    vertices.A,
                    vertices.B,
                    vertices.C
                });
            }
        }
    }

    internal static class DAGradientExtensions
    {            
        internal static bool IsBetween(this float intersectionFactor) => intersectionFactor <= 1 && intersectionFactor >= 0;
    }
}