using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion
{
    [InterfaceEditor]
    public interface IPlaceCondition
    {
        public Result ApplyCondition(
            TileUnionImpl targetTileUnion,
            TileBuilderImpl tileBuilderImpl,
            out List<TileImpl> erroredTiles
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

        public Result ApplyCondition(
            TileUnionImpl targetTileUnion,
            TileBuilderImpl tileBuilderImpl,
            out List<TileImpl> erroredTiles
        )
        {
            Direction bufferDirection = direction;
            Enumerable
                .Range(0, targetTileUnion.Rotation)
                .ToList()
                .ForEach(x => bufferDirection = bufferDirection.Rotate90());
            Vector2Int outTargetPosition =
                targetTile.Position + targetTileUnion.Position + bufferDirection.ToVector2Int();
            TileImpl outTargetTile = tileBuilderImpl.TileUnionDictionary[outTargetPosition].GetTile(
                outTargetPosition
            );
            if (
                outTargetTile == null
                || outTargetTile.Marks.Intersect(requiredTags).Count() != requiredTags.Count()
            )
            {
                erroredTiles = new List<TileImpl>() { targetTile };
                return new FailResult("No required tile");
            }
            else
            {
                erroredTiles = null;
                return new SuccessResult();
            }
        }
    }
}
