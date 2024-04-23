using Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.ShopContent")]
    internal class ShopContent : MonoBehaviour
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

        // TODO: find method to correctly show up/down arrows
        private bool FullyScrolledUp => scrollRect.content.anchoredPosition.y.IsEqualsZero();
        private bool FullyScrolledDown => true;

        private void OnGUI()
        {
            arrowUp.SetActive(!FullyScrolledUp);
            arrowDown.SetActive(!FullyScrolledDown);
        }

        [Button]
        public void OnToggleSwitched(bool check)
        {
            if (check)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        [Button]
        public void Show()
        {
            contentParent.SetActive(true);
        }

        [Button]
        public void Hide()
        {
            contentParent.SetActive(false);
        }
    }
}
