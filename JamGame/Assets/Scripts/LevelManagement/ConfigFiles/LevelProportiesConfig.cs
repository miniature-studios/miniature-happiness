using System;
using UnityEngine;

[Serializable]
public class Tariffs
{
    [SerializeField] private int waterCost;
    [SerializeField] private int electricityCost;
    [SerializeField] private int rentCost;
    public int WaterCost => waterCost;
    public int ElectricityCost => electricityCost;
    public int RentCost => rentCost;
}


[Serializable]
[CreateAssetMenu(fileName = "LevelProportiesConfig", menuName = "Level/LevelProportiesConfig", order = 1)]
public class LevelProportiesConfig : ScriptableObject
{
    [SerializeField] private float bossStressSpeed;
    [SerializeField] private Tariffs tariffs;
    public float BossStressSpeed => bossStressSpeed;
    public Tariffs Tariffs => tariffs;
}
