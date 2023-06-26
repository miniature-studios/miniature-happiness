using Common;
using Level.Config;
using System.Linq;
using UnityEngine;

namespace Level
{
    [SerializeField]
    public struct Check : IReadonlyData<Check>
    {
        public int Water;
        public int Electricity;
        public int Rent;
        public readonly int Sum => Water + Electricity + Rent;
        public readonly Check Data => this;
    }

    public class TarrifsCounter : MonoBehaviour
    {
        [SerializeField]
        private OfficeMonitoring officeMonitoring;

        public Check GetCheck(Tariffs tariffs)
        {
            OfficeMonitoring.OfficeInfo info = officeMonitoring.GetOfficeInfo();
            return new()
            {
                Rent = info.InsideTilesCount * tariffs.RentCost,
                Water = info.RoomProperties
                    .Select(x => x.WaterConsumption * tariffs.WaterCost)
                    .Sum(),
                Electricity = info.RoomProperties
                    .Select(x => x.ElectricityComsumption * tariffs.ElectricityCost)
                    .Sum(),
            };
        }
    }
}
