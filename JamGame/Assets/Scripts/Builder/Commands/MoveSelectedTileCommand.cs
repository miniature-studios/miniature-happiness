using Common;
using System.Linq;
using System;
using UnityEngine;

public class MoveSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public Direction direction;
    public MoveSelectedTileCommand(TileBuilder tileBuilder, Direction direction)
    {
        this.tileBuilder = tileBuilder;
        this.direction = direction;
    }
    public Response Execute()
    {
        return tileBuilder.MoveSelectedTile(direction);
    }
}

