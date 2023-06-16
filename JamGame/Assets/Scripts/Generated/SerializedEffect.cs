using System;
using UnityEngine;

[Serializable]
public class SerializedEffect
{
    [SerializeField]
    private string selectedType;

    [SerializeField]
    private Controller controllerEffect;

    [SerializeField]
    private NeedModifierEffect needModifierEffect;

    [SerializeField]
    private StressEffect stressEffect;

    public IEffect ToEffect()
    {
        return selectedType switch
        {
            "ControllerEffect" => controllerEffect,
            "NeedModifierEffect" => needModifierEffect,
            "StressEffect" => stressEffect,
            _ => throw new NotImplementedException(),
        };
    }
}
