using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tariffs
{
    public float WaterCost;
    public float ElectricityCost;
}

[Serializable]
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/LevelConfig", order = 0)]
public class LevelConfig : ScriptableObject
{
    public Tariffs tariffs;
    public List<DayConfig> Days;
    public DayConfig DefaultDay;
}
