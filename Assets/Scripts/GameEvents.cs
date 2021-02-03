using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PositionEventArgs : EventArgs
{
    public Vector2Int positionFromPayload;
    public Vector2Int positionToPayload;
}

public class GameEvents : MonoBehaviour
{
    public static event EventHandler<PositionEventArgs> PositionChanged;

    public static void InvokePositionChanged(Vector2Int positionFrom, Vector2Int positionTo)
    {
        PositionChanged(null, new PositionEventArgs { positionFromPayload = positionFrom,
                                                      positionToPayload = positionTo});
    }


}
