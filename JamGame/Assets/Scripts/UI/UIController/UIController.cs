using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimatorBool
{
    public string AnimatorName;
    public float StartDelay;
    public bool StartIsActive;
    public float EndDelay;
    public bool EndIsActive;
    public AnimatorBool(string animator_name, float start_delay, bool start_is_active, float end_delay, bool end_is_active)
    {
        AnimatorName = animator_name;
        StartDelay = start_delay;
        StartIsActive = start_is_active;
        EndDelay = end_delay;
        EndIsActive = end_is_active;
    }
}

[Serializable]
public class InterfaceMatch
{
    public string InterfaceName;
    public List<AnimatorBool> animatorBools;
    public InterfaceMatch(string interfaceName, List<AnimatorBool> animatorBools)
    {
        InterfaceName = interfaceName;
        this.animatorBools = animatorBools;
    }
}

[Serializable]
public class NamedAnimator
{
    public string Name;
    public Animator Animator;
}

[Serializable]
public class AnimatorList
{
    public List<NamedAnimator> NamedAnimators = new();
    public List<InterfaceMatch> InterfaceMatcher = new();
}

public class UIController : MonoBehaviour
{
    [SerializeField] private AnimatorList animatorList;
    private InterfaceMatch interfaceMatch;
    public void PlayDayActionStart(Type day_type, Action animation_end)
    {
        interfaceMatch = animatorList.InterfaceMatcher.Find(x => x.InterfaceName == day_type.Name);
        float longest_delay = 0f;
        foreach (AnimatorBool anim_bools in interfaceMatch.animatorBools)
        {
            longest_delay = Mathf.Max(anim_bools.StartDelay, longest_delay);
            Animator animator = animatorList.NamedAnimators.Find(x => x.Name == anim_bools.AnimatorName).Animator;
            _ = StartCoroutine(AnimationTemplate(anim_bools.StartDelay, animator, anim_bools.StartIsActive));
        }
        _ = StartCoroutine(DoAfterWait(longest_delay, animation_end));
    }

    // TODO another implementation 
    public void PlayDayActionEnd(Action animation_end)
    {
        float longest_delay = 0f;
        foreach (AnimatorBool anim_bools in interfaceMatch.animatorBools)
        {
            longest_delay = Mathf.Max(anim_bools.StartDelay, longest_delay);
            Animator animator = animatorList.NamedAnimators.Find(x => x.Name == anim_bools.AnimatorName).Animator;
            animator.SetBool("Showed", anim_bools.EndIsActive);
        }
        _ = StartCoroutine(DoAfterWait(longest_delay, animation_end));
    }

    private IEnumerator AnimationTemplate(float delay, Animator animator, bool param)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Showed", param);
    }

    private IEnumerator DoAfterWait(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}

