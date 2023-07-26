using Level.Config;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion;
using UnityEngine;

namespace Level
{
    [SerializeField]
    public struct Check
    {
        public int Water;
        public int Electricity;
        public int Rent;
        public readonly int Sum => Water + Electricity + Rent;
        public readonly Check Data => this;
    }

    [AddComponentMenu("Level.TariffsCounter")]
    public class TarrifsCounter : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        public Check GetCheck(Tariffs tariffs)
        {
            int inside_tiles_count = tileBuilder.GetAllInsideListPositions().Count();
            IEnumerable<TileUnionImpl> room_properties = tileBuilder.GetTileUnionsInPositions(
                tileBuilder.GetAllInsideListPositions()
            );

            return new()
            {
                Rent = inside_tiles_count * tariffs.RentCost,
                Water = room_properties
                    .Select(x => x.TarrifProperties.WaterConsumption * tariffs.WaterCost)
                    .Sum(),
                Electricity = room_properties
                    .Select(
                        x => x.TarrifProperties.ElectricityConsumption * tariffs.ElectricityCost
                    )
                    .Sum(),
            };
        }
    }
}
