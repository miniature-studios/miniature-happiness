using Common;
using Level.Config;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
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
        private TileBuilderImpl tileBuilder;

        public Check GetCheck(Tariffs tariffs)
        {
            int inside_tiles_count = tileBuilder.GetAllInsideListPositions().Count();
            IEnumerable<RoomProperties> room_properties = tileBuilder
                .GetTileUnionsInPositions(tileBuilder.GetAllInsideListPositions())
                .Where(x => x.TryGetComponent(out RoomProperties roomProperties))
                .Select(x => x.GetComponent<RoomProperties>());

            return new()
            {
                Rent = inside_tiles_count * tariffs.RentCost,
                Water = room_properties.Select(x => x.WaterConsumption * tariffs.WaterCost).Sum(),
                Electricity = room_properties
                    .Select(x => x.ElectricityComsumption * tariffs.ElectricityCost)
                    .Sum(),
            };
        }
    }
}
