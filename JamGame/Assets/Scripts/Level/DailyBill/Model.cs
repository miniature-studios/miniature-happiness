using System.Linq;
using TileBuilder;
using UnityEngine;

namespace Level.DailyBill
{
    [SerializeField]
    public struct Check
    {
        public int Rent;
        public int Sum => Rent;
    }

    [AddComponentMenu("Scripts/Level/DailyBill/Level.DailyBill.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        // Called by animator when showing.
        public Check ComputeCheck()
        {
            return new Check() { Rent = tileBuilder.AllCoreModels.Sum(x => x.RentCost.Value) };
        }
    }
}
