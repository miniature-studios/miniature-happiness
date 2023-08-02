using System;
using UnityEngine;

namespace TileUnion 
{
    [Serializable]
    public class SerializedPlaceCondition
    {
        [SerializeField] 
        private string selectedType;

        [SerializeField]
        private RequiredTile requiredTile;

        public IPlaceCondition ToPlaceCondition()
        {
            return selectedType switch
            {
                "RequiredTile" => requiredTile,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
