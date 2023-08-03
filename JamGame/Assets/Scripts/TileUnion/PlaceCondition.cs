using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion.PlaceCondition
{
    [InterfaceEditor]
    public interface IPlaceCondition
    {
        public List<TileImpl> ApplyCondition(
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

        public List<TileImpl> ApplyCondition(
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
            ImmutableList<TileImpl> outTargetTile = tileBuilderImpl.TileUnionDictionary[outTargetPosition].GetImmutableTile(
                outTargetPosition
            );
            return outTargetTile.Count == 0
                || outTargetTile.First().Marks.Intersect(requiredTags).Count() != requiredTags.Count()
                ? new List<TileImpl>() { targetTile }
                : null;
        }
    }
}
