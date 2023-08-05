using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion.PlaceCondition
{
    [InterfaceEditor]
    public interface IPlaceCondition
    {
        public Result<List<TileImpl>> ApplyCondition(
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

        public Result<List<TileImpl>> ApplyCondition(
            TileUnionImpl targetTileUnion,
            TileBuilderImpl tileBuilderImpl
        )
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
                ? new SuccessResult<List<TileImpl>>(new List<TileImpl>() { targetTile })
                : new FailResult<List<TileImpl>>("Condition pass");
        }
    }
}
