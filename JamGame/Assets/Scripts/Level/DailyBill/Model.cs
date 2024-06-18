using System;
using System.Collections.Generic;
using System.Linq;
using Level.Room;
using TileBuilder;
using UnityEngine;

namespace Level.DailyBill
{
    [Serializable]
    public struct RoomCheck
    {
        public CoreModel CoreModel;
        public int Count;
        public int OneCost => CoreModel.RoomInfo.RentCost.Value;
        public int SumCost => CoreModel.RoomInfo.RentCost.Value * Count;
    }

    [Serializable]
    public struct Check
    {
        public List<RoomCheck> Checks;
        public int Sum => Checks.Sum(x => x.SumCost);
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
                Checks = tileBuilder
                    .AllCoreModels.Where(x => x.RoomInfo.RentCost.Value != 0)
                    .GroupBy(x => x.Uid, x => x)
                    .Select(group => new RoomCheck()
                    {
                        CoreModel = group.First(),
                        Count = group.Count()
                    })
                    .ToList()
            };
        }
    }
}
