using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Inventory
{
    [AddComponentMenu("Scripts/Level/Inventory/Level.Inventory.RoomDescriptionView")]
    internal class RoomDescriptionView : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Image image;

        [SerializeField]
        [Required]
        private TMP_Text description;

        [SerializeField]
        [Required]
        private TMP_Text title;

        [SerializeField]
        [Required]
        private TMP_Text rentCostLabel;

        public void OnActiveRoomChanged(Room.View view)
        {
            gameObject.SetActive(view != null);

            if (view != null)
            {
                title.text = view.CoreModel.RoomInfo.Title;
                description.text = view.CoreModel.RoomInfo.Description;
                image.sprite = view.CoreModel.InventoryModel.DescriptionImage;
                rentCostLabel.text = view.CoreModel.RoomInfo.RentCost.Value.ToString() + "/day";

                UpdateHorizontalPosition(view);
            }
        }

        private void UpdateHorizontalPosition(Room.View view)
        {
            Vector3 position = transform.position;
            position.x = view.transform.position.x;
            transform.position = position;
        }
    }
}
