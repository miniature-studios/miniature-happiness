/*
The MIT License (MIT)

Copyright (c) 2015 Mikola Lysenko

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

namespace DA_Assets.SVGMeshUnity.Internals.Cdt2d
{
    public class Triangulation
    {
        // https://github.com/mikolalysenko/cdt2d
        
        public bool Delaunay = true;
        public bool Interior = true;
        public bool Exterior = true;
        public bool Infinity = false;

        public WorkBufferPool WorkBufferPool;

        private MonotoneTriangulation MonotoneTriangulation = new MonotoneTriangulation();
        private DelaunayRefine DelaunayRefine = new DelaunayRefine();
        private Filter Filter = new Filter();

        public void BuildTriangles(MeshData data)
        {
            //Handle trivial case
            if ((!Interior && !Exterior) || data.Vertices.Count == 0)
            {
                return;
            }

            //Construct initial triangulation
            MonotoneTriangulation.WorkBufferPool = WorkBufferPool;
            MonotoneTriangulation.BuildTriangles(data);

            //If delaunay refinement needed, then improve quality by edge flipping
            if (Delaunay || Interior != Exterior || Infinity)
            {
                //Index all of the cells to support fast neighborhood queries
                var triangles = new Triangles(data);

                //Run edge flipping
                if (Delaunay)
                {
                    DelaunayRefine.WorkBufferPool = WorkBufferPool;
                    DelaunayRefine.RefineTriangles(triangles);
                }

                Filter.WorkBufferPool = WorkBufferPool;
                Filter.Infinity = Infinity;

                //Filter points
                if (!Exterior)
                {
                    Filter.Target = -1;
                    Filter.Do(triangles, data.Triangles);
                    return;
                }
                if (!Interior)
                {
                    Filter.Target = 1;
                    Filter.Do(triangles, data.Triangles);
                    return;
                }
                if (Infinity)
                {
                    Filter.Target = 0;
                    Filter.Do(triangles, data.Triangles);
                    return;
                }
                
                triangles.Fill(data.Triangles);

            }
        }

    }
}