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
        private None no;

        [SerializeField]
        private ExtendedEmployeeInfo extendedEmployeeInfo;

        [SerializeField]
        private Stress stress;

        public IOverlay ToOverlay()
        {
            return selectedType switch
            {
                "No" => no,
                "ExtendedEmployeeInfo" => extendedEmployeeInfo,
                "Stress" => stress,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
