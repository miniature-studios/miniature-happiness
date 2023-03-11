using System.Collections.Generic;
using UnityEngine;
using static NeedProvider;

[RequireComponent(typeof(NeedCollectionModifier))]
public class NeedSlot : MonoBehaviour
{
    [SerializeField] NeedCollectionModifier needCollectionModifier;
    List<NeedParameters> needParameters;

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

    public void ProvideNeedParameters(List<NeedParameters> parameters)
    {
        needParameters = needCollectionModifier.Apply(parameters);
    }

    public NeedParameters GetNeedParameters(NeedType need_type)
    {
        foreach (var np in needParameters)
            if (np.NeedType == need_type)
                return new NeedParameters(np);

        Debug.LogError("Need parameters not found: " + need_type);
        return null;
    }
}
