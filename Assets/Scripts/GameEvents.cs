using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PositionEventArgs : EventArgs
{
    public Vector2Int positionFromPayload;
    public Vector2Int positionToPayload;
}

public class SpawnEventArgs : EventArgs
{
    public Vector2Int spawnPosPayload;
}

public class Vector2IntEventArgs : EventArgs
{
    public Vector2Int positionPayload;
}

public class GameEvents : MonoBehaviour
{
    public static event EventHandler<PositionEventArgs> SheepPositionChanged;
    public static event EventHandler<PositionEventArgs> WolfPositionChanged;
    public static event EventHandler<SpawnEventArgs> SheepSpawning;
    public static event EventHandler<SpawnEventArgs> WolfSpawning;
    public static event EventHandler<Vector2IntEventArgs> TileClicked;
    public static event EventHandler<Vector2IntEventArgs> TileRightClicked;

    public static void InvokeSheepPositionChanged(Vector2Int positionFrom, Vector2Int positionTo)
    {
        SheepPositionChanged(null, new PositionEventArgs { positionFromPayload = positionFrom,
                                                      positionToPayload = positionTo});
    }

    public static void InvokeWolfPositionChanged(Vector2Int positionFrom, Vector2Int positionTo)
    {
        WolfPositionChanged(null, new PositionEventArgs
        {
            positionFromPayload = positionFrom,
            positionToPayload = positionTo
        });
    }

    public static void InvokeSheepSpawning(Vector2Int spawnPos)
    {
        SheepSpawning(null, new SpawnEventArgs {spawnPosPayload = spawnPos});
    }

    public static void InvokeWolfSpawning(Vector2Int spawnPos)
    {
        WolfSpawning(null, new SpawnEventArgs { spawnPosPayload = spawnPos });
    }

    public static void InvokeTileClicked(Vector2Int clickedTilePos)
    {
        TileClicked(null, new Vector2IntEventArgs { positionPayload = clickedTilePos });
    }

    public static void InvokeTileRightClicked(Vector2Int clickedTilePos)
    {
        TileRightClicked(null, new Vector2IntEventArgs { positionPayload = clickedTilePos });
    }


}
