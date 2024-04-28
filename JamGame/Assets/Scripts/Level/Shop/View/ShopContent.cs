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

        private bool FullyScrolledUp => Mathf.Abs(scrollRect.content.anchoredPosition.y) < 0.001;
        private float HeightDelta =>
            Mathf.Abs(scrollRect.content.rect.height - scrollRect.viewport.rect.height);
        private bool FullyScrolledDown =>
            Mathf.Abs(HeightDelta - scrollRect.content.anchoredPosition.y) < 0.001;

        private void Update()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.viewport);
            if (scrollRect.viewport.rect.height <= scrollRect.content.rect.height)
            {
                arrowUp.SetActive(!FullyScrolledUp);
                arrowDown.SetActive(!FullyScrolledDown);
            }
            else
            {
                arrowUp.SetActive(false);
                arrowDown.SetActive(false);
            }
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
