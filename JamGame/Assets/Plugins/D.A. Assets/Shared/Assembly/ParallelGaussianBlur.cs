// The MIT License
// Copyright © 2020 Roger Cabo Ashauer
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,     // sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:     // The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.     // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,     // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,     // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// https://de.wikipedia.org/wiki/MIT-Lizenz
// This solution is based on Fast image convolutions by Wojciech Jarosz.
// http://elynxsdk.free.fr/ext-docs/Blur/Fast_box_blur.pdf
// And Ivan Kutskir
// http://blog.ivank.net/fastest-gaussian-blur.html
// And Mike Demyl
// https://github.com/mdymel  // https://github.com/mdymel/superfastblur

using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class ParallelGaussianBlur
    {
        public ParallelGaussianBlur(Texture2D tex, float step)
        {
            this._tex2D = tex;
            this._radial = step;
        }

        private readonly ParallelOptions _pOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 8
        };

        private NativeArray<Color32> _RawTextureData = new NativeArray<Color32>();

        private Texture2D _tex2D;
        private float _radial;
        private int _width;
        private int _height;
        private int[] m_red;
        private int[] m_green;
        private int[] m_blue;
        private int[] m_alpha;

        public void GaussianBlur()
        {
            _RawTextureData = _tex2D.GetRawTextureData<Color32>();
            _width = _tex2D.width;
            _height = _tex2D.height;
            _width = _tex2D.width;
            _height = _tex2D.height;
            m_red = new int[_width * _height];
            m_green = new int[_width * _height];
            m_blue = new int[_width * _height];
            m_alpha = new int[_width * _height];

            Parallel.For(0, _width * _height, _pOptions, i =>
            {
                m_red[i] = _RawTextureData[i].r;
                m_green[i] = _RawTextureData[i].g;
                m_blue[i] = _RawTextureData[i].b;
                m_alpha[i] = _RawTextureData[i].a;
            });

            int[] newAlpha = new int[_width * _height];
            int[] newRed = new int[_width * _height];
            int[] newGreen = new int[_width * _height];
            int[] newBlue = new int[_width * _height];

            Parallel.Invoke(
                () => gaussBlur_4(m_alpha, newAlpha, _radial),
                () => gaussBlur_4(m_red, newRed, _radial),
                () => gaussBlur_4(m_green, newGreen, _radial),
                () => gaussBlur_4(m_blue, newBlue, _radial));

            Parallel.For(0, _width * _height, _pOptions, i =>
            {
                if (newAlpha[i] > 255) newAlpha[i] = 255;
                if (newRed[i] > 255) newRed[i] = 255;
                if (newGreen[i] > 255) newGreen[i] = 255;
                if (newBlue[i] > 255) newBlue[i] = 255;
                if (newAlpha[i] < 0) newAlpha[i] = 0;
                if (newRed[i] < 0) newRed[i] = 0;
                if (newGreen[i] < 0) newGreen[i] = 0;
                if (newBlue[i] < 0) newBlue[i] = 0;
                _RawTextureData[i] = new Color32((byte)newRed[i], (byte)newGreen[i], (byte)newBlue[i], (byte)newAlpha[i]);
            });

            _tex2D.Apply();
        }

        private int[] boxesForGauss(float sigma, int n)
        {
            float wIdeal = Mathf.Sqrt((12 * sigma * sigma / n) + 1);
            int wl = (int)Mathf.Floor(wIdeal);

            if (wl % 2 == 0)
            {
                wl--;
            }

            int wu = wl + 2;
            float mIdeal = (float)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            float m = Mathf.Round(mIdeal);
            List<int> sizes = new List<int>();

            for (int i = 0; i < n; i++)
            {
                sizes.Add(i < m ? wl : wu);
            }

            return sizes.ToArray();
        }

        private void gaussBlur_4(int[] colorChannel, int[] destChannel, float r)
        {
            int[] bxs = boxesForGauss(r, 3);

            boxBlur_4(colorChannel, destChannel, _width, _height, (bxs[0] - 1) / 2);
            boxBlur_4(destChannel, colorChannel, _width, _height, (bxs[1] - 1) / 2);
            boxBlur_4(colorChannel, destChannel, _width, _height, (bxs[2] - 1) / 2);
        }

        private void boxBlur_4(int[] colorChannel, int[] destChannel, int w, int h, int r)
        {
            for (int i = 0; i < colorChannel.Length; i++)
            {
                destChannel[i] = colorChannel[i];
            }

            boxBlurH_4(destChannel, colorChannel, w, h, r);
            boxBlurT_4(colorChannel, destChannel, w, h, r);
        }

        private void boxBlurH_4(int[] colorChannel, int[] dest, int w, int h, int radial)
        {
            float iar = (float)1 / (radial + radial + 1);

            Parallel.For(0, h, _pOptions, i =>
            {
                int ti = i * w;
                int li = ti;
                int ri = ti + radial;
                int fv = colorChannel[ti];
                int lv = colorChannel[ti + w - 1];
                int val = (radial + 1) * fv;

                for (int j = 0; j < radial; j++)
                {
                    val += colorChannel[ti + j];
                }

                for (int j = 0; j <= radial; j++)
                {
                    val += colorChannel[ri++] - fv;
                    dest[ti++] = (int)Mathf.Round(val * iar);
                }

                for (int j = radial + 1; j < w - radial; j++)
                {
                    val += colorChannel[ri++] - dest[li++];
                    dest[ti++] = (int)Mathf.Round(val * iar);
                }

                for (int j = w - radial; j < w; j++)
                {
                    val += lv - colorChannel[li++];
                    dest[ti++] = (int)Mathf.Round(val * iar);
                }
            });
        }

        private void boxBlurT_4(int[] colorChannel, int[] dest, int w, int h, int r)
        {
            float iar = (float)1 / (r + r + 1);

            Parallel.For(0, w, _pOptions, i =>
            {
                int ti = i;
                int li = ti;
                int ri = ti + r * w;
                int fv = colorChannel[ti];
                int lv = colorChannel[ti + w * (h - 1)];
                int val = (r + 1) * fv;

                for (int j = 0; j < r; j++)
                {
                    val += colorChannel[ti + j * w];
                }

                for (int j = 0; j <= r; j++)
                {
                    val += colorChannel[ri] - fv;
                    dest[ti] = (int)Mathf.Round(val * iar);
                    ri += w;
                    ti += w;
                }

                for (int j = r + 1; j < h - r; j++)
                {
                    val += colorChannel[ri] - colorChannel[li];
                    dest[ti] = (int)Mathf.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }

                for (int j = h - r; j < h; j++)
                {
                    val += lv - colorChannel[li];
                    dest[ti] = (int)Mathf.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }
    }
}
