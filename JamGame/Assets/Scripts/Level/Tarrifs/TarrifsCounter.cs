using Common;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [SerializeField]
    public struct Check
    {
        public int Water;
        public int Electricity;
        public int Rent;
        public readonly int Sum => Water + Electricity + Rent;
    }

    [AddComponentMenu("Level.TariffsCounter")]
    public class TarrifsCounter : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private ConfigHandler levelConfig;

        [SerializeField, InspectorReadOnly]
        private Check check;
        public Check Check => check;

        public UnityEvent<Check> CheckChanged;

        public void UpdateCheck()
        {
            int inside_tiles_count = tileBuilder.GetAllInsideListPositions().Count();
            IEnumerable<TileUnionImpl> room_properties = tileBuilder.GetTileUnionsInPositions(
                tileBuilder.GetAllInsideListPositions()
            );

            check = new()
            {
                Rent = inside_tiles_count * levelConfig.Config.Tariffs.RentCost,
                Water = room_properties
                    .Select(
                        x =>
                            x.TariffProperties.WaterConsumption
                            * levelConfig.Config.Tariffs.WaterCost
                    )
                    .Sum(),
                Electricity = room_properties
                    .Select(
                        x =>
                            x.TariffProperties.ElectricityConsumption
                            * levelConfig.Config.Tariffs.ElectricityCost
                    )
                    .Sum(),
            };

            CheckChanged?.Invoke(Check);
        }
    }
}
