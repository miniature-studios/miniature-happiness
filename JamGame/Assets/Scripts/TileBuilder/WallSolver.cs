using System.Collections.Generic;
using System.Linq;
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
        private List<string> ignoringMarks;

        public WallType? ChooseWall(
            IEnumerable<string> myMarks,
            IEnumerable<WallType> myWalls,
            IEnumerable<string> outMarks,
            IEnumerable<WallType> outWalls
        )
        {
            IEnumerable<string> myNewMarks = myMarks.Where(x => !ignoringMarks.Contains(x));
            IEnumerable<string> outNewMarks = outMarks.Where(x => !ignoringMarks.Contains(x));

            IEnumerable<WallType> wallTypeIntersect = myWalls.Intersect(outWalls).ToList();
            if (wallTypeIntersect.Count() == 1)
            {
                return wallTypeIntersect.First();
            }
            else if (wallTypeIntersect.Count() > 1)
            {
                IEnumerable<string> marksIntersect = myNewMarks.Intersect(outNewMarks).ToList();
                // Unique rule
                if (
                    !(myMarks.Contains("Freespace") || outMarks.Contains("Freespace"))
                    && !(myMarks.Contains("Outside") || outMarks.Contains("Outside"))
                    && (myMarks.Contains("Corridor") || outMarks.Contains("Corridor"))
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
