using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEngine;

namespace Level.DailyBill
{
    [SerializeField]
    public struct Check
    {
        public Dictionary<string, int> RentByRoom;
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
                    .GroupBy(x => x.Title)
                    .Select(x => (x.First().Title, Sum: x.Sum(x => x.RentCost.Value)))
                    .ToDictionary(x => x.Title, x => x.Sum)
            };
        }
    }
}
