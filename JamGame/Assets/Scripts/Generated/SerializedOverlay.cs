using System;
using UnityEngine;

namespace Overlay
{
    [Serializable]
    public class SerializedOverlay
    {
        [SerializeField]
        private string selectedType;

        [SerializeField]
        private Default defaultOverlay;

        [SerializeField]
        private ExtendedEmployeeInfo extendedEmployeeInfoOverlay;

        [SerializeField]
        private Stress stressOverlay;

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
}
