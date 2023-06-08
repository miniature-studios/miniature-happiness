using UnityEngine;

public partial class TileBuilder
{
    [HideInInspector]
    public GameObject LoadingPrefab;

    [HideInInspector]
    public string SavingName = "SampleBuilding";

    [HideInInspector]
    public TileUnion StairsPrefab;

    [HideInInspector]
    public TileUnion WindowPrefab;

    [HideInInspector]
    public TileUnion OutdoorPrefab;

    [HideInInspector]
    public TileUnion CorridoorPrefab;

    [HideInInspector]
    public TileUnion WorkingPlaceFree;

    [HideInInspector]
    public TileUnion WorkingPlace;

    [HideInInspector]
    public int SquareSideLength = 30;

    [HideInInspector]
    public bool LoadFromSceneComposition;

    public void CreateRandomBuilding()
    {
        int x = 0;
        int y = 0;
        DeleteAllTiles();
        for (int i = 0; i < SquareSideLength * SquareSideLength; i++)
        {
            float value = Random.value * 100;
            if (value < 50)
            {
                CreateTileAndBind(freespacePrefab, new(x, y), 0);
            }
            else if (value is > 50 and < 65)
            {
                CreateTileAndBind(StairsPrefab, new(x, y), 0);
            }
            else if (value is > 65 and < 80)
            {
                CreateTileAndBind(WindowPrefab, new(x, y), 0);
            }
            else if (value > 80)
            {
                CreateTileAndBind(OutdoorPrefab, new(x, y), 0);
            }
            y++;
            if (y >= SquareSideLength)
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
            CreateTileAndBind(OutdoorPrefab, new(0, i), 0);
        }

        for (int i = 0; i < 8; i++)
        {
            if (i == 1)
            {
                CreateTileAndBind(StairsPrefab, new(i + 1, 0), 0);
            }
            else
            {
                CreateTileAndBind(OutdoorPrefab, new(i + 1, 0), 0);
            }

            for (int j = 0; j < 7; j++)
            {
                if (j == 2)
                {
                    CreateTileAndBind(CorridoorPrefab, new(i + 1, j + 1), 0);
                }
                else if (j == 3)
                {
                    CreateTileAndBind(WorkingPlace, new(i + 1, j + 1), 0);
                }
                else if (j == 4)
                {
                    CreateTileAndBind(WorkingPlaceFree, new(i + 1, j + 1), 0);
                }
                else
                {
                    CreateTileAndBind(freespacePrefab, new(i + 1, j + 1), 0);
                }
            }
            CreateTileAndBind(OutdoorPrefab, new(i + 1, 8), 0);
        }
        for (int i = 0; i < 9; i++)
        {
            CreateTileAndBind(OutdoorPrefab, new(9, i), 0);
        }
    }

    public void CreateFourTiles()
    {
        DeleteAllTiles();
        CreateTileAndBind(OutdoorPrefab, new(0, 0), 0);
        CreateTileAndBind(OutdoorPrefab, new(0, 1), 0);
        CreateTileAndBind(WorkingPlaceFree, new(1, 0), 0);
        CreateTileAndBind(WorkingPlace, new(1, 1), 0);
    }
}
