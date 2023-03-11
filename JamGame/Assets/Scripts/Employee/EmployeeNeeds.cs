using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmployeeNeeds", menuName = "ScriptableObjects/EmployeeNeeds", order = 1)]
public class EmployeeNeeds : ScriptableObject
{
    public List<Need> needs;
}
