using Common;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEngine;

namespace TileUnion.PlaceCondition
{
    [HideReferenceObjectPicker]
    public interface IPlaceCondition
    {
        public Result PassCondition(TileUnionImpl targetTileUnion, TileBuilderImpl tileBuilderImpl);
    }

    [Serializable]
    public class RequiredTile : IPlaceCondition
    {
        [SerializeField]
        [FoldoutGroup("RequiredTile")]
        private Vector2Int position;

        [SerializeField]
        [FoldoutGroup("RequiredTile")]
        private List<string> requiredTileTags = new();

        public Result PassCondition(TileUnionImpl targetTileUnion, TileBuilderImpl tileBuilderImpl)
        {
            Vector2Int bufferPosition = position;
            Enumerable
                .Range(0, targetTileUnion.Rotation)
                .ToList()
                .ForEach(x => bufferPosition = new Vector2Int(bufferPosition.y, -bufferPosition.x));
            Vector2Int outTargetPosition = targetTileUnion.Position + bufferPosition;

            TileUnionImpl outTargetTile = tileBuilderImpl.GetTileUnionInPosition(outTargetPosition);
            return outTargetTile == null
                ? new FailResult("No target tile")
                : outTargetTile.GetTileMarks(outTargetPosition).Intersect(requiredTileTags).Count()
                == requiredTileTags.Count()
                    ? new SuccessResult()
                    : new FailResult("No needed Marks");
        }
    }
}
