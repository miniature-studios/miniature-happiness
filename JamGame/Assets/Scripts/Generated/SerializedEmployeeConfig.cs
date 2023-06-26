using System;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    public class SerializedEmployeeConfig
    {
        [SerializeField]
        private string selectedType;

        [SerializeField]
        private FixedEmployeeConfig fixedEmployeeConfig;

        [SerializeField]
        private RandomEmployeeConfig randomEmployeeConfig;

        public IEmployeeConfig ToEmployeeConfig()
        {
            return selectedType switch
            {
                "FixedEmployeeConfig" => fixedEmployeeConfig,
                "RandomEmployeeConfig" => randomEmployeeConfig,
                _ => throw new NotImplementedException(),
            };
        }
    }
}