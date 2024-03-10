using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Inventory.Controller
{
    [AddComponentMenu(
        "Scripts/Level/Inventory/Controller/Level.Inventory.Controller.BuilderModePreparer"
    )]
    internal class BuilderModePreparer : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private ControllerImpl controller;

        [Required]
        [SerializeField]
        private View view;

        [SerializeField]
        private int roomsAddCount = 20;

        private void Awake()
        {
            controller = GetComponent<ControllerImpl>();
        }

        private void Start()
        {
            view.ShowInventory();
            controller.AddRoomsFromAssets(roomsAddCount);
        }
    }
}
