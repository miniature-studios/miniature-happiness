using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NeedParametersModifiers
{
    public float satisfaction_time = 1.0f;
    public float satisfaction_gained = 1.0f;
}

[Serializable]
public struct NeedTypeWithModifiers
{
    public NeedType ty;
    public NeedParametersModifiers mods;
}


public class NeedCollectionModifier : MonoBehaviour
{
    public List<NeedTypeWithModifiers> ModifiersRaw;
    public Dictionary<NeedType, NeedParametersModifiers> Modifiers;

    private void OnValidate()
    {
        PrepareModifiers();
    }

    private void Awake()
    {
        PrepareModifiers();
    }

    void PrepareModifiers()
    {
        Modifiers = new Dictionary<NeedType, NeedParametersModifiers>();
        if (ModifiersRaw != null)
            foreach (var modifiers in ModifiersRaw)
                Modifiers.Add(modifiers.ty, modifiers.mods);
    }

    public List<NeedParameters> Apply(List<NeedParameters> needs)
    {
        List<NeedParameters> new_needs = new List<NeedParameters>();
        foreach (var need in needs)
        {
            var new_need = new NeedParameters(need);
            if (Modifiers.ContainsKey(need.NeedType))
                new_need.ApplyModifiers(Modifiers[need.NeedType]);
            new_needs.Add(new_need);
        }
        return new_needs;
    }
}
