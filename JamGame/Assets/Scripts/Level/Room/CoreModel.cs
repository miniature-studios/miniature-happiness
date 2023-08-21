using Common;
using System;
using UnityEngine;

namespace Level.Room
{
    [Serializable]
    public struct Cost
    {
        [SerializeField]
        private int cost;
        public int Value => cost;
    }

    [Serializable]
    public struct TariffProperties
    {
        [SerializeField]
        private int waterConsumption;
        public int WaterConsumption => waterConsumption;

        [SerializeField]
        private int electricityConsumption;
        public int ElectricityConsumption => electricityConsumption;
    }

    [Serializable]
    public struct PlacingProperties
    {
        [SerializeField]
        private int placingRotation;
        public int PlacingRotation
        {
            get => placingRotation;
            set => placingRotation = value;
        }
    }

    [AddComponentMenu("Scripts/Level.Room.CoreModel")]
    public partial class CoreModel : MonoBehaviour
    {
        [SerializeField]
        private Cost cost;
        public Cost Cost => cost;

        [SerializeField]
        private TariffProperties tariffProperties;
        public TariffProperties TariffProperties => tariffProperties;

        [SerializeField]
        private PlacingProperties placingProperties;
        public PlacingProperties PlacingProperties => placingProperties;

        public void ModifyPlacingProperties(RotationDirection rotationDirection)
        {
            placingProperties.PlacingRotation += (int)rotationDirection;
            placingProperties.PlacingRotation += 4;
            placingProperties.PlacingRotation %= 4;
        }

        public void SetPlacingProperties(int rotation)
        {
            placingProperties.PlacingRotation = rotation;
        }
    }
}
