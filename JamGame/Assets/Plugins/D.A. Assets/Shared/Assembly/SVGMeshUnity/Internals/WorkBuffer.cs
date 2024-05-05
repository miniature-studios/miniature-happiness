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
using System.Linq;
using UnityEngine;

namespace DA_Assets.SVGMeshUnity.Internals
{
    public class WorkBuffer<T>
    {
        public WorkBuffer(int size = 32)
        {
            GrowSize = size;
            PrivateData = new T[size];
        }
        
        public T[] Data
        {
            get { return PrivateData; }
        }
        public int UsedSize
        {
            get { return PrivateUsedSize; }
        }

        public Func<T> NewForClass;

        private int GrowSize;
        private T[] PrivateData;
        private int PrivateUsedSize;

        private void Grow(int size)
        {
            var newPrivateData = new T[size];
            PrivateData.CopyTo(newPrivateData, 0);
            PrivateData = newPrivateData;
        }

        private void GrowIfNeeded()
        {
            if (PrivateData.Length == PrivateUsedSize)
            {
                Grow(PrivateData.Length + GrowSize);
            }
        }

        public void Extend(int size)
        {
            if (PrivateData.Length < size)
            {
                Grow(size);
            }
        }

        public void Fill(ref T val, int n)
        {
            if (PrivateData.Length < n)
            {
                Grow(n);
            }

            for (var i = 0; i < n; ++i)
            {
                PrivateData[i] = val;
            }

            PrivateUsedSize = n;
        }

        public void Push(ref T val)
        {
            GrowIfNeeded();
            PrivateData[PrivateUsedSize] = val;
            ++PrivateUsedSize;
        }

        public T Push()
        {
            GrowIfNeeded();

            var val = PrivateData[PrivateUsedSize];

            if (val == null)
            {
                val = NewForClass();
                PrivateData[PrivateUsedSize] = val;
            }

            ++PrivateUsedSize;

            return val;
        }

        public T Pop()
        {
            var val = PrivateData[PrivateUsedSize - 1];
            --PrivateUsedSize;
            return val;
        }

        public T Insert(int index)
        {
            if (index == PrivateUsedSize)
            {
                return Push();
            }

            GrowIfNeeded();

            var val = PrivateData[PrivateUsedSize];

            for (var i = PrivateUsedSize - 1; i >= index; --i)
            {
                PrivateData[i + 1] = PrivateData[i];
            }

            if (val == null)
            {
                val = NewForClass();
            }

            PrivateData[index] = val;

            ++PrivateUsedSize;

            return val;
        }

        public void RemoveAt(int index)
        {
            var old = PrivateData[index];
            
            for (var i = index; i < PrivateUsedSize - 1; ++i)
            {
                PrivateData[i] = PrivateData[i + 1];
            }

            PrivateData[PrivateUsedSize - 1] = old;

            --PrivateUsedSize;
        }

        public static void Sort<G>(WorkBuffer<G> buf) where G : IComparable<G>
        {
            Internals.Sort<G>.QuickSort(buf.PrivateData, 0, buf.PrivateUsedSize - 1);
        }

        public void RemoveLast(int n)
        {
            PrivateUsedSize -= n;
        }

        public void Clear()
        {
            PrivateUsedSize = 0;
        }

        public void Dump()
        {
            Debug.Log(PrivateData.Take(PrivateUsedSize).Aggregate("", (_, s) => _ + s.ToString() + "\n"));
        }

        public void DumpHash()
        {
            Debug.LogFormat("{0}{1}", PrivateUsedSize, PrivateData.Select(_ => string.Format("{0:x}",_ != null ? _.GetHashCode() : 0)).Aggregate("", (_, s) => _ + ", " + s));
        }
    }
}