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
    [SerializeField] Tile _wolfTile;
    [SerializeField] GameObject _grassTilePrefab;
    [SerializeField] GameObject _sheepTilePrefab;
    [SerializeField] GameObject _wolfTilePrefab;
    static int widthStart;
    static int heightStart;

    // I'm going to add a 2D array of GrassTiles to cover the whole map, and I'll index them the same
    // as the Tilemap Tiles and use them to tell where entities are and can move. 

    public static GrassTile[,] grassMap;

    // Start is called before the first frame update. In this function I'll grab all the tiles
    // already initiated and assign them GrassTiles which I'll start rolling with their setup function.
    void Start()
    {
        GameEvents.SheepDestroyed += OnSheepDestroyed;
        GameEvents.SheepPositionChanged += OnSheepPositionChanged;
        GameEvents.WolfPositionChanged += OnWolfPositionChanged;
        GameEvents.SheepSpawning += OnSheepSpawning;
        GameEvents.WolfSpawning += OnWolfSpawning;

        tileMap = GetComponent<Tilemap>();
        entityTileMap = transform.GetChild(0).GetComponent<Tilemap>();

        int width = tileMap.cellBounds.size.x;
        int height = tileMap.cellBounds.size.y;

        grassMap = new GrassTile[width, height];

        widthStart = -(width / 2);
        heightStart = -(height / 2);

        int widthCounter = widthStart;
        int heightCounter = heightStart;

        while (widthCounter < width + widthStart)
        {
            while (heightCounter < height + heightStart)
            {
                GrassTile grassTile = Instantiate(_grassTilePrefab).GetComponent<GrassTile>();
                grassTile.Setup(widthCounter, heightCounter, widthCounter - widthStart, heightCounter - heightStart);
                grassMap[widthCounter - widthStart, heightCounter - heightStart] = grassTile;

                heightCounter += 1;
            }

            heightCounter = heightStart;
            widthCounter += 1;
        }

        foreach (GrassTile tile in grassMap) 
        {
            if (!tile.isIce) 
            {
                tile.SetAdjacentTiles();
            }
        }

        // I will be adding a spawn sheep function and it will be called a few times here to get
        // the simulation started.

        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();
        SpawnRandomSheep();

        SpawnRandomWolf();
        SpawnRandomWolf();

        CheckTiles();
    }

    // I added this so you can hit escape to close the game. 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }

        /*
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseClickPos.z = 0;
            Vector3Int clickedTilePos = tileMap.WorldToCell(mouseClickPos);
            ResetTileColors();

            GameEvents.InvokeTileRightClicked(TileMapToPos(clickedTilePos));
        }
        */
    }

    void CheckTiles()
    {
        foreach (var pos in entityTileMap.cellBounds.allPositionsWithin)
        {
            Vector2Int entityPos = TileMapToPos(pos);
            bool somethingOnSpot = false;

            foreach (Wolf wolf in Wolf.allWolves) 
            {
                if (entityPos == wolf._tilePos)
                    somethingOnSpot = true;
            }

            foreach (Sheep sheep in Sheep.allSheep)
            {
                if (entityPos == sheep._tilePos)
                    somethingOnSpot = true;
            }

            if (!somethingOnSpot) 
            {
                entityTileMap.SetTile(pos, null);
            }
        }
    }

    // This grabs the tile you clicked and gives the information to the sheep with an event.
    /*
    private void OnMouseDown()
    {
        Vector3 mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseClickPos.z = 0;
        Vector3Int clickedTilePos = tileMap.WorldToCell(mouseClickPos);
        ResetTileColors();

        GameEvents.InvokeTileClicked(TileMapToPos(clickedTilePos));
    }
    */


    // These next few functions just deal with changing the tile colors and changing
    // them back to display paths when you click.
    public static void SetTileBlue(Vector3Int pos) 
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, Color.blue);
    }

    public static void SetTileRed(Vector3Int pos)
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, Color.grey);
    }

    public static void ResetTileColor(Vector3Int pos)
    {
        tileMap.SetTileFlags(pos, TileFlags.None);
        tileMap.SetColor(pos, Color.white);
    }

    public static void ResetTileColors()
    {
        int xIndex;
        int yIndex;
        for (xIndex = tileMap.cellBounds.xMin; xIndex < tileMap.cellBounds.xMax; xIndex++)
        {
            for (yIndex = tileMap.cellBounds.yMin; yIndex < tileMap.cellBounds.yMax; yIndex++)
            {
                Vector3Int tilePos = new Vector3Int(xIndex, yIndex, 0);
                ResetTileColor(tilePos);
            }
        }
    }

    // This does what it says on the can. Spawns a sheep at a random location.
    void SpawnRandomSheep() 
    {
        int xPos = Random.Range(0, (-widthStart) * 2);
        int yPos = Random.Range(0, (-heightStart) * 2);

        Vector2Int spawnPos = new Vector2Int(xPos, yPos);

        while (grassMap[spawnPos.x, spawnPos.y].isIce) 
        {
            xPos = Random.Range(0, (-widthStart) * 2);
            yPos = Random.Range(0, (-heightStart) * 2);
            spawnPos = new Vector2Int(xPos, yPos);
        }

        Sheep newSheep = Instantiate(_sheepTilePrefab).GetComponent<Sheep>();
        newSheep.Setup(xPos, yPos, 5);

        entityTileMap.SetTile(PosToTileMap(spawnPos), _sheepTile);
    }

    void SpawnRandomWolf()
    {
        int xPos = Random.Range(0, (-widthStart) * 2);
        int yPos = Random.Range(0, (-heightStart) * 2);

        Vector2Int spawnPos = new Vector2Int(xPos, yPos);

        while (grassMap[spawnPos.x, spawnPos.y].isIce)
        {
            xPos = Random.Range(0, (-widthStart) * 2);
            yPos = Random.Range(0, (-heightStart) * 2);
            spawnPos = new Vector2Int(xPos, yPos);
        }

        Wolf newWolf = Instantiate(_wolfTilePrefab).GetComponent<Wolf>();
        newWolf.Setup(xPos, yPos, 5);

        entityTileMap.SetTile(PosToTileMap(spawnPos), _wolfTile);
    }

    // This reacts to an event called by sheep, and it spawns a new sheep at the suitable
    // location the sheep decided on. 
    void OnSheepSpawning(object sender, SpawnEventArgs args)
    {
        Vector2Int spawnPos = args.spawnPosPayload;

        Sheep newSheep = Instantiate(_sheepTilePrefab).GetComponent<Sheep>();
        newSheep.Setup(spawnPos.x, spawnPos.y, 2);
        entityTileMap.SetTile(PosToTileMap(spawnPos), _sheepTile);
    }

    void OnWolfSpawning(object sender, SpawnEventArgs args)
    {
        Vector2Int spawnPos = args.spawnPosPayload;

        Wolf newWolf = Instantiate(_wolfTilePrefab).GetComponent<Wolf>();
        newWolf.Setup(spawnPos.x, spawnPos.y, 2);
        entityTileMap.SetTile(PosToTileMap(spawnPos), _wolfTile);
    }

    // This converts an index on my GrassTile 2D array to its actual location on the
    // tilemap.
    public static Vector3Int PosToTileMap(Vector2Int pos) 
    {
        Vector3Int output = new Vector3Int(pos.x + widthStart, pos.y + heightStart, 0);
        return output;
    }

    // This is the opposite of the above.
    Vector2Int TileMapToPos(Vector3Int pos)
    {
        Vector2Int output = new Vector2Int(pos.x - widthStart, pos.y - heightStart);
        return output;
    }

    // This erases a sheep from it's previous spot and places it at its new spot 
    // every time it moves. It listens to an event that is called by every sheep when
    // they move, and the sheep give the event where they came from and where they're 
    // going.
    void OnSheepPositionChanged(object sender, PositionEventArgs args)
    {
        Vector2Int posFrom = args.positionFromPayload;
        Vector2Int posTo = args.positionToPayload;

        entityTileMap.SetTile(PosToTileMap(posFrom), null);
        entityTileMap.SetTile(PosToTileMap(posTo), _sheepTile);
    }

    void OnWolfPositionChanged(object sender, PositionEventArgs args)
    {
        Vector2Int posFrom = args.positionFromPayload;
        Vector2Int posTo = args.positionToPayload;

        entityTileMap.SetTile(PosToTileMap(posFrom), null);
        entityTileMap.SetTile(PosToTileMap(posTo), _wolfTile);
    }

    void OnSheepDestroyed(object sender, Vector2IntEventArgs args)
    {
        Vector2Int posDeleted = args.positionPayload;

        entityTileMap.SetTile(PosToTileMap(posDeleted), null);
    }

    // This function returns true if a position is actually on the visible grid and false
    // if it's not. It's used by a lot of functions here to prevent trying to grab something
    // that's not on the grid and getting an error. It also prevents sheep from moving off 
    // the screen.
    static bool MoveWorks(Vector2Int move, int widthLength, int heightLength)
    {
        if (move.x >= 0 && move.x < widthLength && move.y >= 0 && move.y < heightLength && !grassMap[move.x, move.y].isIce)
            return true;
        else
            return false;
    }

    // This function checks the four possible spots for adjacent grass tiles and
    // returns the ones actually there (not off the screen.)
    public static Dictionary<Vector2Int, int> GetAdjacentTiles(Vector2Int pos)
    {
        Dictionary<Vector2Int, int> output = new Dictionary<Vector2Int, int>();
        List<Vector2Int> checkMoves = new List<Vector2Int>();

        checkMoves.Add(new Vector2Int(pos.x, pos.y - 1));
        checkMoves.Add(new Vector2Int(pos.x, pos.y + 1));
        checkMoves.Add(new Vector2Int(pos.x - 1, pos.y));
        checkMoves.Add(new Vector2Int(pos.x + 1, pos.y));

        int widthLength = grassMap.GetLength(0);
        int heightLength = grassMap.GetLength(1);

        foreach (Vector2Int move in checkMoves)
        {
            if (MoveWorks(move, widthLength, heightLength))
            {
                if (grassMap[move.x, move.y].isSand) 
                {
                    output.Add(move, 4);
                }
                else
                {
                    output.Add(move, 1);
                }
            }
        }

        return output;
    }
}
