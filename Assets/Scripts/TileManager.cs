using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class's purpose is to grab all the tiles I want to be active and create a grid. Then
// when I have sheep/wolves it can use the grid to give them information on where they can move.
// This class will also give every appropriate tile a GrassTile object which will handle whether
// the grass is grown or should grow, and this class will get it started by calling the Setup function
// I put on it. 

// This class will be on the Tilemap object in Unity when I apply it and finish coding it. It will be 
// a singleton that the sheep/wolves should find very convenient for sensing the world. 

public class TileManager : MonoBehaviour
{
    public static Tilemap tileMap;
    TileBase[] _tiles;
    [SerializeField] GameObject _grassTilePrefab;

    // Start is called before the first frame update. In this function I'll grab all the tiles
    // already initiated and assign them GrassTiles which I'll start rolling with their setup function.
    void Start()
    {
        tileMap = GetComponent<Tilemap>();

        int width = tileMap.cellBounds.size.x;
        int height = tileMap.cellBounds.size.y;

        int widthStart = -(width/2);
        int heightStart = -(height/2);

        int widthCounter = widthStart;
        int heightCounter = heightStart;

        while (widthCounter < width + widthStart) 
        {
            while (heightCounter < height + heightStart) 
            {
                GrassTile grassTile = Instantiate(_grassTilePrefab).GetComponent<GrassTile>();
                grassTile.Setup(widthCounter, heightCounter);

                heightCounter += 1;
            }

            heightCounter = heightStart;
            widthCounter += 1;
        }
    }

    // I'm kind of designing parts one at a time but eventually I think I will give this class functions
    // like CheckForWolves/Grass/Sheep(Location) to make implementing those classes easier. 
}
