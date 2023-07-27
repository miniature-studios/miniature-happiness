using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnematorWithBool
{
    public string AnimatorName;
    public bool Bool;

    public AnematorWithBool(string animator, bool _bool)
    {
        AnimatorName = animator;
        Bool = _bool;
    }
}

[Serializable]
public class InterfaceMatch
{
    public string InterfaceName;
    public List<AnematorWithBool> Bools;

    public InterfaceMatch(string interfaceName, List<AnematorWithBool> bools)
    {
        InterfaceName = interfaceName;
        Bools = bools;
    }
}

[Serializable]
public class AnimatorList
{
    public List<Animator> Animators = new();
    public List<InterfaceMatch> InterfaceMatcher = new();
}

public class AnimatorsSwitcher : MonoBehaviour
{
    [SerializeField]
    private AnimatorList animatorList;
    private InterfaceMatch interfaceMatch;

    // TODO: Better transition
    public void SetAnimatorStates(Type day_type)
    {
        interfaceMatch = animatorList.InterfaceMatcher.Find(x => x.InterfaceName == day_type.Name);
        foreach (AnematorWithBool anim_bools in interfaceMatch.Bools)
        {
            animatorList.Animators
                .Find(x => x.name == anim_bools.AnimatorName)
                .SetBool("Showed", anim_bools.Bool);
        }
    }
}
