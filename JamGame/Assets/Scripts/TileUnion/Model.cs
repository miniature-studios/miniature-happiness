using Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
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

    [AddComponentMenu("Scripts/TileUnion.Model")]
    public class Model : SerializedMonoBehaviour
    {
        [SerializeField]
        private PlacingProperties placingProperties;
        public PlacingProperties PlacingProperties => placingProperties;

        [OdinSerialize]
        public IEnumerable<IPlaceCondition> PlaceConditions { get; private set; } =
            new List<IPlaceCondition>();
    }
}
