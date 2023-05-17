using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class NeedModifierEffect : IEffect
{
    [SerializeField] private List<Need.NeedProperties> needModifiers;
    public ReadOnlyCollection<Need.NeedProperties> NeedModifiers =>
        needModifiers.AsReadOnly();
}
