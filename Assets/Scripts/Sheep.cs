using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    // This class will have a tilePosition like the grass tiles, and of course a health stat.
    // it will have another Setup function TileManager will use to get it started after it 
    // creates them.

    // Also like the grass tiles it will have a coroutine where it takes an action on a loop.
    // It will consider taking an action every 2-5 seconds (not sure what will feel right yet.)
    // I think if it has low health it will make it's priority eating grass, and if not it will 
    // prioritize standing by other sheep. If it's not hungry and a sheep is close it will just
    // wander a bit. It will also wander if it has a need but nothing to fulfill it is close enough
    // to see. It will ask TileManager for information about it's environment as it needs it.
    // In this coroutine it will also check if the prequisites for breeding are met and if so that
    // will happen automatically. 

}
