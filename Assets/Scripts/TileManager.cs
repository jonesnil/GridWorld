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
    Tilemap entityTileMap;
    TileBase[] _tiles;
    [SerializeField] Tile _sheepTile;
    [SerializeField] GameObject _grassTilePrefab;
    [SerializeField] GameObject _sheepTilePrefab;
    int widthStart;
    int heightStart;

    // I'm going to add a 2D array of GrassTiles to cover the whole map, and I'll index them the same
    // as the Tilemap Tiles and use them to tell where entities are and can move. 

    public static GrassTile[,] grassMap;

    // Start is called before the first frame update. In this function I'll grab all the tiles
    // already initiated and assign them GrassTiles which I'll start rolling with their setup function.
    void Start()
    {
        GameEvents.PositionChanged += OnPositionChanged;

        tileMap = GetComponent<Tilemap>();
        entityTileMap = transform.GetChild(0).GetComponent<Tilemap>();

        int width = tileMap.cellBounds.size.x;
        int height = tileMap.cellBounds.size.y;

        grassMap = new GrassTile[width , height];

        widthStart = -(width/2);
        heightStart = -(height/2);

        int widthCounter = widthStart;
        int heightCounter = heightStart;

        while (widthCounter < width + widthStart) 
        {
            while (heightCounter < height + heightStart) 
            {
                GrassTile grassTile = Instantiate(_grassTilePrefab).GetComponent<GrassTile>();
                grassTile.Setup(widthCounter, heightCounter);
                grassMap[widthCounter - widthStart, heightCounter - heightStart] = grassTile;

                heightCounter += 1;
            }

            heightCounter = heightStart;
            widthCounter += 1;
        }

        // I will be adding a spawn sheep function and it will be called a few times here to get
        // the simulation started.

        SpawnSheep();
        SpawnSheep();
        SpawnSheep();
        SpawnSheep();
        SpawnSheep();
        SpawnSheep();
    }

    void SpawnSheep() 
    {
        int xPos = Random.Range(0, (-widthStart) * 2);
        int yPos = Random.Range(0, (-heightStart) * 2);

        Sheep newSheep = Instantiate(_sheepTilePrefab).GetComponent<Sheep>();
        newSheep.Setup(xPos, yPos);
    }

    Vector2Int PosToTileMap(Vector2Int pos) 
    {
        Vector2Int output = new Vector2Int(pos.x + widthStart, pos.y + heightStart);
        return output;
    }

    void OnPositionChanged(object sender, PositionEventArgs args)
    {
        Vector2Int posFrom = args.positionFromPayload;
        Vector2Int posTo = args.positionToPayload;

        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(posFrom)), null);
        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(posTo)), _sheepTile);
    }

    Vector3Int Vec2IntToVec3Int(Vector2Int input) 
    {
        Vector3Int output = new Vector3Int(input.x, input.y, 0);
        return output;
    }

    // Insert spawn sheep function

    // I'm kind of designing parts one at a time but eventually I think I will give this class functions
    // like CheckForWolves/Grass/Sheep(Location) to make implementing those classes easier. 
}
