using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;

namespace Level.Inventory.Controller
{
    public partial class ControllerImpl
    {
        [Button(Style = ButtonStyle.Box)]
        private void AddRoomToInventory(CoreModel coreModel)
        {
            Assert.IsNotNull(coreModel);
            AddNewRoom(CoreModel.InstantiateCoreModel(coreModel.Uid));
        }

        [Button]
        private void AddRoomsFromAssets()
        {
            foreach (
                AssetWithLocation<CoreModel> core in AddressableTools<CoreModel>.LoadAllFromLabel(
                    "CoreModel"
                )
            )
            {
                AddNewRoom(CoreModel.InstantiateCoreModel(core.Asset.Uid));
            }
        }
    }
}
