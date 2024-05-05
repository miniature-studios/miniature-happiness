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

namespace DA_Assets.SVGMeshUnity.Internals
{
    public static class Sort<T> where T : IComparable<T>
    {
        public static void QuickSort(T[] elements, int left, int right)
        {
            var i = left;
            var j = right;
            var pivot = elements[(left + right) >> 1];
 
            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    ++i;
                }
 
                while (elements[j].CompareTo(pivot) > 0)
                {
                    --j;
                }
 
                if (i <= j)
                {
                    // Swap
                    var tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;
 
                    i++;
                    j--;
                }
            }
 
            // Recursive calls
            if (left < j)
            {
                QuickSort(elements, left, j);
            }
 
            if (i < right)
            {
                QuickSort(elements, i, right);
            }
        }
    }
}