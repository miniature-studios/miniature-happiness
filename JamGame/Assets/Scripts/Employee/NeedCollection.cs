using System.Collections.Generic;
using UnityEngine;

public class NeedCollection
{
    public List<NeedParameters> Needs;
}

public class NeedModifiers
{
    float decrease_speed = 1.0f;
    float satisfaction_time = 1.0f;
    float satisfaction_gained = 1.0f;
}

public class NeedCollectionModifier
{
    public Dictionary<NeedType, NeedModifiers> Modifiers;

    public NeedCollection Apply(NeedCollection need_collection)
    {
        NeedCollection new_collection = new NeedCollection();
        foreach (var need in need_collection.Needs)
        {
            //need.
            //new_collection.Needs.Add(need);
        }
        return new_collection;
    }
}

