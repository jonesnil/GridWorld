using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// I'm not used to designing out an entire script before testing it so sorry in 
// advance if it changes a lot. The idea of this class is to hold the information
// for each grass tile and be able to randomly choose what type it is when the game
// loads. It will also be responsible for naturally advancing the state of the grass
// on the tile. 

public class GrassTile : MonoBehaviour
{
    // First I will give it serializable fields where I will put each tile graphic from
    // the editor. It will also be given it's address on the grid somehow, so it can update
    // it's own image when it changes state, as well as a variable to hold it's state (which
    // will be an enum for convenience.)
    [SerializeField] Tile[] _graphics;
    GrassState _state;
    Vector3Int _tilePos;

    // I will also add a boolean to tell whether something is on the tile or not.

    // Here there will be a void Setup() function that will be called by the TileManager class
    // when this grass tile is instantiated for a tile on the tilemap. It will randomly assign a 
    // level of grass to this object and put the proper graphic on the tilemap location. It will also
    // set up it's tilemap location variable. It will also call the advancestate enum below which will 
    // decide if the grass state should change.

    public void Setup(int x, int y) 
    {
        _tilePos = new Vector3Int(x, y, 0);
        _state = (GrassState) Random.Range(0, 6);

        StartCoroutine("AdvanceState");
    }

    // Here I'll add a function to remove an entity from the tile or add an entity to the tile. 

    // Next I'll make some kind of AdvanceState function. I'm thinking it will probably be a coroutine
    // that's called for each grass every few seconds, and it will randomly decide if grass should grow up
    // or if it's already grown if it should wither back to dirt. The enum will be first called in the Setup
    // function above.
    IEnumerator AdvanceState()
    {
        int currState = (int)_state;

        if (currState < 5)
        {
            _state = (GrassState)currState + 1;
        }
        else
        {
            _state = 0;
        }

        SetTileGraphic();

        yield return new WaitForSeconds(Random.Range(30f, 180f));
        StartCoroutine("AdvanceState");
    }


    // Spaced earlier that I should have a display function for each grass so I can conveniently display the 
    // graphic. 

    void SetTileGraphic() 
    {
        TileManager.tileMap.SetTile(_tilePos, _graphics[(int)_state]);
    }
}
