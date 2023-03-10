using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EmployeeController))]
public class Employee : MonoBehaviour
{
    public Location location;

    EmployeeController controller;
    List<Need> needs = new List<Need>();

    void Start()
    {
        controller = GetComponent<EmployeeController>();
    }

    void Update()
    {

    }

    void UpdateNeedPriority()
    {
        Need prev_top_pririty = needs[0];
        needs.Sort((x, y) => x.satisfied.CompareTo(y.satisfied));
        foreach (var need in needs)
        {
            if (need == prev_top_pririty)
                break;

            bool booked = location.TryBookSlotInNeedProvider(this, need.NeedType);
            if (booked)
            {
                // Start move to booked slot.
            }
        }
    }
}
