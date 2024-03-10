using Sirenix.OdinInspector;
using UnityEngine;

namespace BuildingEditor
{
    [AddComponentMenu("Scripts/BuildingEditor/BuildingEditor")]
    internal class BuildingEditorImpl : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Level.Inventory.Controller.ControllerImpl inventoryController;

        [Required]
        [SerializeField]
        private Level.Inventory.View inventoryView;

        [SerializeField]
        private int roomsAddCount = 20;

        private void Start()
        {
            inventoryView.ShowInventory();
            inventoryController.AddRoomsFromAssets(roomsAddCount);
        }
    }
}
