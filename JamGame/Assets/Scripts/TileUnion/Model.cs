using Common;
using System;
using UnityEngine;

namespace TileUnion
{
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

    [AddComponentMenu("Scripts/TileUnion.Model")]
    public class Model : MonoBehaviour
    {
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
