using System;
using UnityEngine;

public interface IEffect
{

}

[Serializable]
public class SerializedEffect
{
    [SerializeField] private string effectType;

    // TODO: Refactor (codegen?)
    [SerializeField] private ControllerEffect controllerEffect;
    [SerializeField] private NeedModifierEffect needModifierEffect;
    [SerializeField] private StressEffect stressEffect;

    public IEffect ToEffect()
    {
        return effectType switch
        {
            "controllerEffect" => controllerEffect,
            "needModifierEffect" => needModifierEffect,
            "stressEffect" => stressEffect,
            _ => throw new NotImplementedException(),
        };
    }
}
