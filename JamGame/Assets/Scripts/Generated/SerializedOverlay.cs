using System;
using UnityEngine;

[Serializable]
public class SerializedOverlay
{
    [SerializeField]
    private string selectedType;

    [SerializeField]
    private DefaultOverlay defaultOverlay;

    [SerializeField]
    private ExtendedEmployeeInfoOverlay extendedEmployeeInfoOverlay;

    [SerializeField]
    private StressOverlay stressOverlay;

    public IOverlay ToOverlay()
    {
        return selectedType switch
        {
            "DefaultOverlay" => defaultOverlay,
            "ExtendedEmployeeInfoOverlay" => extendedEmployeeInfoOverlay,
            "StressOverlay" => stressOverlay,
            _ => throw new NotImplementedException(),
        };
    }
}
