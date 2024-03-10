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

        [Button(Style = ButtonStyle.Box)]
        public void AddRoomsFromAssets(int count)
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
