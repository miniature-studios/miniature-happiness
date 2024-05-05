using UnityEngine;

namespace DA_Assets.Shared.Extensions
{
    public static class TextureExtensions
    {
        public static Texture2D Merge(this Texture2D largeTexture, Texture2D smallTexture)
        {
            //Create a new texture with the size of the large texture
            Texture2D overlayTexture = new Texture2D(largeTexture.width, largeTexture.height, TextureFormat.RGBA32, false);

            //Copy pixels of a large texture to a new texture
            Color[] pixels = largeTexture.GetPixels();
            overlayTexture.SetPixels(pixels);

            //Determine the coordinates to center the small texture
            int startX = (largeTexture.width - smallTexture.width) / 2;
            int startY = (largeTexture.height - smallTexture.height) / 2;

            //Overlay pixels of a small texture onto a large texture
            Color[] overlayPixels = smallTexture.GetPixels();
            for (int x = 0; x < smallTexture.width; x++)
            {
                for (int y = 0; y < smallTexture.height; y++)
                {
                    int targetX = startX + x;
                    int targetY = startY + y;

                    Color overlayPixel = overlayPixels[x + y * smallTexture.width];

                    //Applying the alpha channel of a small texture to the pixels of a large texture
                    Color targetPixel = overlayTexture.GetPixel(targetX, targetY);
                    Color finalPixel = Color.Lerp(targetPixel, overlayPixel, overlayPixel.a);
                    overlayTexture.SetPixel(targetX, targetY, finalPixel);
                }
            }

            //Applying changes to the texture
            overlayTexture.Apply();

            return overlayTexture;
        }

        public static void Colorize(this Texture2D texture, Color32 color)
        {
            Color32[] pixels = texture.GetPixels32();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(color.r, color.g, color.b, pixels[i].a);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }

        public static void Blur(this Texture2D texture2D, float step = 0.5f)
        {
            ParallelGaussianBlur bl = new ParallelGaussianBlur(texture2D, step);
            bl.GaussianBlur();
        }


        public static void Resize(this Texture2D texture2D, Vector2Int targetSize, int depth, FilterMode filterMode, RenderTextureFormat rtFormat)
        {
            RenderTexture rt = RenderTexture.GetTemporary(targetSize.x, targetSize.y, depth, rtFormat, RenderTextureReadWrite.Default);

            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);

#if UNITY_2021_2_OR_NEWER
            texture2D.Reinitialize(targetSize.x, targetSize.y, texture2D.format, false);
#else
            texture2D.Resize(targetSize.x, targetSize.y, texture2D.format, false);
#endif
            texture2D.filterMode = filterMode;
            texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetSize.x, targetSize.y), 0, 0);
            texture2D.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }
    }
}
