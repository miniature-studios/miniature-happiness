using System;
using UnityEngine;

namespace TileUnion.PlaceCondition 
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
