using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.View
{
    [AddComponentMenu("Scripts/Level/Shop/View/Level.Shop.View.Tab")]
    internal class Tab : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image image;

        [SerializeField]
        private Color activeColor;

        [SerializeField]
        private Color inactiveColor;

        [Button]
        public void Activate()
        {
            image.color = activeColor;
        }

        [Button]
        public void Deactivate()
        {
            image.color = inactiveColor;
        }
    }
}
