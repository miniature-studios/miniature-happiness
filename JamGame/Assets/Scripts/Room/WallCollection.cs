using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WallCollectionEntry
{
    public WallType type;
    public GameObject wall;
}

public class WallCollection : MonoBehaviour
{
    [SerializeField] WallCollectionEntry[] walls;
    Dictionary<WallType, GameObject> wallsDict;
    GameObject activeWall;

    void Start()
    {
        wallsDict = new Dictionary<WallType, GameObject>();
        foreach (var wall in walls)
            if (wall.wall != null)
                wallsDict.Add(wall.type, wall.wall);
    }

    public IEnumerable<WallType> GetAvailableWalls()
    {
        return wallsDict.Keys;
    }

    public void SetWall(WallType type)
    {
        if (activeWall != null)
            activeWall.SetActive(true);

        activeWall = wallsDict[type].gameObject;
        activeWall.SetActive(true);
    }
}
