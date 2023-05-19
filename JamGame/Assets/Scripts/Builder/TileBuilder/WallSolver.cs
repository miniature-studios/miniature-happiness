using Common;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSolver", menuName = "Builder/WallSolver", order = 1)]
public class WallSolver : ScriptableObject
{
    public List<TileWallType> ForSameWallsPriorityQueue = new() {
        TileWallType.None,
        TileWallType.Door,
        TileWallType.Window,
        TileWallType.Wall
    };
    public List<TileWallType> ForSameWallsPriorityQueueFirstDoor = new() {
        TileWallType.Door,
        TileWallType.None,
        TileWallType.Window,
        TileWallType.Wall
    };
    public List<TileWallType> ForDifferentTilesPriorityQueue = new() {
        TileWallType.Wall,
        TileWallType.Window,
        TileWallType.Door,
        TileWallType.None
    };

    public List<string> IgnoringMarks = new()
    {
        "immutable",
    };

    public TileWallType? ChooseWall(IEnumerable<string> my_marks, List<TileWallType> my_walls, IEnumerable<string> out_marks, List<TileWallType> out_walls)
    {
        IEnumerable<string> my_new_marks = my_marks.Where(x => !IgnoringMarks.Contains(x));
        IEnumerable<string> out_new_marks = out_marks.Where(x => !IgnoringMarks.Contains(x));

        List<TileWallType> wall_type_intersect = my_walls.Intersect(out_walls).ToList();
        if (wall_type_intersect.Count == 1)
        {
            return wall_type_intersect.First();
        }
        else if (wall_type_intersect.Count > 1)
        {
            List<string> marks_intersect = my_new_marks.Intersect(out_new_marks).ToList();
            // Unique rule
            if (
                !(my_marks.Contains("freespace") || out_marks.Contains("freespace"))
                &&
                !(my_marks.Contains("outside") || out_marks.Contains("outside"))
                &&
                (my_marks.Contains("corridor") || out_marks.Contains("corridor"))
                )
            {
                foreach (TileWallType iterator in ForSameWallsPriorityQueueFirstDoor)
                {
                    if (wall_type_intersect.Contains(iterator))
                    {
                        return iterator;
                    }
                }
            }
            else // Gather rule
            {
                foreach (TileWallType iterator in marks_intersect.Count == 0 ? ForDifferentTilesPriorityQueue : ForSameWallsPriorityQueue)
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