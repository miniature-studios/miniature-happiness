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


using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.SVGMeshUnity.Internals.Cdt2d
{
    public class Triangles
    {
        public Triangles(MeshData data)
        {
            PrivateVertices = data.Vertices;
            CreateStars(data.Vertices.Count);
            CreateEdges(data.Edges);
            CreateTriangles(data.Triangles);
        }

        public List<Vector3> Vertices
        {
            get { return PrivateVertices; }
        }

        public WorkBuffer<Int2>[] Stars
        {
            get { return PrivateStars; }
        }

        private List<Vector3> PrivateVertices;
        private WorkBuffer<Int2>[] PrivateStars;
        private Int2[] Edges;

        private void CreateStars(int n)
        {
            var stars = new WorkBuffer<Int2>[n];
            for (var i = 0; i < n; ++i)
            {
                stars[i] = new WorkBuffer<Int2>(16);
            }
            PrivateStars = stars;
        }

        private void CreateEdges(List<Int2> source)
        {
            var l = source.Count;
            var edges = new Int2[l];
            for (var i = 0; i < l; ++i)
            {
                var edge = source[i];
                if (edge.y < edge.x)
                {
                    var x = edge.x;
                    edge.x = edge.y;
                    edge.y = x;
                }
                edges[i] = edge;
            }
            Sort<Int2>.QuickSort(edges, 0, edges.Length - 1);
            Edges = edges;
        }
 
        private void CreateTriangles(List<int> source)
        {
            var l = source.Count;
            for (var i = 0; i < l; i += 3)
            {
                AddTriangle(source[i + 0], source[i + 1], source[i + 2]);
            }
        }
        
        public void AddTriangle(int i, int j, int k)
        {
            var jk = new Int2(j, k);
            var ki = new Int2(k, i);
            var ij = new Int2(i, j);
            PrivateStars[i].Push(ref jk);
            PrivateStars[j].Push(ref ki);
            PrivateStars[k].Push(ref ij);
        }
        
        public void RemoveTriangle(int i, int j, int k)
        {
            RemovePair(PrivateStars[i], j, k);
            RemovePair(PrivateStars[j], k, i);
            RemovePair(PrivateStars[k], i, j);
        }

        private void RemovePair(WorkBuffer<Int2> list, int j, int k)
        {
            var n = list.UsedSize;
            var data = list.Data;
            for (var i = 0; i < n; ++i)
            {
                var s = data[i];
                if (s.x == j && s.y == k)
                {
                    data[i] = data[n - 1];
                    list.RemoveLast(1);
                    return;
                }
            }
        }
        
        public int Opposite(int j, int i)
        {
            var list = PrivateStars[i];
            var n = list.UsedSize;
            var data = list.Data;
            for (var k = 0; k < n; ++k)
            {
                if (data[k].y == j)
                {
                    return data[k].x;
                }
            }
            return -1;
        }
        
        public void Flip(int i, int j)
        {
            var a = Opposite(i, j);
            var b = Opposite(j, i);
            RemoveTriangle(i, j, a);
            RemoveTriangle(j, i, b);
            AddTriangle(i, b, a);
            AddTriangle(j, a, b);
        }

        public bool IsConstraint(int i, int j)
        {
            var e = new Int2(Mathf.Min(i, j), Mathf.Max(i, j));
            return BinarySearch.EQ(Edges, e, 0, Edges.Length) >= 0;
        }

        public void Fill(List<int> triangles)
        {
            var n = PrivateStars.Length;

            if (triangles.Capacity < n)
            {
                triangles.Capacity = n;
            }
            triangles.Clear();
            
            for(var i = 0; i < n; ++i)
            {
                var list = PrivateStars[i];
                var data = list.Data;
                var m = list.UsedSize;
                for(var j = 0; j < m; ++j)
                {
                    var s = data[j];
                    if(i < Mathf.Min(s.x, s.y))
                    {
                        triangles.Add(i);
                        triangles.Add(s.x);
                        triangles.Add(s.y);
                    }
                }
            }
        }

        public void Fill(WorkBuffer<Int3> triangles)
        {
            var n = PrivateStars.Length;

            triangles.Extend(n);
            triangles.Clear();
            
            for(var i = 0; i < n; ++i)
            {
                var list = PrivateStars[i];
                var data = list.Data;
                var m = list.UsedSize;
                for(var j = 0; j < m; ++j)
                {
                    var s = data[j];
                    if(i < Mathf.Min(s.x, s.y))
                    {
                        var v = new Int3(i, s.x, s.y);
                        triangles.Push(ref v);
                    }
                }
            }
        }
    }
}