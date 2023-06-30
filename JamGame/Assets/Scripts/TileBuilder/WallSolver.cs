using System.Collections.Generic;
using System.Linq;
using TileUnion.Tile;
using Unity.VisualScripting;
using UnityEngine;

namespace TileBuilder
{
    [CreateAssetMenu(fileName = "WallSolver", menuName = "Builder/WallSolver", order = 1)]
    public class WallSolver : ScriptableObject
    {
        [SerializeField]
        private List<WallType> forSameTilesPriorityQueue;

        [SerializeField]
        private List<WallType> forSameTilesPriorityQueueForCoridoor;

        [SerializeField]
        private List<WallType> forDifferentTilesPriorityQueue;

        [SerializeField]
        private List<string> ignoringMarks;

        public WallType? ChooseWall(
            IEnumerable<string> my_marks,
            IEnumerable<WallType> my_walls,
            IEnumerable<string> out_marks,
            IEnumerable<WallType> out_walls
        )
        {
            IEnumerable<string> my_new_marks = my_marks.Where(x => !ignoringMarks.Contains(x));
            IEnumerable<string> out_new_marks = out_marks.Where(x => !ignoringMarks.Contains(x));

            IEnumerable<WallType> wall_type_intersect = my_walls.Intersect(out_walls).ToList();
            if (wall_type_intersect.Count() == 1)
            {
                return wall_type_intersect.First();
            }
            else if (wall_type_intersect.Count() > 1)
            {
                IEnumerable<string> marks_intersect = my_new_marks
                    .Intersect(out_new_marks)
                    .ToList();
                // Unique rule
                if (
                    !(my_marks.Contains("freespace") || out_marks.Contains("freespace"))
                    && !(my_marks.Contains("Outside") || out_marks.Contains("Outside"))
                    && (my_marks.Contains("Corridor") || out_marks.Contains("Corridor"))
                )
                {
                    foreach (WallType iterator in forSameTilesPriorityQueueForCoridoor)
                    {
                        if (wall_type_intersect.Contains(iterator))
                        {
                            return iterator;
                        }
                    }
                }
                else // Gather rule
                {
                    foreach (
                        WallType iterator in marks_intersect.Count() == 0
                            ? forDifferentTilesPriorityQueue
                            : forSameTilesPriorityQueue
                    )
                    {
                        if (wall_type_intersect.Contains(iterator))
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
