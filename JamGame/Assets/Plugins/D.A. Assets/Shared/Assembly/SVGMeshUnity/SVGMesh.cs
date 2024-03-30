/*
MIT License

Copyright (c) 2018 Yoshihiro Shindo

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
SOFTWARE.
*/

using DA_Assets.SVGMeshUnity.Internals;
using DA_Assets.SVGMeshUnity.Internals.Cdt2d;
using System;
using UnityEngine;

namespace DA_Assets.SVGMeshUnity
{
    [Serializable]
    public class SVGMesh
    {
        [SerializeField] float scale;
        [SerializeField] bool delaunay;
        [SerializeField] bool interior;
        [SerializeField] bool exterior;
        [SerializeField] bool infinity;

        private static WorkBufferPool workBufferPool = new WorkBufferPool();
        private MeshData meshData = new MeshData();
        private BezierToVertex bezierToVertex;
        private Triangulation triangulation;
        private Mesh mesh;

        public void Init(
            float scale,
            bool delaunay = false,
            bool interior = true,
            bool exterior = false,
            bool infinity = false)
        {
            this.scale = scale;
            this.delaunay = delaunay;
            this.interior = interior;
            this.exterior = exterior;
            this.infinity = infinity;
            bezierToVertex = new BezierToVertex();
            bezierToVertex.WorkBufferPool = workBufferPool;
            triangulation = new Triangulation();
            triangulation.WorkBufferPool = workBufferPool;
        }

        public void Fill(SVGData svg, MeshFilter filter)
        {
            meshData.Clear();
            // convert curves into discrete points
            bezierToVertex.Scale = scale;
            bezierToVertex.GetContours(svg, meshData);
            // triangulate mesh
            triangulation.Delaunay = delaunay;
            triangulation.Interior = interior;
            triangulation.Exterior = exterior;
            triangulation.Infinity = infinity;
            triangulation.BuildTriangles(meshData);
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.MarkDynamic();
            }
            meshData.MakeUnityFriendly();
            meshData.Upload(mesh);
            if (filter != null)
            {
                filter.sharedMesh = mesh;
            }
        }
    }
}