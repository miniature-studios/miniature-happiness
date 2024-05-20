using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Common.UI
{
    [AddComponentMenu("Scripts/Common/UI/Common.UI.OnHoverImageColorSwapper")]
    internal class OnHoverImageColorSwapper
        : MonoBehaviour,
            IPointerEnterHandler,
            IPointerExitHandler
    {
        [SerializeField]
        private List<Image> backgroundImages;

        [SerializeField]
        private Color hoveredColor;

        [SerializeField]
        private Color notHoveredColor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetBackgroundImagesColor(hoveredColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetBackgroundImagesColor(notHoveredColor);
        }

        private void SetBackgroundImagesColor(Color color)
        {
            foreach (Image image in backgroundImages)
            {
                image.color = color;
            }
        }
    }
}
