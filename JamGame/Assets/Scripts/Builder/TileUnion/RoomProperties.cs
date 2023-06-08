using System;
using UnityEngine;

[Serializable]
public class RoomProperties : MonoBehaviour
{
    [SerializeField]
    private int cost;

    [SerializeField]
    private int waterConsumption;

    [SerializeField]
    private int electricityComsumption;
    public int Cost => cost;
    public int WaterConsumption => waterConsumption;
    public int ElectricityComsumption => electricityComsumption;
}
