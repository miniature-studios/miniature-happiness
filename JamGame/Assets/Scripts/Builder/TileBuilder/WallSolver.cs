using Common;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "WallSolver", menuName = "ScriptableObjects/WallSolver", order = 4)]
public class WallSolver : ScriptableObject
{
    public List<TileWallType> ForSameWalls_PriorityQueue = new() {
        TileWallType.none,
        TileWallType.door,
        TileWallType.window,
        TileWallType.wall
    };
    public List<TileWallType> ForSameWalls_PriorityQueue_FirstDoor = new() {
        TileWallType.door,
        TileWallType.none,
        TileWallType.window,
        TileWallType.wall
    };
    public List<TileWallType> ForDifferentTiles_PriorityQueue = new() {
        TileWallType.wall,
        TileWallType.window,
        TileWallType.door,
        TileWallType.none
    };

    public List<string> ignoringMarks = new()
    {
        "immutable",
    };

    public TileWallType? ChooseWall(IEnumerable<string> MyMarks, List<TileWallType> MyWalls, IEnumerable<string> OutMarks, List<TileWallType> OutWalls)
    {
        var MyNewMarks = MyMarks.Where(x => !ignoringMarks.Contains(x));
        var OutNewMarks = OutMarks.Where(x => !ignoringMarks.Contains(x));

        var wall_type_intersect = MyWalls.Intersect(OutWalls).ToList();
        if (wall_type_intersect.Count == 1)
        {
            return wall_type_intersect.First();
        }
        else if (wall_type_intersect.Count > 1)
        {
            var marks_intersect = MyNewMarks.Intersect(OutNewMarks).ToList();
            // Unique rule
            if ((
                !(MyMarks.Contains("freespace") || OutMarks.Contains("freespace"))
                &&
                !(MyMarks.Contains("outside") || OutMarks.Contains("outside")))
                &&
                (MyMarks.Contains("corridor") || OutMarks.Contains("corridor"))
                )
            {
                foreach (var iterator in ForSameWalls_PriorityQueue_FirstDoor)
                {
                    if (wall_type_intersect.Contains(iterator))
                        return iterator;
                }
            }
            else // Gather rule
            {
                foreach (var iterator in marks_intersect.Count == 0 ? ForDifferentTiles_PriorityQueue : ForSameWalls_PriorityQueue)
                {
                    if (wall_type_intersect.Contains(iterator))
                        return iterator;
                }
            }
        }
        return null;
    }
}

