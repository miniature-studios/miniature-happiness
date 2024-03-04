using System.Collections.Generic;
using System.Linq;
using Common;
using TileUnion.Tile;
using UnityEngine;

namespace TileBuilder
{
    [CreateAssetMenu(
        fileName = "TileBuilder.WallSolver",
        menuName = "TileBuilder/WallSolver",
        order = 1
    )]
    public class WallSolver : ScriptableObject
    {
        [SerializeField]
        private List<WallType> forSameTilesPriorityQueue;

        [SerializeField]
        private List<WallType> forSameTilesPriorityQueueForCorridor;

        [SerializeField]
        private List<WallType> forDifferentTilesPriorityQueue;

        [SerializeField]
        private List<RoomTileLabel> ignoringMarks;

        public WallType? ChooseWall(
            IEnumerable<RoomTileLabel> myMarks,
            IEnumerable<WallType> myWalls,
            IEnumerable<RoomTileLabel> outMarks,
            IEnumerable<WallType> outWalls
        )
        {
            IEnumerable<RoomTileLabel> myNewMarks = myMarks.Where(x => !ignoringMarks.Contains(x));
            IEnumerable<RoomTileLabel> outNewMarks = outMarks.Where(x =>
                !ignoringMarks.Contains(x)
            );

            IEnumerable<WallType> wallTypeIntersect = myWalls.Intersect(outWalls).ToList();
            if (wallTypeIntersect.Count() == 1)
            {
                return wallTypeIntersect.First();
            }
            else if (wallTypeIntersect.Count() > 1)
            {
                IEnumerable<RoomTileLabel> marksIntersect = myNewMarks
                    .Intersect(outNewMarks)
                    .ToList();
                // Unique rule
                if (
                    !(
                        myMarks.Contains(RoomTileLabel.FreeSpace)
                        || outMarks.Contains(RoomTileLabel.FreeSpace)
                    )
                    && !(
                        myMarks.Contains(RoomTileLabel.Outside)
                        || outMarks.Contains(RoomTileLabel.Outside)
                    )
                    && (
                        myMarks.Contains(RoomTileLabel.Corridor)
                        || outMarks.Contains(RoomTileLabel.Corridor)
                    )
                )
                {
                    foreach (WallType iterator in forSameTilesPriorityQueueForCorridor)
                    {
                        if (wallTypeIntersect.Contains(iterator))
                        {
                            return iterator;
                        }
                    }
                }
                else // Gather rule
                {
                    foreach (
                        WallType iterator in marksIntersect.Count() == 0
                            ? forDifferentTilesPriorityQueue
                            : forSameTilesPriorityQueue
                    )
                    {
                        if (wallTypeIntersect.Contains(iterator))
                        {
                            return iterator;
                        }
                    }
                }
            }
            return null;
        }
    }
}
