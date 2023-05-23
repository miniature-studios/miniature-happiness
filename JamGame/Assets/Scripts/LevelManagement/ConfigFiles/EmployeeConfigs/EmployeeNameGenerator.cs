using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EmployeeNameGenerator", menuName = "Level/EmployeeNameGenerator", order = 0)]
public class EmployeeNameGenerator : ScriptableObject
{
    [SerializeField] private List<string> firstNames;
    [SerializeField] private List<string> lastNames;

    public string GenerateName()
    {
        string first_name = firstNames[UnityEngine.Random.Range(0, firstNames.Count)];
        string last_name = lastNames[UnityEngine.Random.Range(0, lastNames.Count)];

        return $"{first_name} {last_name}";
    }
}

