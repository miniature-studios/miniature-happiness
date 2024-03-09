using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace Level.Inventory.Controller
{
    public partial class ControllerImpl
    {
        [SerializeField]
        private View view;

        [SerializeField]
        private bool prepareForBuildingModeOnStart = false;

        [SerializeField]
        [ShowIf(nameof(prepareForBuildingModeOnStart))]
        private int roomsAddCountOnStart = 20;

        private void Start()
        {
            if (prepareForBuildingModeOnStart)
            {
                view.ShowInventory();
                AddRoomsFromAssets(roomsAddCountOnStart);
            }
        }

        [Button(Style = ButtonStyle.Box)]
        private void AddRoomToInventory(CoreModel coreModel)
        {
            Assert.IsNotNull(coreModel);
            AddNewRoom(CoreModel.InstantiateCoreModel(coreModel.Uid));
        }

        [Button(Style = ButtonStyle.Box)]
        private void AddRoomsFromAssets(int count)
        {
            foreach (
                AssetWithLocation<CoreModel> core in AddressableTools<CoreModel>.LoadAllFromLabel(
                    "CoreModel"
                )
            )
            {
                for (int i = 0; i < count; i++)
                {
                    AddNewRoom(CoreModel.InstantiateCoreModel(core.Asset.Uid));
                }
            }
        }
    }
}
