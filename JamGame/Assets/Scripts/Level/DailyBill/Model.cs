using Common;
using System.Linq;
using TileBuilder;
using UnityEngine;
using UnityEngine.Events;

namespace Level.DailyBill
{
    [SerializeField]
    public struct Check
    {
        public int Rent;
        public int Sum => Rent;
    }

    [AddComponentMenu("Scripts/Level.DailyBill.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField, InspectorReadOnly]
        private Check check = new();
        public Check Check => check;

        public UnityEvent<Check> CheckChanged;

        // Called by animator when showing.
        public void ComputeCheck()
        {
            check.Rent = tileBuilder.AllCoreModels.Sum(x => x.RentCost.Value);
            CheckChanged?.Invoke(Check);
        }
    }
}
