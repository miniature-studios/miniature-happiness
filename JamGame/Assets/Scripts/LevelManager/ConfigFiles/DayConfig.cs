using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DayConfig", menuName = "Level/DayConfig", order = 1)]
public class DayConfig : ScriptableObject
{
    public List<DayAction> DayActions;
}

