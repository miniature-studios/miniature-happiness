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

    [AddComponentMenu("Scripts/Level.TariffsCounter")]
    public class TariffsCounter : MonoBehaviour
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
            int insideTilesCount = tileBuilder.GetAllInsideListPositions().Count();
            IEnumerable<TileUnionImpl> roomProperties = tileBuilder.GetTileUnionsInPositions(
                tileBuilder.GetAllInsideListPositions()
            );

            check = new()
            {
                Rent = insideTilesCount * levelConfig.Config.Tariffs.RentCost,
                Water = roomProperties
                    .Select(
                        x =>
                            x.GetCoreModel().TariffProperties.WaterConsumption
                            * levelConfig.Config.Tariffs.WaterCost
                    )
                    .Sum(),
                Electricity = roomProperties
                    .Select(
                        x =>
                            x.GetCoreModel().TariffProperties.ElectricityConsumption
                            * levelConfig.Config.Tariffs.ElectricityCost
                    )
                    .Sum(),
            };

            CheckChanged?.Invoke(Check);
        }
    }
}
