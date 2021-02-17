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
    public static event EventHandler<PositionEventArgs> PositionChanged;
    public static event EventHandler<SpawnEventArgs> SheepSpawning;
    public static event EventHandler<Vector2IntEventArgs> TileClicked;

    public static void InvokePositionChanged(Vector2Int positionFrom, Vector2Int positionTo)
    {
        PositionChanged(null, new PositionEventArgs { positionFromPayload = positionFrom,
                                                      positionToPayload = positionTo});
    }

    public static void InvokeSheepSpawning(Vector2Int spawnPos)
    {
        SheepSpawning(null, new SpawnEventArgs {spawnPosPayload = spawnPos});
    }

    public static void InvokeTileClicked(Vector2Int clickedTilePos)
    {
        TileClicked(null, new Vector2IntEventArgs { positionPayload = clickedTilePos });
    }


}
