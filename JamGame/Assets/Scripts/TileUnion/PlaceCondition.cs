using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion.PlaceCondition
{
    public class Result
    {
        public bool Success { get; protected set; }
        public bool Failure => !Success;

        protected List<TileImpl> FailedTiles_ = null;
        public List<TileImpl> FailedTiles =>
            Success ? throw new Exception("Condition pass, no failed tiles.") : FailedTiles_;
    }

    public class SuccessResult : Result
    {
        public SuccessResult()
        {
            Success = true;
        }
    }

    public class FailResult : Result
    {
        public FailResult(List<TileImpl> failedTiles)
        {
            FailedTiles_ = failedTiles;
            Success = false;
        }
    }

    [InterfaceEditor]
    public interface IPlaceCondition
    {
        public Result ApplyCondition(
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

        public Result ApplyCondition(TileUnionImpl targetTileUnion, TileBuilderImpl tileBuilderImpl)
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
                ? new FailResult(new List<TileImpl>() { targetTile })
                : new SuccessResult();
        }
    }
}
