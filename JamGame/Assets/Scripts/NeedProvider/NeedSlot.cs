using System.Collections.Generic;
using UnityEngine;
using static NeedProvider;

[RequireComponent(typeof(NeedCollectionModifier))]
public class NeedSlot : MonoBehaviour
{
    [SerializeField] private NeedCollectionModifier needCollectionModifier;
    private List<NeedParameters> needParameters;

    // FIXME: employee target position should be defined as Room[Slot]
    public Room room;

    public Filter filter;
    private Employee employee = null;

    private void Start()
    {
        room = GetComponentInParent<Room>();
    }

    public bool TryBook(Employee employee)
    {
        return this.employee == null && filter.IsEmployeeAllowed(employee);
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
        foreach (NeedParameters np in needParameters)
        {
            if (np.NeedType == need_type)
            {
                return new NeedParameters(np);
            }
        }

        Debug.LogError("Need parameters not found: " + need_type);
        return null;
    }
}
