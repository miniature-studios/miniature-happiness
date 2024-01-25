using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.ExtendedView")]
    public class ExtendedView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text label;

        private RectTransform canvas;
        private Transform parent;

        private void Awake()
        {
            parent = transform.parent;
            canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        }

        public bool IsVisible => gameObject.activeSelf;

        public void Hide()
        {
            transform.SetParent(parent);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            transform.SetParent(canvas);
            transform.position = parent.position;
            gameObject.SetActive(true);
        }

        public void SetLabelText(string labelText)
        {
            label.text = labelText;
        }
    }
}
