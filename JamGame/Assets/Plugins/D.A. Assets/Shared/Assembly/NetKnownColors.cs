using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class NetKnownColors
    {
        private static Dictionary<NetKnownColor, Color32> knownColors;
        public static Dictionary<NetKnownColor, Color32> KnownColors => knownColors;

        static NetKnownColors()
        {
            knownColors = new Dictionary<NetKnownColor, Color32>
            {
                { NetKnownColor.AliceBlue, AliceBlue },
                { NetKnownColor.AntiqueWhite, AntiqueWhite },
                { NetKnownColor.Aqua, Aqua },
                { NetKnownColor.Aquamarine, Aquamarine },
                { NetKnownColor.Azure, Azure },
                { NetKnownColor.Beige, Beige },
                { NetKnownColor.Bisque, Bisque },
                { NetKnownColor.Black, Black },
                { NetKnownColor.BlanchedAlmond, BlanchedAlmond },
                { NetKnownColor.Blue, Blue },
                { NetKnownColor.BlueViolet, BlueViolet },
                { NetKnownColor.Brown, Brown },
                { NetKnownColor.BurlyWood, BurlyWood },
                { NetKnownColor.CadetBlue, CadetBlue },
                { NetKnownColor.Chartreuse, Chartreuse },
                { NetKnownColor.Chocolate, Chocolate },
                { NetKnownColor.Coral, Coral },
                { NetKnownColor.CornflowerBlue, CornflowerBlue },
                { NetKnownColor.Cornsilk, Cornsilk },
                { NetKnownColor.Crimson, Crimson },
                { NetKnownColor.Cyan, Cyan },
                { NetKnownColor.DarkBlue, DarkBlue },
                { NetKnownColor.DarkCyan, DarkCyan },
                { NetKnownColor.DarkGoldenrod, DarkGoldenrod },
                { NetKnownColor.DarkGray, DarkGray },
                { NetKnownColor.DarkGreen, DarkGreen },
                { NetKnownColor.DarkKhaki, DarkKhaki },
                { NetKnownColor.DarkMagenta, DarkMagenta },
                { NetKnownColor.DarkOliveGreen, DarkOliveGreen },
                { NetKnownColor.DarkOrange, DarkOrange },
                { NetKnownColor.DarkOrchid, DarkOrchid },
                { NetKnownColor.DarkRed, DarkRed },
                { NetKnownColor.DarkSalmon, DarkSalmon },
                { NetKnownColor.DarkSeaGreen, DarkSeaGreen },
                { NetKnownColor.DarkSlateBlue, DarkSlateBlue },
                { NetKnownColor.DarkSlateGray, DarkSlateGray },
                { NetKnownColor.DarkTurquoise, DarkTurquoise },
                { NetKnownColor.DarkViolet, DarkViolet },
                { NetKnownColor.DeepPink, DeepPink },
                { NetKnownColor.DeepSkyBlue, DeepSkyBlue },
                { NetKnownColor.DimGray, DimGray },
                { NetKnownColor.DodgerBlue, DodgerBlue },
                { NetKnownColor.Firebrick, Firebrick },
                { NetKnownColor.FloralWhite, FloralWhite },
                { NetKnownColor.ForestGreen, ForestGreen },
                { NetKnownColor.Fuchsia, Fuchsia },
                { NetKnownColor.Gainsboro, Gainsboro },
                { NetKnownColor.GhostWhite, GhostWhite },
                { NetKnownColor.Gold, Gold },
                { NetKnownColor.Goldenrod, Goldenrod },
                { NetKnownColor.Gray, Gray },
                { NetKnownColor.Green, Green },
                { NetKnownColor.GreenYellow, GreenYellow },
                { NetKnownColor.Honeydew, Honeydew },
                { NetKnownColor.HotPink, HotPink },
                { NetKnownColor.IndianRed, IndianRed },
                { NetKnownColor.Indigo, Indigo },
                { NetKnownColor.Ivory, Ivory },
                { NetKnownColor.Khaki, Khaki },
                { NetKnownColor.Lavender, Lavender },
                { NetKnownColor.LavenderBlush, LavenderBlush },
                { NetKnownColor.LawnGreen, LawnGreen },
                { NetKnownColor.LemonChiffon, LemonChiffon },
                { NetKnownColor.LightBlue, LightBlue },
                { NetKnownColor.LightCoral, LightCoral },
                { NetKnownColor.LightCyan, LightCyan },
                { NetKnownColor.LightGoldenrodYellow, LightGoldenrodYellow },
                { NetKnownColor.LightGray, LightGray },
                { NetKnownColor.LightGreen, LightGreen },
                { NetKnownColor.LightPink, LightPink },
                { NetKnownColor.LightSalmon, LightSalmon },
                { NetKnownColor.LightSeaGreen, LightSeaGreen },
                { NetKnownColor.LightSkyBlue, LightSkyBlue },
                { NetKnownColor.LightSlateGray, LightSlateGray },
                { NetKnownColor.LightSteelBlue, LightSteelBlue },
                { NetKnownColor.LightYellow, LightYellow },
                { NetKnownColor.Lime, Lime },
                { NetKnownColor.LimeGreen, LimeGreen },
                { NetKnownColor.Linen, Linen },
                { NetKnownColor.Magenta, Magenta },
                { NetKnownColor.Maroon, Maroon },
                { NetKnownColor.MediumAquamarine, MediumAquamarine },
                { NetKnownColor.MediumBlue, MediumBlue },
                { NetKnownColor.MediumOrchid, MediumOrchid },
                { NetKnownColor.MediumPurple, MediumPurple },
                { NetKnownColor.MediumSeaGreen, MediumSeaGreen },
                { NetKnownColor.MediumSlateBlue, MediumSlateBlue },
                { NetKnownColor.MediumSpringGreen, MediumSpringGreen },
                { NetKnownColor.MediumTurquoise, MediumTurquoise },
                { NetKnownColor.MediumVioletRed, MediumVioletRed },
                { NetKnownColor.MidnightBlue, MidnightBlue },
                { NetKnownColor.MintCream, MintCream },
                { NetKnownColor.MistyRose, MistyRose },
                { NetKnownColor.Moccasin, Moccasin },
                { NetKnownColor.NavajoWhite, NavajoWhite },
                { NetKnownColor.Navy, Navy },
                { NetKnownColor.OldLace, OldLace },
                { NetKnownColor.Olive, Olive },
                { NetKnownColor.OliveDrab, OliveDrab },
                { NetKnownColor.Orange, Orange },
                { NetKnownColor.OrangeRed, OrangeRed },
                { NetKnownColor.Orchid, Orchid },
                { NetKnownColor.PaleGoldenrod, PaleGoldenrod },
                { NetKnownColor.PaleGreen, PaleGreen },
                { NetKnownColor.PaleTurquoise, PaleTurquoise },
                { NetKnownColor.PaleVioletRed, PaleVioletRed },
                { NetKnownColor.PapayaWhip, PapayaWhip },
                { NetKnownColor.PeachPuff, PeachPuff },
                { NetKnownColor.Peru, Peru },
                { NetKnownColor.Pink, Pink },
                { NetKnownColor.Plum, Plum },
                { NetKnownColor.PowderBlue, PowderBlue },
                { NetKnownColor.Purple, Purple },
                { NetKnownColor.Red, Red },
                { NetKnownColor.RosyBrown, RosyBrown },
                { NetKnownColor.RoyalBlue, RoyalBlue },
                { NetKnownColor.SaddleBrown, SaddleBrown },
                { NetKnownColor.Salmon, Salmon },
                { NetKnownColor.SandyBrown, SandyBrown },
                { NetKnownColor.SeaGreen, SeaGreen },
                { NetKnownColor.SeaShell, SeaShell },
                { NetKnownColor.Sienna, Sienna },
                { NetKnownColor.Silver, Silver },
                { NetKnownColor.SkyBlue, SkyBlue },
                { NetKnownColor.SlateBlue, SlateBlue },
                { NetKnownColor.SlateGray, SlateGray },
                { NetKnownColor.Snow, Snow },
                { NetKnownColor.SpringGreen, SpringGreen },
                { NetKnownColor.SteelBlue, SteelBlue },
                { NetKnownColor.Tan, Tan },
                { NetKnownColor.Teal, Teal },
                { NetKnownColor.Thistle, Thistle },
                { NetKnownColor.Tomato, Tomato },
                { NetKnownColor.Turquoise, Turquoise },
                { NetKnownColor.Violet, Violet },
                { NetKnownColor.Wheat, Wheat },
                { NetKnownColor.White, White },
                { NetKnownColor.WhiteSmoke, WhiteSmoke },
                { NetKnownColor.Yellow, Yellow },
                { NetKnownColor.YellowGreen, YellowGreen },
            };
        }

        public static Color32 AliceBlue = new Color32(240, 248, 255, 255);
        public static Color32 AntiqueWhite = new Color32(250, 235, 215, 255);
        public static Color32 Aqua = new Color32(0, 255, 255, 255);
        public static Color32 Aquamarine = new Color32(127, 255, 212, 255);
        public static Color32 Azure = new Color32(240, 255, 255, 255);
        public static Color32 Beige = new Color32(245, 245, 220, 255);
        public static Color32 Bisque = new Color32(255, 228, 196, 255);
        public static Color32 Black = new Color32(0, 0, 0, 255);
        public static Color32 BlanchedAlmond = new Color32(255, 235, 205, 255);
        public static Color32 Blue = new Color32(0, 0, 255, 255);
        public static Color32 BlueViolet = new Color32(138, 43, 226, 255);
        public static Color32 Brown = new Color32(165, 42, 42, 255);
        public static Color32 BurlyWood = new Color32(222, 184, 135, 255);
        public static Color32 CadetBlue = new Color32(95, 158, 160, 255);
        public static Color32 Chartreuse = new Color32(127, 255, 0, 255);
        public static Color32 Chocolate = new Color32(210, 105, 30, 255);
        public static Color32 Coral = new Color32(255, 127, 80, 255);
        public static Color32 CornflowerBlue = new Color32(100, 149, 237, 255);
        public static Color32 Cornsilk = new Color32(255, 248, 220, 255);
        public static Color32 Crimson = new Color32(220, 20, 60, 255);
        public static Color32 Cyan = new Color32(0, 255, 255, 255);
        public static Color32 DarkBlue = new Color32(0, 0, 139, 255);
        public static Color32 DarkCyan = new Color32(0, 139, 139, 255);
        public static Color32 DarkGoldenrod = new Color32(184, 134, 11, 255);
        public static Color32 DarkGray = new Color32(169, 169, 169, 255);
        public static Color32 DarkGreen = new Color32(0, 100, 0, 255);
        public static Color32 DarkKhaki = new Color32(189, 183, 107, 255);
        public static Color32 DarkMagenta = new Color32(139, 0, 139, 255);
        public static Color32 DarkOliveGreen = new Color32(85, 107, 47, 255);
        public static Color32 DarkOrange = new Color32(255, 140, 0, 255);
        public static Color32 DarkOrchid = new Color32(153, 50, 204, 255);
        public static Color32 DarkRed = new Color32(139, 0, 0, 255);
        public static Color32 DarkSalmon = new Color32(233, 150, 122, 255);
        public static Color32 DarkSeaGreen = new Color32(143, 188, 139, 255);
        public static Color32 DarkSlateBlue = new Color32(72, 61, 139, 255);
        public static Color32 DarkSlateGray = new Color32(47, 79, 79, 255);
        public static Color32 DarkTurquoise = new Color32(0, 206, 209, 255);
        public static Color32 DarkViolet = new Color32(148, 0, 211, 255);
        public static Color32 DeepPink = new Color32(255, 20, 147, 255);
        public static Color32 DeepSkyBlue = new Color32(0, 191, 255, 255);
        public static Color32 DimGray = new Color32(105, 105, 105, 255);
        public static Color32 DodgerBlue = new Color32(30, 144, 255, 255);
        public static Color32 Firebrick = new Color32(178, 34, 34, 255);
        public static Color32 FloralWhite = new Color32(255, 250, 240, 255);
        public static Color32 ForestGreen = new Color32(34, 139, 34, 255);
        public static Color32 Fuchsia = new Color32(255, 0, 255, 255);
        public static Color32 Gainsboro = new Color32(220, 220, 220, 255);
        public static Color32 GhostWhite = new Color32(248, 248, 255, 255);
        public static Color32 Gold = new Color32(255, 215, 0, 255);
        public static Color32 Goldenrod = new Color32(218, 165, 32, 255);
        public static Color32 Gray = new Color32(128, 128, 128, 255);
        public static Color32 Green = new Color32(0, 128, 0, 255);
        public static Color32 GreenYellow = new Color32(173, 255, 47, 255);
        public static Color32 Honeydew = new Color32(240, 255, 240, 255);
        public static Color32 HotPink = new Color32(255, 105, 180, 255);
        public static Color32 IndianRed = new Color32(205, 92, 92, 255);
        public static Color32 Indigo = new Color32(75, 0, 130, 255);
        public static Color32 Ivory = new Color32(255, 255, 240, 255);
        public static Color32 Khaki = new Color32(240, 230, 140, 255);
        public static Color32 Lavender = new Color32(230, 230, 250, 255);
        public static Color32 LavenderBlush = new Color32(255, 240, 245, 255);
        public static Color32 LawnGreen = new Color32(124, 252, 0, 255);
        public static Color32 LemonChiffon = new Color32(255, 250, 205, 255);
        public static Color32 LightBlue = new Color32(173, 216, 230, 255);
        public static Color32 LightCoral = new Color32(240, 128, 128, 255);
        public static Color32 LightCyan = new Color32(224, 255, 255, 255);
        public static Color32 LightGoldenrodYellow = new Color32(250, 250, 210, 255);
        public static Color32 LightGray = new Color32(211, 211, 211, 255);
        public static Color32 LightGreen = new Color32(144, 238, 144, 255);
        public static Color32 LightPink = new Color32(255, 182, 193, 255);
        public static Color32 LightSalmon = new Color32(255, 160, 122, 255);
        public static Color32 LightSeaGreen = new Color32(32, 178, 170, 255);
        public static Color32 LightSkyBlue = new Color32(135, 206, 250, 255);
        public static Color32 LightSlateGray = new Color32(119, 136, 153, 255);
        public static Color32 LightSteelBlue = new Color32(176, 196, 222, 255);
        public static Color32 LightYellow = new Color32(255, 255, 224, 255);
        public static Color32 Lime = new Color32(0, 255, 0, 255);
        public static Color32 LimeGreen = new Color32(50, 205, 50, 255);
        public static Color32 Linen = new Color32(250, 240, 230, 255);
        public static Color32 Magenta = new Color32(255, 0, 255, 255);
        public static Color32 Maroon = new Color32(128, 0, 0, 255);
        public static Color32 MediumAquamarine = new Color32(102, 205, 170, 255);
        public static Color32 MediumBlue = new Color32(0, 0, 205, 255);
        public static Color32 MediumOrchid = new Color32(186, 85, 211, 255);
        public static Color32 MediumPurple = new Color32(147, 112, 219, 255);
        public static Color32 MediumSeaGreen = new Color32(60, 179, 113, 255);
        public static Color32 MediumSlateBlue = new Color32(123, 104, 238, 255);
        public static Color32 MediumSpringGreen = new Color32(0, 250, 154, 255);
        public static Color32 MediumTurquoise = new Color32(72, 209, 204, 255);
        public static Color32 MediumVioletRed = new Color32(199, 21, 133, 255);
        public static Color32 MidnightBlue = new Color32(25, 25, 112, 255);
        public static Color32 MintCream = new Color32(245, 255, 250, 255);
        public static Color32 MistyRose = new Color32(255, 228, 225, 255);
        public static Color32 Moccasin = new Color32(255, 228, 181, 255);
        public static Color32 NavajoWhite = new Color32(255, 222, 173, 255);
        public static Color32 Navy = new Color32(0, 0, 128, 255);
        public static Color32 OldLace = new Color32(253, 245, 230, 255);
        public static Color32 Olive = new Color32(128, 128, 0, 255);
        public static Color32 OliveDrab = new Color32(107, 142, 35, 255);
        public static Color32 Orange = new Color32(255, 165, 0, 255);
        public static Color32 OrangeRed = new Color32(255, 69, 0, 255);
        public static Color32 Orchid = new Color32(218, 112, 214, 255);
        public static Color32 PaleGoldenrod = new Color32(238, 232, 170, 255);
        public static Color32 PaleGreen = new Color32(152, 251, 152, 255);
        public static Color32 PaleTurquoise = new Color32(175, 238, 238, 255);
        public static Color32 PaleVioletRed = new Color32(219, 112, 147, 255);
        public static Color32 PapayaWhip = new Color32(255, 239, 213, 255);
        public static Color32 PeachPuff = new Color32(255, 218, 185, 255);
        public static Color32 Peru = new Color32(205, 133, 63, 255);
        public static Color32 Pink = new Color32(255, 192, 203, 255);
        public static Color32 Plum = new Color32(221, 160, 221, 255);
        public static Color32 PowderBlue = new Color32(176, 224, 230, 255);
        public static Color32 Purple = new Color32(128, 0, 128, 255);
        public static Color32 Red = new Color32(255, 0, 0, 255);
        public static Color32 RosyBrown = new Color32(188, 143, 143, 255);
        public static Color32 RoyalBlue = new Color32(65, 105, 225, 255);
        public static Color32 SaddleBrown = new Color32(139, 69, 19, 255);
        public static Color32 Salmon = new Color32(250, 128, 114, 255);
        public static Color32 SandyBrown = new Color32(244, 164, 96, 255);
        public static Color32 SeaGreen = new Color32(46, 139, 87, 255);
        public static Color32 SeaShell = new Color32(255, 245, 238, 255);
        public static Color32 Sienna = new Color32(160, 82, 45, 255);
        public static Color32 Silver = new Color32(192, 192, 192, 255);
        public static Color32 SkyBlue = new Color32(135, 206, 235, 255);
        public static Color32 SlateBlue = new Color32(106, 90, 205, 255);
        public static Color32 SlateGray = new Color32(112, 128, 144, 255);
        public static Color32 Snow = new Color32(255, 250, 250, 255);
        public static Color32 SpringGreen = new Color32(0, 255, 127, 255);
        public static Color32 SteelBlue = new Color32(70, 130, 180, 255);
        public static Color32 Tan = new Color32(210, 180, 140, 255);
        public static Color32 Teal = new Color32(0, 128, 128, 255);
        public static Color32 Thistle = new Color32(216, 191, 216, 255);
        public static Color32 Tomato = new Color32(255, 99, 71, 255);
        public static Color32 Turquoise = new Color32(64, 224, 208, 255);
        public static Color32 Violet = new Color32(238, 130, 238, 255);
        public static Color32 Wheat = new Color32(245, 222, 179, 255);
        public static Color32 White = new Color32(255, 255, 255, 255);
        public static Color32 WhiteSmoke = new Color32(245, 245, 245, 255);
        public static Color32 Yellow = new Color32(255, 255, 0, 255);
        public static Color32 YellowGreen = new Color32(154, 205, 50, 255);

#if false
        public static void PrintKnownColors()
        {
            Dictionary<System.Drawing.KnownColor, Color32> knownC = new Dictionary<System.Drawing.KnownColor, Color32>();

            System.Drawing.KnownColor[] colors = (System.Drawing.KnownColor[])Enum.GetValues(typeof(System.Drawing.KnownColor));

            foreach (System.Drawing.KnownColor knownColor in colors)
            {
                if (knownColor < System.Drawing.KnownColor.AliceBlue)
                    continue;
                if (knownColor > System.Drawing.KnownColor.YellowGreen)
                    continue;

                System.Drawing.Color sdColor = System.Drawing.Color.FromKnownColor(knownColor);

                Color32 unityColor = new Color32(sdColor.R, sdColor.G, sdColor.B, sdColor.A);
                knownC.Add(knownColor, unityColor);
            }

            string f = "";

            foreach (var item in knownC)
            {
                f += Environment.NewLine;
                f += $"{{ NetKnownColor.{item.Key}, {item.Key}}},";
            }

            Debug.Log(f);
        }
#endif
    }

    public enum NetKnownColor
    {
        AliceBlue = 28,
        AntiqueWhite = 29,
        Aqua = 30,
        Aquamarine = 31,
        Azure = 32,
        Beige = 33,
        Bisque = 34,
        Black = 35,
        BlanchedAlmond = 36,
        Blue = 37,
        BlueViolet = 38,
        Brown = 39,
        BurlyWood = 40,
        CadetBlue = 41,
        Chartreuse = 42,
        Chocolate = 43,
        Coral = 44,
        CornflowerBlue = 45,
        Cornsilk = 46,
        Crimson = 47,
        Cyan = 48,
        DarkBlue = 49,
        DarkCyan = 50,
        DarkGoldenrod = 51,
        DarkGray = 52,
        DarkGreen = 53,
        DarkKhaki = 54,
        DarkMagenta = 55,
        DarkOliveGreen = 56,
        DarkOrange = 57,
        DarkOrchid = 58,
        DarkRed = 59,
        DarkSalmon = 60,
        DarkSeaGreen = 61,
        DarkSlateBlue = 62,
        DarkSlateGray = 63,
        DarkTurquoise = 64,
        DarkViolet = 65,
        DeepPink = 66,
        DeepSkyBlue = 67,
        DimGray = 68,
        DodgerBlue = 69,
        Firebrick = 70,
        FloralWhite = 71,
        ForestGreen = 72,
        Fuchsia = 73,
        Gainsboro = 74,
        GhostWhite = 75,
        Gold = 76,
        Goldenrod = 77,
        Gray = 78,
        Green = 79,
        GreenYellow = 80,
        Honeydew = 81,
        HotPink = 82,
        IndianRed = 83,
        Indigo = 84,
        Ivory = 85,
        Khaki = 86,
        Lavender = 87,
        LavenderBlush = 88,
        LawnGreen = 89,
        LemonChiffon = 90,
        LightBlue = 91,
        LightCoral = 92,
        LightCyan = 93,
        LightGoldenrodYellow = 94,
        LightGray = 95,
        LightGreen = 96,
        LightPink = 97,
        LightSalmon = 98,
        LightSeaGreen = 99,
        LightSkyBlue = 100,
        LightSlateGray = 101,
        LightSteelBlue = 102,
        LightYellow = 103,
        Lime = 104,
        LimeGreen = 105,
        Linen = 106,
        Magenta = 107,
        Maroon = 108,
        MediumAquamarine = 109,
        MediumBlue = 110,
        MediumOrchid = 111,
        MediumPurple = 112,
        MediumSeaGreen = 113,
        MediumSlateBlue = 114,
        MediumSpringGreen = 115,
        MediumTurquoise = 116,
        MediumVioletRed = 117,
        MidnightBlue = 118,
        MintCream = 119,
        MistyRose = 120,
        Moccasin = 121,
        NavajoWhite = 122,
        Navy = 123,
        OldLace = 124,
        Olive = 125,
        OliveDrab = 126,
        Orange = 127,
        OrangeRed = 128,
        Orchid = 129,
        PaleGoldenrod = 130,
        PaleGreen = 131,
        PaleTurquoise = 132,
        PaleVioletRed = 133,
        PapayaWhip = 134,
        PeachPuff = 135,
        Peru = 136,
        Pink = 137,
        Plum = 138,
        PowderBlue = 139,
        Purple = 140,
        Red = 141,
        RosyBrown = 142,
        RoyalBlue = 143,
        SaddleBrown = 144,
        Salmon = 145,
        SandyBrown = 146,
        SeaGreen = 147,
        SeaShell = 148,
        Sienna = 149,
        Silver = 150,
        SkyBlue = 151,
        SlateBlue = 152,
        SlateGray = 153,
        Snow = 154,
        SpringGreen = 155,
        SteelBlue = 156,
        Tan = 157,
        Teal = 158,
        Thistle = 159,
        Tomato = 160,
        Turquoise = 161,
        Violet = 162,
        Wheat = 163,
        White = 164,
        WhiteSmoke = 165,
        Yellow = 166,
        YellowGreen = 167
    }
}