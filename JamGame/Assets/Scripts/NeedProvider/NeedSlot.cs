using UnityEngine;
using static NeedProvider;

public class NeedSlot : MonoBehaviour
{
    // FIXME: employee target position should be defined as Room[Slot]
    public Room room;

    public Filter filter;
    Employee employee = null;

    void Start()
    {
        room = GetComponentInParent<Room>();
    }

    public bool TryBook(Employee employee)
    {
        if (this.employee != null)
            return false;

        if (!filter.IsEmployeeAllowed(employee))
            return false;

        return true;
    }

    public void Free()
    {
        employee = null;
    }
}
