using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Here there will be a void Setup() function that will be called by the TileManager class
    // when this grass tile is instantiated for a tile on the tilemap. It will randomly assign a 
    // level of grass to this object and put the proper graphic on the tilemap location. It will also
    // set up it's tilemap location variable. It will also call the advancestate enum below which will 
    // decide if the grass state should change.

    // Next I'll make some kind of AdvanceState function. I'm thinking it will probably be a coroutine
    // that's called for each grass every few seconds, and it will randomly decide if grass should grow up
    // or if it's already grown if it should wither back to dirt. The enum will be first called in the Setup
    // function above.
}
