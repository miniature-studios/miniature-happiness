using System;
using Common;
using UnityEngine;

public class BuilderValidator : MonoBehaviour
{
    [SerializeField] GameObject pointerPrefab;

    [SerializeField] GameObject buildPrefab;
    [SerializeField] GameObject stairsPrefab;
    [SerializeField] GameObject windowPrefab;
    [SerializeField] GameObject outdoorPrefab;

    // Public for unity editor
    public TileBuilder tileBuilder;
    bool EditorMode = false;
    bool GameMode = false;

    public void Awake()
    {
        tileBuilder = new(pointerPrefab, transform);
        tileBuilder.CreateRandomTiles(buildPrefab, stairsPrefab, windowPrefab, outdoorPrefab);
    }

    public void SetEditorMode(bool mode)
    {
        GameMode = false;
        EditorMode = mode;
    }
    public void SetGameMode(bool mode)
    {
        GameMode = mode;
        EditorMode = false;
    }
    public void SelectTile(Tile tile)
    {
        tileBuilder.SelectTile(tile);
    }
    public void ComletePlacing()
    {
        tileBuilder.ComletePlacing();
    }
    public bool IsTileSelected()
    {
        return tileBuilder.IsTileSelected();
    }
    public void MoveSelectedTile(Direction direction)
    {
        tileBuilder.MoveSelectedTile(direction);
    }
    public void RotateSelectedTile()
    {
        tileBuilder.RotateSelectedTile();
    }
    public void ChangeXMatrixPlacing(int value)
    {
        tileBuilder.ChangeXMatrixPlacing(value);
    }
    public void DeleteSelectedTile()
    {
        tileBuilder.DeleteSelectedTile();
    }
}

