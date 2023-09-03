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
        private Check check;
        public Check Check => check;

        public UnityEvent<Check> CheckChanged;

        public void UpdateCheck()
        {
            check = new() { Rent = tileBuilder.AllRoomsCOreModels.Sum(x => x.RentCost.Value) };
            CheckChanged?.Invoke(Check);
        }
    }
}
