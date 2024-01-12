using TMPro;
using UnityEngine;

namespace Level.Inventory.Room
{
    [AddComponentMenu("Scripts/Level/Inventory/Room/Level.Inventory.Room.ExtendedView")]
    public class ExtendedView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text label;

        public void SetLabelText(string labelText)
        {
            label.text = labelText;
        }
    }
}
