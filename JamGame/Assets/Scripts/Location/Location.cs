using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField]
    private Employee employeePrototype;
    private List<NeedProvider> needProviders;
    private readonly List<Employee> employees = new();

    private void Start()
    {
        InitGameMode();
    }

    public void InitGameMode()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());
    }

    public void AddEmployee()
    {
        Employee new_employee = Instantiate(employeePrototype, employeePrototype.transform.parent);
        new_employee.gameObject.SetActive(true);
        employees.Add(new_employee);
    }

    public IEnumerable<NeedProvider> FindAllAvailableProviders(
        Employee employee,
        NeedType need_type
    )
    {
        foreach (NeedProvider provider in needProviders)
        {
            if (provider.NeedType == need_type && provider.IsAvailable(employee))
            {
                yield return provider;
            }
        }
    }
}
