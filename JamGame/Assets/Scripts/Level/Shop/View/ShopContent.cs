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

        private bool FullyScrolledUp => Mathf.Abs(scrollRect.normalizedPosition.y) < 0.001;
        private bool FullyScrolledDown => Mathf.Abs(scrollRect.normalizedPosition.y) > 0.999;

        private void Update()
        {
            if (scrollRect.viewport.sizeDelta.y > scrollRect.content.sizeDelta.y)
            {
                arrowUp.SetActive(FullyScrolledUp);
                arrowDown.SetActive(FullyScrolledDown);
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
