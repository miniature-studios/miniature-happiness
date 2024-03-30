using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace DA_Assets.Shared.Extensions
{
    public struct TransformProps
    {
        public Vector3 position;
        public Quaternion rotation;
        public Transform parent;
    }
    /// <summary>
    /// D.A. Unity Code Helpers [09.2023]
    /// </summary>
    public static class OtherExtensions
    {
        public static bool IsDefault<T>(this T color)
        {
            if (color == null)
            {
                return true;
            }

            return color.Equals(default(T));
        }

        public static int NormalizeAngleToSize(this float val, float width, float height)
        {
            if (val <= 0)
            {
                val = 0;
                return (int)val;
            }

            float resW = width / val;
            float resH = height / val;

            if (resW < 2 || resH < 2)
            {
                int min = Mathf.Min((int)(width / 2), (int)(height / 2));
                val = min;
            }

            if (val <= 0)
                val = 0;

            return (int)val;
        }

        public static Color Lerp(this Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            var result = new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
            return result;
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }
        public static string CreateShortGuid(this string value, out string result)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = System.Guid.NewGuid().ToString().Split('-')[0];

            result = value;
            return value;
        }

        public static T CopyClass<T>(this T source)
        {
            string json = JsonUtility.ToJson(source);
            T copiedObject = JsonUtility.FromJson<T>(json);
            return copiedObject;
        }
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static void SetTransformProps(this TransformProps transformProps, Transform source)
        {
            transformProps.position = source.position;
            transformProps.rotation = source.rotation;
            transformProps.parent = source.parent;
        }
        public static void SetTransform(this Transform target, TransformProps transformProps)
        {
            target.transform.position = transformProps.position;
            target.transform.rotation = transformProps.rotation;
            target.transform.parent = transformProps.parent;
        }
        /// <summary>
        /// https://stackoverflow.com/a/33784596/8265642
        /// </summary>
        public static string GetNumbers(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return new string(text.Where(p => char.IsDigit(p)).ToArray());
        }
        public static byte[] GetSHA1Hash(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] result = sha1.ComputeHash(bytes);
                return result;
            }
        }
        public static string ToHEX(this byte[] bytes)
        {
            StringBuilder buffer = new StringBuilder(bytes.Length * 2);
            foreach (var byt in bytes)
            {
                buffer.Append(byt.ToString("X2"));
            }

            return buffer.ToString();
        }
        public static System.Numerics.BigInteger ToBigInteger(this byte[] data)
        {
            return new System.Numerics.BigInteger(data);
        }

#if UNITY_EDITOR
        /// <summary>
        /// https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/
        /// </summary>
        public static bool GetTextureSize(this UnityEditor.TextureImporter importer, out int width, out int height)
        {
            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(UnityEditor.TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];

                return true;
            }

            height = width = 0;
            return false;
        }

        /// <summary>
        /// Sets the maximum size of the texture based on its width and height.
        /// <para><see href="https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/"/></para>
        /// </summary>
        public static void SetMaxTextureSize(this UnityEditor.TextureImporter importer, int width, int height)
        {
            int[] maxTextureSizeValues = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

            int max = Mathf.Max(width, height);

            int defsize = 1024; //Default size

            for (int i = 0; i < maxTextureSizeValues.Length; i++)
            {
                if (maxTextureSizeValues[i] >= max)
                {
                    defsize = maxTextureSizeValues[i];
                    break;
                }
            }

            importer.maxTextureSize = defsize;
        }
#endif

        public static float ToFloat(this float? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        public static bool ToBoolNullTrue(this bool? value)
        {
            if (value == null)
            {
                return true;
            }

            return value.Value;
        }

        public static bool ToBoolNullFalse(this bool? value)
        {
            if (value == null)
            {
                return false;
            }

            return value.Value;
        }

        public static string ToLower(this Enum value)
        {
            return value.ToString().ToLower();
        }

        public static string ToUpper(this Enum value)
        {
            return value.ToString().ToUpper();
        }

        /// <summary>
        /// <para>Example: "#ff000099".ToColor() red with alpha ~50%</para>
        /// <para>Example: "ffffffff".ToColor() white with alpha 100%</para>
        /// <para>Example: "00ff00".ToColor() green with alpha 100%</para>
        /// <para>Example: "0000ff00".ToColor() blue with alpha 0%</para>
        /// <para><see href="https://github.com/smkplus/KamaliDebug"/></para>
        /// </summary>
        public static Color ToColor(this string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                color = color.Substring(1); // strip #
            }

            if (color.Length == 6)
            {
                color += "FF"; // add alpha if missing
            }

            uint hex = Convert.ToUInt32(color, 16);
            float r = ((hex & 0xff000000) >> 0x18) / 255f;
            float g = ((hex & 0xff0000) >> 0x10) / 255f;
            float b = ((hex & 0xff00) >> 8) / 255f;
            float a = (hex & 0xff) / 255f;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// <para><see href="https://forum.unity.com/threads/easy-text-format-your-debug-logs-rich-text-format.906464/"/></para>
        /// </summary>
        public static string TextBold(this string str) => "<b>" + str + "</b>";
        public static string TextColor(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
        public static string TextItalic(this string str) => "<i>" + str + "</i>";
        public static string TextSize(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    }
}