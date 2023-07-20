using System;
using UnityEngine;

namespace Employee 
{
    [Serializable]
    public class SerializedEffect
    {
        [SerializeField] 
        private string selectedType;

        [SerializeField]
        private StressEffect stressEffect;

        [SerializeField]
        private NeedModifierEffect needModifierEffect;

        [SerializeField]
        private ControllerEffect controllerEffect;

        public IEffect ToEffect()
        {
            return selectedType switch
            {
                "StressEffect" => stressEffect,
                "NeedModifierEffect" => needModifierEffect,
                "ControllerEffect" => controllerEffect,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
