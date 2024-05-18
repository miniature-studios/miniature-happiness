using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.Employee
{
    [AddComponentMenu("Scripts/Level/Shop/Employee/Level.Shop.Employee.CardQuirkIcon")]
    internal class CardQuirkIcon : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image iconGraphic;

        public void SetIcon(Sprite sprite)
        {
            iconGraphic.sprite = sprite;
        }
    }
}
