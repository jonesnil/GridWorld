using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    // This class will have a tilePosition like the grass tiles, and of course a health stat.
    // it will have another Setup function TileManager will use to get it started after it 
    // creates them.

    int _health;
    int _maxHealth;
    Vector2Int _tilePos;
    Dictionary<Vector2Int, Vector2Int> currentPath;
    Vector2Int currentGoal;

    // This is the implementation of Dijkstra. It's not used by the sheep/bunny's pathfinding
    // normally when they wander around, but if you click a spot on the map this function gives
    // you the path to it. The guide you posted in slack about pathfinding was extremely useful
    // by the way, I followed along with it when I wrote this.
    public Dictionary<Vector2Int, Vector2Int> GetPathDijkstra(Vector2Int goal) 
    {
        currentGoal = goal;

        // I was running a little short on time and didn't want to implement a priority
        // queue so I decided to use a list of GrassTiles and just sort it by a distance
        // value I give it here, removing the first item every time I grab it. It's a little 
        // hacky but I don't think it affects the path quality at all. 
        List<GrassTile> frontier = new List<GrassTile>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        costSoFar[_tilePos] = 0;

        TileManager.grassMap[_tilePos.x, _tilePos.y].tempDistance = 0;
        frontier.Add(TileManager.grassMap[_tilePos.x, _tilePos.y]);

        while (frontier.Count != 0) 
        {
            GrassTile currentPos = frontier[0];
            TileManager.SetTileRed(TileManager.PosToTileMap(currentPos.nodePos));
            frontier.Remove(currentPos);

            if (currentPos.nodePos == goal) 
            {
                break;
            }

            GrassTile currentTile = TileManager.grassMap[currentPos.nodePos.x, currentPos.nodePos.y];

            foreach (Vector2Int adjacentPos in currentTile.adjacentTiles.Keys) 
            {
                float newCost = costSoFar[currentPos.nodePos] + currentTile.adjacentTiles[adjacentPos];

                if (!costSoFar.ContainsKey(adjacentPos) || (newCost < costSoFar[adjacentPos])) 
                {
                    GrassTile adjacentTile = TileManager.grassMap[adjacentPos.x, adjacentPos.y];

                    // This tempDistance thing is just a hacky way to force Unity to do the work
                    // of sorting the tiles for me. I set it here every time I need to consider 
                    // distance, and then I put the tile in the list and sort it. 
                    adjacentTile.tempDistance = newCost;

                    frontier.Add(adjacentTile);
                    frontier.Sort();

                    costSoFar[adjacentPos] = adjacentTile.tempDistance;
                    cameFrom[adjacentPos] = currentPos.nodePos;
                }
            }
        }

        return cameFrom;
    }

    // This is the same as above but I added heuristics based on distance to make it A*.
    public Dictionary<Vector2Int, Vector2Int> GetPathAStar(Vector2Int goal)
    {
        currentGoal = goal;

        // I was running a little short on time and didn't want to implement a priority
        // queue so I decided to use a list of GrassTiles and just sort it by a distance
        // value I give it here, removing the first item every time I grab it. It's a little 
        // hacky but I don't think it affects the path quality at all. 
        List<GrassTile> frontier = new List<GrassTile>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        costSoFar[_tilePos] = 0;

        TileManager.grassMap[_tilePos.x, _tilePos.y].tempDistance = 0;
        frontier.Add(TileManager.grassMap[_tilePos.x, _tilePos.y]);

        while (frontier.Count != 0)
        {
            GrassTile currentPos = frontier[0];
            TileManager.SetTileRed(TileManager.PosToTileMap(currentPos.nodePos));
            frontier.Remove(currentPos);

            if (currentPos.nodePos == goal)
            {
                break;
            }

            GrassTile currentTile = TileManager.grassMap[currentPos.nodePos.x, currentPos.nodePos.y];

            foreach (Vector2Int adjacentPos in currentTile.adjacentTiles.Keys)
            {
                // Here are the heuristic A* changes.
                float heuristic = Vector2.Distance(adjacentPos, goal);
                float newCost = costSoFar[currentPos.nodePos] + currentTile.adjacentTiles[adjacentPos] + heuristic;

                if (!costSoFar.ContainsKey(adjacentPos) || (newCost < costSoFar[adjacentPos]))
                {
                    GrassTile adjacentTile = TileManager.grassMap[adjacentPos.x, adjacentPos.y];

                    // This tempDistance thing is just a hacky way to force Unity to do the work
                    // of sorting the tiles for me. I set it here every time I need to consider 
                    // distance, and then I put the tile in the list and sort it. 
                    adjacentTile.tempDistance = newCost;

                    frontier.Add(adjacentTile);
                    frontier.Sort();

                    costSoFar[adjacentPos] = adjacentTile.tempDistance;
                    cameFrom[adjacentPos] = currentPos.nodePos;
                }
            }
        }

        return cameFrom;
    }
}
