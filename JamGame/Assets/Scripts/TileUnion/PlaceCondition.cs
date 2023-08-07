using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion.PlaceCondition
{
    public struct ConditionResult
    {
        public bool Success;
        public bool Failure => !Success;
        public List<TileImpl> FailedTiles;
        public ConditionResult(bool success, List<TileImpl> failedTiles)
        {
            Success = success;
            FailedTiles = failedTiles;
        }
    }

    [InterfaceEditor]
    public interface IPlaceCondition
    {
        public ConditionResult ApplyCondition(
            TileUnionImpl targetTileUnion,
            TileBuilderImpl tileBuilderImpl
        );
    }

    [Serializable]
    public class RequiredTile : IPlaceCondition
    {
        [SerializeField]
        private TileImpl targetTile;

        [SerializeField]
        private Direction direction;

        [SerializeField]
        private List<string> requiredTags;

        public ConditionResult ApplyCondition(TileUnionImpl targetTileUnion, TileBuilderImpl tileBuilderImpl)
        {
            Direction bufferDirection = direction;
            Enumerable
                .Range(0, targetTileUnion.Rotation)
                .ToList()
                .ForEach(x => bufferDirection = bufferDirection.Rotate90());
            Vector2Int outTargetPosition =
                targetTile.Position + targetTileUnion.Position + bufferDirection.ToVector2Int();

            IEnumerable<string> marks = null;
            if (
                tileBuilderImpl.TileUnionDictionary.TryGetValue(
                    outTargetPosition,
                    out TileUnionImpl outTargetTile
                )
            )
            {
                marks = outTargetTile.GetTileMarks(outTargetPosition);
            }
            return marks == null || marks.Intersect(requiredTags).Count() != requiredTags.Count()
                ? new ConditionResult(false, new List<TileImpl>() { targetTile })
                : new ConditionResult(true, null);
        }
    }
}
