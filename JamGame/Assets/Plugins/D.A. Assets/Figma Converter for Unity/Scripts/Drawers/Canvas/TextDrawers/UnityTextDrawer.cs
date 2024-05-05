using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class UnityTextDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public Text Draw(FObject fobject)
        {
            fobject.Data.GameObject.TryAddGraphic(out Text text);

            SetStyle(text, fobject);
            SetFont(text, fobject);

            text.fontStyle = fobject.GetFontWeight();
            text.alignment = fobject.GetTextAnchor();

            return text;
        }

        private void SetFont(Text text, FObject fobject)
        {
            Font font = monoBeh.FontLoader.GetFontFromArray(fobject, monoBeh.FontLoader.TtfFonts);
            text.font = font;
        }

        private void SetStyle(Text text, FObject fobject)
        {
            text.resizeTextForBestFit = monoBeh.Settings.UnityTextSettings.BestFit;
            text.text = fobject.GetText();
            text.resizeTextMinSize = 1;

            text.resizeTextMaxSize = Convert.ToInt32(fobject.Style.FontSize);
            text.fontSize = Convert.ToInt32(fobject.Style.FontSize);

            text.verticalOverflow = monoBeh.Settings.UnityTextSettings.VerticalWrapMode;
            text.horizontalOverflow = monoBeh.Settings.UnityTextSettings.HorizontalWrapMode;
            text.lineSpacing = monoBeh.Settings.UnityTextSettings.FontLineSpacing;

            if (fobject.Fills[0].GradientStops != null)
            {
                text.color = fobject.Fills[0].GradientStops[0].Color;
            }
            else
            {
                text.color = fobject.Fills[0].Color;
            }
        }
    }
}
