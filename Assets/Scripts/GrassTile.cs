using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

// I'm not used to designing out an entire script before testing it so sorry in 
// advance if it changes a lot. The idea of this class is to hold the information
// for each grass tile and be able to randomly choose what type it is when the game
// loads. It will also be responsible for naturally advancing the state of the grass
// on the tile. 

// It's IComparable so I can have Unity do some sorting for me to make pathfinding simpler.

public class GrassTile : MonoBehaviour, IComparable<GrassTile>
{
    // First I will give it serializable fields where I will put each tile graphic from
    // the editor. It will also be given it's address on the grid somehow, so it can update
    // it's own image when it changes state, as well as a variable to hold it's state (which
    // will be an enum for convenience.)
    [SerializeField] Tile[] _graphics;
    Vector3Int _tilePos;


    public Vector2Int nodePos;
    public Dictionary<Vector2Int, int> adjacentTiles;
    public bool occupied;
    public bool wolf;
    public bool isIce;
    public bool isSand;
    public GrassState state;

    // this variable is a placeholder to put the current distance to the tile in. That way
    // I can sort using it later.
    public float tempDistance;

    // I will also add a boolean to tell whether something is on the tile or not.

    // Here there will be a void Setup() function that will be called by the TileManager class
    // when this grass tile is instantiated for a tile on the tilemap. It will randomly assign a 
    // level of grass to this object and put the proper graphic on the tilemap location. It will also
    // set up it's tilemap location variable. It will also call the advancestate enum below which will 
    // decide if the grass state should change.

    public void Setup(int xTile, int yTile, int xNode, int yNode) 
    {
        _tilePos = new Vector3Int(xTile, yTile, 0);
        nodePos = new Vector2Int(xNode, yNode);

        float typeRoll = UnityEngine.Random.Range(0f,1f);

        if (typeRoll < .15f)
        {
            state = (GrassState) 0;
            isIce = true;
            SetTileGraphic();
        }
        else if (typeRoll < .3f)
        {
            state = (GrassState) 0;
            isSand = true;
            SetTileGraphic();
        }
        else
        {
            state = (GrassState) UnityEngine.Random.Range(0, 6);
            occupied = false;
            StartCoroutine("AdvanceState");
        }
    }

    // This function implements IComparable so Unity will sort these for me
    // later in Dijkstra.
    public int CompareTo(GrassTile other) 
    {
        if (other.tempDistance > this.tempDistance)
        {
            return -1;
        }
        else if (other.tempDistance < this.tempDistance)
        {
            return 1;
        }
        else 
        {
            return 0;
        }
    }

    // This tells the grasstile to set it's adjacent tiles. It has to be done separately
    // from the Setup function, because it requires all other tiles to have run Setup 
    // (because it needs to know if they're ice tiles.)
    public void SetAdjacentTiles() 
    {
        adjacentTiles = TileManager.GetAdjacentTiles(nodePos);
    }

    // Here I'll add a function to remove an entity from the tile or add an entity to the tile. 

    // Next I'll make some kind of AdvanceState function. I'm thinking it will probably be a coroutine
    // that's called for each grass every few seconds, and it will randomly decide if grass should grow up
    // or if it's already grown if it should wither back to dirt. The enum will be first called in the Setup
    // function above.
    IEnumerator AdvanceState()
    {
        int currState = (int)state;

        if (currState < 5)
        {
            state = (GrassState)currState + 1;
        }
        else
        {
            state = 0;
        }

        SetTileGraphic();

        yield return new WaitForSeconds(UnityEngine.Random.Range(30f, 180f));
        StartCoroutine("AdvanceState");
    }

    public void LowerState() 
    {
        state = (GrassState) ((int) state) - 1;
        SetTileGraphic();
    }

    // Spaced earlier that I should have a display function for each grass so I can conveniently display the 
    // graphic. 

    void SetTileGraphic() 
    {
        if (isIce)
        {
            TileManager.tileMap.SetTile(_tilePos, _graphics[_graphics.Length - 1]);
        }
        else if (isSand)
        {
            TileManager.tileMap.SetTile(_tilePos, _graphics[_graphics.Length - 2]);
        }
        else
        {
            TileManager.tileMap.SetTile(_tilePos, _graphics[(int)state]);
        }
    }

    public int CompareTo(object obj)
    {
        return tempDistance.CompareTo(obj);
    }
}
