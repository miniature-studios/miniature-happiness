using System.Collections.Generic;
using System.Linq;
using Level.Room;
using TileBuilder;
using UnityEngine;

namespace Level.DailyBill
{
    [SerializeField]
    public struct Check
    {
        public Dictionary<CoreModel, int> RentByRoom;
        public int Sum => RentByRoom.Values.Sum();
    }

    [AddComponentMenu("Scripts/Level/DailyBill/Level.DailyBill.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        // Called by animator when showing.
        public Check ComputeCheck()
        {
            return new Check()
            {
                RentByRoom = tileBuilder
                    .AllCoreModels.Where(x => x.RentCost.Value != 0)
                    .GroupBy(x => x.Uid, x => x)
                    .Select(x => (CoreModel: x.First(), Sum: x.Sum(x => x.RentCost.Value)))
                    .ToDictionary(x => x.CoreModel, x => x.Sum)
            };
        }
    }
}
