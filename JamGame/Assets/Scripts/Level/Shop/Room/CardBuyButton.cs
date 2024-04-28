using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.CardBuyButton")]
    internal class CardBuyButton : Button
    {
        protected override void Awake()
        {
            base.Awake();
            //colors.colorMultiplier = 0f;
            colors = new ColorBlock()
            {
                colorMultiplier = 0f,
                disabledColor = colors.disabledColor,
                fadeDuration = colors.fadeDuration,
                highlightedColor = colors.highlightedColor,
                normalColor = colors.normalColor,
                pressedColor = colors.pressedColor,
                selectedColor = colors.selectedColor,
            };
        }
    }
}
