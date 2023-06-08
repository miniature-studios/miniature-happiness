using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EmployeeWeights
{
    public float Weight;
    public Employee Employee;
}

[Serializable]
public class RandomEmployeeConfig : IEmployeeConfig
{
    [SerializeField]
    private List<EmployeeWeights> employeeWeights;

    [SerializeField]
    private EmployeeNameGenerator nameGenerator;

    public EmployeeConfig GetEmployeeConfig()
    {
        List<float> list = employeeWeights.Select(x => x.Weight).ToList();
        Employee result = employeeWeights[RandomTools.RandomlyChooseWithWeights(list)].Employee;
        return new EmployeeConfig(result, nameGenerator.GenerateName());
    }
}
