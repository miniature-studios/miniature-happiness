using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] private Employee employeePrototype;
    private List<NeedProvider> needProviders;
    private readonly List<Employee> employees = new();

    // TODO: Update in runtime.
    private List<IOverlayRenderer> overlayRenderers;

    private void Start()
    {
        InitGameMode();

        overlayRenderers = GetComponentsInChildren<IOverlayRenderer>().ToList();
    }

    public void InitGameMode()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());
    }

    [SerializeField] private EmployeeNeeds employeeNeeds;
    public void AddEmployee()
    {
        Employee new_employee = Instantiate(employeePrototype, employeePrototype.transform.parent);
        new_employee.gameObject.SetActive(true);
        employees.Add(new_employee);
    }

    public IEnumerable<NeedProvider> FindAllAvailableProviders(Employee employee, NeedType need_type)
    {
        foreach (NeedProvider provider in needProviders)
        {
            if (provider.NeedType == need_type && provider.IsAvailable(employee))
            {
                yield return provider;
            }
        }
    }

    // TODO: Move to the other script (UI folder?).
    public void ApplyOverlay<O>(O overlay) where O : class, IOverlay
    {
        foreach (IOverlayRenderer overlay_renderer in overlayRenderers)
        {
            if (overlay_renderer is IOverlayRenderer<O> or)
            {
                or.ApplyOverlay(overlay);
            }
        }
    }

    // TODO: Move to the other script (UI folder?).
    public void RevertOverlay<O>(O overlay) where O : class, IOverlay
    {
        //foreach (IOverlayRenderer overlay_renderer in overlayRenderers)
        //{
        //    if (overlay_renderer is IOverlayRenderer<O> or)
        //    {
        //        or.RevertOverlay(overlay);
        //    }
        //}
    }
}