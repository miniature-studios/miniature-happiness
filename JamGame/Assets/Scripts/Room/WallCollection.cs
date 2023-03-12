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

    void Awake()
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

    public WallType GetActiveWall()
    {
        foreach (var item in wallsDict)
        {
            if (item.Value == activeWall)
                return item.Key;
        }
        throw new Exception();
    }

    public void SetWall(WallType type)
    {
        if (!wallsDict.ContainsKey(type))
        {
            Debug.LogError($"Trying set {type}, that do not exist. {transform.position}");
        }

        foreach (var wall in wallsDict)
            wall.Value.SetActive(false);

        //if (activeWall != null)
        //    activeWall.SetActive(true);

        activeWall = wallsDict[type].gameObject;
        activeWall.SetActive(true);
    }
}
