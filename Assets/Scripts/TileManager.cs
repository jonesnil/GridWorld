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

    // I decided to make the tileMap and the GrassTile 2D array static so the sheep/grass
    // could use them without needing a reference to the TileManager. I didn't want to because
    // it's kind of janky but it seemed more jank to have TileManager decide for each object what
    // it should do and I didn't want to have to give all my objects a direct reference to this 
    // or make a scriptable object or something to hold all that information so it was a compromise.

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
        GameEvents.SheepSpawning += OnSheepSpawning;

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

        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();
    }

    // This does what it says on the can. Spawns a sheep at a random location.
    void SpawnRandomSheep() 
    {
        int xPos = Random.Range(0, (-widthStart) * 2);
        int yPos = Random.Range(0, (-heightStart) * 2);

        Sheep newSheep = Instantiate(_sheepTilePrefab).GetComponent<Sheep>();
        newSheep.Setup(xPos, yPos, 5);
        Vector2Int spawnPos = new Vector2Int(xPos, yPos);
        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(spawnPos)), _sheepTile);
    }
    
    // This reacts to an event called by sheep, and it spawns a new sheep at the suitable
    // location the sheep decided on. 
    void OnSheepSpawning(object sender, SpawnEventArgs args)
    {
        Vector2Int spawnPos = args.spawnPosPayload;

        Sheep newSheep = Instantiate(_sheepTilePrefab).GetComponent<Sheep>();
        newSheep.Setup(spawnPos.x, spawnPos.y, 2);
        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(spawnPos)), _sheepTile);
    }

    // This converts an index on my GrassTile 2D array to its actual location on the
    // tilemap.
    Vector2Int PosToTileMap(Vector2Int pos) 
    {
        Vector2Int output = new Vector2Int(pos.x + widthStart, pos.y + heightStart);
        return output;
    }

    // This erases a sheep from it's previous spot and places it at its new spot 
    // every time it moves. It listens to an event that is called by every sheep when
    // they move, and the sheep give the event where they came from and where they're 
    // going.
    void OnPositionChanged(object sender, PositionEventArgs args)
    {
        Vector2Int posFrom = args.positionFromPayload;
        Vector2Int posTo = args.positionToPayload;

        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(posFrom)), null);
        entityTileMap.SetTile(Vec2IntToVec3Int(PosToTileMap(posTo)), _sheepTile);
    }

    // This puts a 0 on the end of a Vector2Int to make it a Vector3 because I'm too
    // lazy to write 0 where it's pointless information. Unity tilemaps take Vector3s
    // for some reason so it's a necessary step.
    Vector3Int Vec2IntToVec3Int(Vector2Int input) 
    {
        Vector3Int output = new Vector3Int(input.x, input.y, 0);
        return output;
    }
}
