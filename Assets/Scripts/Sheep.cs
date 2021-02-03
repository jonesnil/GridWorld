using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    // This class will have a tilePosition like the grass tiles, and of course a health stat.
    // it will have another Setup function TileManager will use to get it started after it 
    // creates them.

    int _health;
    int _maxHealth;
    Vector2Int _tilePos;

    public void Setup(int x, int y)
    {
        _tilePos = new Vector2Int(x, y);
        _health = 5;
        _maxHealth = 10;

        StartCoroutine("AILoop");
    }

    // Also like the grass tiles it will have a coroutine where it takes an action on a loop.
    // It will consider taking an action every 2-5 seconds (not sure what will feel right yet.)
    // I think if it has low health it will make it's priority eating grass, and if not it will 
    // prioritize standing by other sheep. If it's not hungry and a sheep is close it will just
    // wander a bit. It will also wander if it has a need but nothing to fulfill it is close enough
    // to see. It will ask TileManager for information about it's environment as it needs it.
    // In this coroutine it will also check if the prequisites for breeding are met and if so that
    // will happen automatically.
    
    IEnumerator AILoop() 
    {
        GrassTile currGrass = TileManager.grassMap[_tilePos.x, _tilePos.y];

        if (_health < _maxHealth) 
        {
            if ((int) currGrass.state > 0)
            {
                currGrass.LowerState();
                this._health += 1;
            }

            else
                Wander();
        }

        else
            Wander();

        yield return new WaitForSeconds(.5f);
        StartCoroutine("AILoop");
    }

    public void Wander() 
    {
        List<Vector2Int> moves = GetPossibleMoves(_tilePos);
        if (moves.Count != 0)
        {
            Vector2Int move = moves[Random.Range(0, moves.Count)];
            GameEvents.InvokePositionChanged(_tilePos, move);

            TileManager.grassMap[move.x, move.y].occupied = true;
            TileManager.grassMap[_tilePos.x, _tilePos.y].occupied = false;

            _tilePos = move;
        }
    }

    List<Vector2Int> GetPossibleMoves(Vector2Int pos)
    {
        List<Vector2Int> output = new List<Vector2Int>();
        List<Vector2Int> checkMoves = new List<Vector2Int>();
        checkMoves.Add(new Vector2Int(pos.x, pos.y + 1));
        checkMoves.Add(new Vector2Int(pos.x, pos.y - 1));
        checkMoves.Add(new Vector2Int(pos.x - 1, pos.y));
        checkMoves.Add(new Vector2Int(pos.x + 1, pos.y));

        int widthLength = TileManager.grassMap.GetLength(0);
        int heightLength = TileManager.grassMap.GetLength(1);

        foreach (Vector2Int move in checkMoves)
        {
            if (MoveWorks(move, widthLength, heightLength))
            {
                output.Add(move);
            }
        }

        return output;
    }

    bool MoveWorks(Vector2Int move, int widthLength, int heightLength)
    {
        if (move.x >= 0 && move.x < widthLength && move.y >= 0 && move.y < heightLength)
            if (!TileManager.grassMap[move.x, move.y].occupied)
                return true;
            else
                return false;
        else
            return false;
    }
}
