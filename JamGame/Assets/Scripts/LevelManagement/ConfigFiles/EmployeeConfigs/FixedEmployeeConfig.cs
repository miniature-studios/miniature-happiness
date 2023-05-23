using System;
using UnityEngine;

[Serializable]
public class FixedEmployeeConfig : IEmployeeConfig
{
    [SerializeField] private string employeeName;
    [SerializeField] private Employee employee;

    public EmployeeConfig GetEmployeeConfig()
    {
        return new EmployeeConfig(employee, employeeName);
    }
}
