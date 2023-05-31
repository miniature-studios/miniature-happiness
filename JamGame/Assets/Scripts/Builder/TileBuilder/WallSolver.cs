using Common;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSolver", menuName = "Builder/WallSolver", order = 1)]
public class WallSolver : ScriptableObject
{
    [SerializeField] private List<TileWallType> ForSameTilesPriorityQueue;
    [SerializeField] private List<TileWallType> ForSameTilesPriorityQueueForCoridoor;
    [SerializeField] private List<TileWallType> ForDifferentTilesPriorityQueue;
    [SerializeField] private List<string> IgnoringMarks;

    public TileWallType? ChooseWall(IEnumerable<string> my_marks, IEnumerable<TileWallType> my_walls, IEnumerable<string> out_marks, IEnumerable<TileWallType> out_walls)
    {
        IEnumerable<string> my_new_marks = my_marks.Where(x => !IgnoringMarks.Contains(x));
        IEnumerable<string> out_new_marks = out_marks.Where(x => !IgnoringMarks.Contains(x));

        IEnumerable<TileWallType> wall_type_intersect = my_walls.Intersect(out_walls).ToList();
        if (wall_type_intersect.Count() == 1)
        {
            return wall_type_intersect.First();
        }
        else if (wall_type_intersect.Count() > 1)
        {
            IEnumerable<string> marks_intersect = my_new_marks.Intersect(out_new_marks).ToList();
            // Unique rule
            if (
                !(my_marks.Contains("freespace") || out_marks.Contains("freespace"))
                &&
                !(my_marks.Contains("Outside") || out_marks.Contains("Outside"))
                &&
                (my_marks.Contains("Corridor") || out_marks.Contains("Corridor"))
                )
            {
                foreach (TileWallType iterator in ForSameTilesPriorityQueueForCoridoor)
                {
                    if (wall_type_intersect.Contains(iterator))
                    {
                        return iterator;
                    }
                }
            }
            else // Gather rule
            {
                foreach (TileWallType iterator in marks_intersect.Count() == 0 ? ForDifferentTilesPriorityQueue : ForSameTilesPriorityQueue)
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