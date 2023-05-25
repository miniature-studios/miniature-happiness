using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Employee))]
public class Personality : MonoBehaviour
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private List<Quirk> quirks;

    private void Start()
    {
        Employee employee = GetComponent<Employee>();

        foreach (Quirk quirk in quirks)
        {
            foreach (Need.NeedProperties additional_need in quirk.AdditionalNeeds)
            {
                employee.AddNeed(additional_need);
            }
        }
    }
}
