using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "FixedEmployeeConfig", menuName = "Level/Employee/FixedEmployeeConfig", order = 0)]
public class FixedEmployeeConfig : AbstractEmployeeConfig
{
    [SerializeField] private string employeeName;
    [SerializeField] private Profession profession;
    [SerializeField] private List<Peculiarity> peculiarities;

    public override EmployeeConfig GetEmployeeConfig()
    {
        return new EmployeeConfig(employeeName, profession, peculiarities);
    }
}
