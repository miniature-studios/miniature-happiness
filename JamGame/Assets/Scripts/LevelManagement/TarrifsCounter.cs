using System.Linq;
using UnityEngine;

public struct Check
{
    public int Water;
    public int Electricity;
    public int Rent;
    public int Summ => Water + Electricity + Rent;
}

public class TarrifsCounter : MonoBehaviour
{
    [SerializeField] private TileBuilderController tileBuilderController;
    public Check GetCheck(Tariffs tariffs)
    {
        TileBuilderController.OfficeInfo info = tileBuilderController.GetOfficeInfo();
        return new()
        {
            Rent = info.InsideTilesCount * tariffs.RentCost,
            Water = info.RoomProperties.Select(x => x.WaterConsumption * tariffs.WaterCost).Sum(),
            Electricity = info.RoomProperties.Select(x => x.ElectricityComsumption * tariffs.ElectricityCost).Sum(),
        };
    }
}

