﻿using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion
{
    [AddComponentMenu("Scripts/TileUnion.MeetingRoomLogics")]
    public class MeetingRoomLogics : MonoBehaviour
    {
        [SerializeField]
        private TileUnionImpl tileUnion;

        public TileUnionImpl TileUnion => tileUnion;

        [SerializeField]
        private List<TileImpl> tilesToAdd;

        public List<TileImpl> TilesToAdd => tilesToAdd;

        [SerializeField]
        private Direction growDirection;
        public Direction GrowDirection => growDirection;

        [SerializeField]
        private int maximumSize;
        public int MaximumSize => maximumSize;

        [SerializeField]
        [InspectorReadOnly]
        private int currentSize = 2;

        public int CurrentSize
        {
            get => currentSize;
            set => currentSize = value;
        }

        [SerializeField]
        private List<string> incorrectMarks = new();
        public IEnumerable<string> IncorrectMarks => incorrectMarks;

        public bool IsEnoughPlace(int employeeCount)
        {
            return employeeCount <= (currentSize * 2) - 1;
        }

        public bool IsCanFitEmployees(int employeeCount)
        {
            return employeeCount >= (maximumSize * 2) - 1;
        }

        public (
            IEnumerable<Vector2Int> movingTileUnionPositions,
            IEnumerable<Vector2Int> positionsToTake,
            Vector2Int movingDirection
        ) GetMeetingRoomGrowingInformation()
        {
            Direction tempGrowDirection = GrowDirection;
            Enumerable
                .Range(0, TileUnion.Rotation)
                .ToList()
                .ForEach((x) => tempGrowDirection = tempGrowDirection.Rotate90());

            IEnumerable<Vector2Int> movingTileUnionPositions = tempGrowDirection switch
            {
                Direction.Up
                    => TileUnion.TilesPositions
                        .OrderByDescending(pos => pos.y)
                        .Take(TilesToAdd.Count()),
                Direction.Right
                    => TileUnion.TilesPositions
                        .OrderByDescending(pos => pos.x)
                        .Take(TilesToAdd.Count()),
                Direction.Down
                    => TileUnion.TilesPositions.OrderBy(pos => pos.y).Take(TilesToAdd.Count()),
                Direction.Left
                    => TileUnion.TilesPositions.OrderBy(pos => pos.x).Take(TilesToAdd.Count()),
                _ => throw new ArgumentException()
            };

            Vector2Int movingDirection = tempGrowDirection.ToVector2Int();
            IEnumerable<Vector2Int> positionsToTake = movingTileUnionPositions.Select(
                x => x + movingDirection
            );

            return (movingTileUnionPositions, positionsToTake, movingDirection);
        }

        public void AddTiles(Vector2Int movingDirection, IEnumerable<Vector2Int> movingTileUnionPositions)
        {
            TileUnion.MoveTiles(movingDirection, movingTileUnionPositions);
            Dictionary<(Vector2Int position, int roatation), TileImpl> addingConfig = new();

            for (int i = 0; i < movingTileUnionPositions.Count(); i++)
            {
                addingConfig.Add(
                    (movingTileUnionPositions.ToList()[i], TileUnion.Rotation),
                    TilesToAdd[i]
                );
            }

            TileUnion.AddTiles(addingConfig);

            Vector2Int unionPosition = TileUnion.Position;
            TileUnion.CreateCache(false);
            TileUnion.SetPosition(unionPosition);
            CurrentSize++;
        }
    }
}
