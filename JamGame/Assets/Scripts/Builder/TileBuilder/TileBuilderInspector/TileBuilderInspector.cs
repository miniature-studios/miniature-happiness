using UnityEngine;

public partial class TileBuilder
{
    [HideInInspector] public GameObject loadingPrefab;
    [HideInInspector] public string savingName = "SampleBuilding";

    [HideInInspector] public TileUnion stairsPrefab;
    [HideInInspector] public TileUnion windowPrefab;
    [HideInInspector] public TileUnion outdoorPrefab;
    [HideInInspector] public TileUnion corridoorPrefab;
    [HideInInspector] public TileUnion workingPlaceFree;
    [HideInInspector] public TileUnion workingPlace;
    [HideInInspector] public int squareSideLength = 30;
    [HideInInspector] public bool loadFromSceneComposition;

    public void CreateRandomBuilding()
    {
        int x = 0;
        int y = 0;
        DeleteAllTiles();
        for (int i = 0; i < squareSideLength * squareSideLength; i++)
        {
            float value = Random.value * 100;
            if (value < 50)
            {
                CreateTileAndBind(freespacePrefab, new(x, y), 0);
            }
            else if (value is > 50 and < 65)
            {
                CreateTileAndBind(stairsPrefab, new(x, y), 0);
            }
            else if (value is > 65 and < 80)
            {
                CreateTileAndBind(windowPrefab, new(x, y), 0);
            }
            else if (value > 80)
            {
                CreateTileAndBind(outdoorPrefab, new(x, y), 0);
            }
            y++;
            if (y >= squareSideLength)
            {
                y = 0;
                x++;
            }
        }
    }

    public void CreateNormalBuilding()
    {
        DeleteAllTiles();
        for (int i = 0; i < 9; i++)
        {
            CreateTileAndBind(outdoorPrefab, new(0, i), 0);
        }

        for (int i = 0; i < 8; i++)
        {
            if (i == 1)
            {
                CreateTileAndBind(stairsPrefab, new(i + 1, 0), 0);
            }
            else
            {
                CreateTileAndBind(outdoorPrefab, new(i + 1, 0), 0);
            }

            for (int j = 0; j < 7; j++)
            {
                if (j == 2)
                {
                    CreateTileAndBind(corridoorPrefab, new(i + 1, j + 1), 0);
                }
                else if (j == 3)
                {
                    CreateTileAndBind(workingPlace, new(i + 1, j + 1), 0);
                }
                else if (j == 4)
                {
                    CreateTileAndBind(workingPlaceFree, new(i + 1, j + 1), 0);
                }
                else
                {
                    CreateTileAndBind(freespacePrefab, new(i + 1, j + 1), 0);
                }
            }
            CreateTileAndBind(outdoorPrefab, new(i + 1, 8), 0);
        }
        for (int i = 0; i < 9; i++)
        {
            CreateTileAndBind(outdoorPrefab, new(9, i), 0);
        }
    }

    public void CreateFourTiles()
    {
        DeleteAllTiles();
        CreateTileAndBind(outdoorPrefab, new(0, 0), 0);
        CreateTileAndBind(outdoorPrefab, new(0, 1), 0);
        CreateTileAndBind(workingPlaceFree, new(1, 0), 0);
        CreateTileAndBind(workingPlace, new(1, 1), 0);
    }
}

