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
        private RectTransform viewport;

        [Required]
        [SerializeField]
        private GameObject arrowUp;

        [Required]
        [SerializeField]
        private GameObject arrowDown;

        [Required]
        [SerializeField]
        private GameObject contentParent;

        private bool FullyScrolledUp => scrollRect.content.anchoredPosition.y == 0;
        private bool FullyScrolledDown =>
            scrollRect.content.anchoredPosition.y == 650
            || viewport.sizeDelta.y > scrollRect.content.sizeDelta.y;

        private void Update()
        {
            arrowUp.SetActive(FullyScrolledUp);
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
