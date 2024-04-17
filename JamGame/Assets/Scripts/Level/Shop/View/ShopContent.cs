using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.ShopContent")]
    internal class ShopContent : MonoBehaviour, IShopContent
    {
        [Required]
        [SerializeField]
        private ScrollRect scrollRect;
        public RectTransform ContentTransform => scrollRect.content;

        [Required]
        [SerializeField]
        private GameObject arrowUp;

        [Required]
        [SerializeField]
        private GameObject arrowDown;

        [Required]
        [SerializeField]
        private GameObject contentParent;

        public event Action OnSwitchedTo;

        private void Update()
        {
            // Todo
        }

        public void Show()
        {
            contentParent.SetActive(true);
        }

        public void Hide()
        {
            contentParent.SetActive(false);
        }
    }
}
