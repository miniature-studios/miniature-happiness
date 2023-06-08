using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "Employee/Buff", order = 1)]
public class Buff : ScriptableObject
{
    // TODO: refactor
    public float Time;

    [SerializeField]
    private List<SerializedEffect> rawEffects;
    private List<IEffect> effects;
    public ReadOnlyCollection<IEffect> Effects
    {
        get
        {
            if (effects == null)
            {
                effects = new();
                foreach (SerializedEffect effect in rawEffects)
                {
                    effects.Add(effect.ToEffect());
                }
            }

            return effects.AsReadOnly();
        }
    }

    // TODO: Move to BuffView
    // TODO: Change to Image
    [SerializeField]
    private string name_;
    public string Name => name_;
}
