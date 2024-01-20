using System;
using System.Collections.Generic;
using Common;
using TileUnion.PlaceCondition;
using UnityEngine;

namespace TileUnion
{
    [Serializable]
    public class PlacingProperties
    {
        [SerializeField]
        private int rotation;
        public int Rotation => rotation;

        public void SetRotation(int rotation)
        {
            this.rotation = ((rotation % 4) + 4) % 4;
        }

        public void ApplyRotation(RotationDirection rotation)
        {
            SetRotation(this.rotation + (int)rotation);
        }

        [SerializeField]
        private Vector2Int position;
        public Vector2Int Position => position;

        public void SetPosition(Vector2Int position)
        {
            this.position = position;
        }

        public void SetPositionAndRotation(Vector2Int position, int rotation)
        {
            this.rotation = rotation;
            this.position = position;
        }
    }

    [AddComponentMenu("Scripts/TileUnion/TileUnion.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private PlacingProperties placingProperties;
        public PlacingProperties PlacingProperties => placingProperties;

        [SerializeReference]
        private List<IPlaceCondition> placeConditions = new();
        public IEnumerable<IPlaceCondition> PlaceConditions => placeConditions;
    }
}
