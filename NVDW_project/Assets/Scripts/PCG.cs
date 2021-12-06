using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class PCG : MonoBehaviour
{
    public RuleTile tile;
    public RuleTile limitTile;

    public Tilemap terrain;

    float[,] map;

    public Transform spawnPoint;
    public Transform player;

    public PolygonCollider2D confinement;
    public SpriteRenderer background;
    public BoxCollider2D deathZone;

    public int mapSizeX = 100;
    public int mapSizeY = 25;

    public int maxRoomSize = 8;
    private int minRoomSize = 3;

    public int minJump = 2;
    public int maxJump = 4;

    public int roomProb = 50;
    public int jumpProb = 30;
    public int wallJumpProb = 20;

    public int maxWallJumpDist = 5;
    private int minWallJumpDist = 4;
    public int maxWallJumpHeight = 10;

    void Start()
    {
        var spawn = new Vector3Int(2, Random.Range(3, mapSizeY - 5), 0);
        spawnPoint.position = terrain.CellToWorld(spawn);
        spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 3, 0);

        map = new float[mapSizeX, mapSizeY];

        var current = new Vector3Int(Random.Range(minRoomSize, maxRoomSize), spawn.y - 1, 0);

        GenTerrain(current);
        FillTerrain();

        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != 0)
                {
                    terrain.SetTile(new Vector3Int(x, y, 0), tile);
                }
                if (x == 0 || x == map.GetLength(0) - 1)
                {
                    terrain.SetTile(new Vector3Int(x + ((x == 0) ? -1 : 0), y, 0), limitTile);
                }
            }
        }

        var botLeft = terrain.CellToWorld(new Vector3Int(0, 0, 0));
        var botRight = terrain.CellToWorld(new Vector3Int(mapSizeX-1, 0, 0));
        var topRight = terrain.CellToWorld(new Vector3Int(mapSizeX-1, mapSizeY, 0));
        var topLeft = terrain.CellToWorld(new Vector3Int(0, mapSizeY, 0));

        Vector2[] confLims = { botLeft, botRight, topRight, topLeft };

        confinement.pathCount = 1;
        confinement.SetPath(0, confLims);

        background.size = new Vector2(mapSizeX, mapSizeY)/2;
        background.transform.position += new Vector3(mapSizeX, mapSizeY, 0)/2;

        deathZone.size = new Vector2(mapSizeX, 10);
        deathZone.transform.position += new Vector3(mapSizeX/2, -5, 0);

        foreach (GridGraph graph in AstarPath.active.data.GetUpdateableGraphs())
        {
            var w = mapSizeX / 0.2;
            var h = mapSizeY / 0.2;
            graph.SetDimensions((int)w, (int)h, 0.2f);
            graph.center = new Vector3(mapSizeX / 2, (mapSizeY / 2) + 0, 0);
        }

        player.position = spawnPoint.position;

        Invoke("Scan", 2.0f);
    }

    void FillTerrain()
    {
        for (var x = 0; x < map.GetLength(0); x++)
        {
            var groundFound = false;
            for (var y = map.GetLength(1) - 1; y >= 0; y--)
            {
                if (groundFound)
                {
                    map[x, y] = 1;
                }
                else
                {
                    groundFound = map[x, y] == 1;
                }
            }
        }
    }

    void GenTerrain(Vector3Int current)
    {
        // initial room
        for (var x = 0; x < current.x; x++)
        {
            map[x, current.y] = 1;
        }

        var prev = "start";
        while (current.x < mapSizeX - 10)
        {
            int room = Random.Range(0, 100);
            switch (room)
            {
                case int n when (n <= roomProb):
                    if (prev == "room") break;
                    current = GenRoom(current);
                    prev = "room";
                    break;

                case int n when (n <= roomProb + jumpProb):
                    if (prev == "jump") break;
                    current = GenJump(current);
                    prev = "jump";
                    break;
                case int n when (n <= roomProb + jumpProb + wallJumpProb):
                    if (prev == "wallJump") break;
                    current = GenWallJump(current);
                    prev = "wallJump";
                    break;
                default:
                    break;
            }
        }
        current = GenRoom(current, mapSizeX - current.x - 1); // always finish in room to place goal
    }

    Vector3Int GenAddDot(Vector3Int current, int dist = 1) // helps fitting
    {
        for (var i = 0; i < dist && current.x < mapSizeX; i++)
        {
            map[current.x, current.y] = 1;
            current += new Vector3Int(1, 0, 0);
        }
        return current;
    }

    Vector3Int GenRoom(Vector3Int current, int size = 0)
    {
        if (size == 0)
        {
            size = Random.Range(minRoomSize, maxRoomSize);
        }
        for (var x = current.x; x < current.x + size && x < mapSizeX - 1; x++)
        {
            map[x, current.y] = 1;
        }
        current += new Vector3Int(size, 0, 0);
        return current;
    }

    Vector3Int GenJump(Vector3Int current)
    {
        int minJumpWidth, maxJumpWidth, minJumpHeight, maxJumpHeight;
        if (Random.Range(0, 1) == 0) // horizontal
        {
            minJumpWidth = 0;
            maxJumpWidth = 2;
            minJumpHeight = minJump;
            maxJumpHeight = maxJump;
        }
        else // vertical
        {
            minJumpWidth = minJump;
            maxJumpWidth = maxJump;
            minJumpHeight = 0;
            maxJumpHeight = 2;
        }
        var width = Random.Range(minJumpWidth, maxJumpWidth);
        var height = Random.Range(minJumpHeight, maxJumpHeight);
        if ((Random.Range(0, 1) == 0) && (current.y - height > 0 || current.y + height > mapSizeY - 2)) //down
        {
            height *= -1;
        }
        current += new Vector3Int(width, height, 0);
        current = GenAddDot(current, 2);
        return current;
    }

    Vector3Int GenWallJump(Vector3Int current)
    {
        var distToOpposite = Random.Range(minWallJumpDist, maxWallJumpDist);
        var wallHeight = Random.Range(maxJump + 1, maxWallJumpHeight);

        if ((Random.Range(0, 1) == 0)&&(current.y - wallHeight > 0 || current.y + wallHeight > mapSizeY - 2)) //down
        {
            if (wallHeight > maxJump)
            {
                wallHeight = maxJump;
            }
            for (var y = current.y; y > current.y - wallHeight; y--)
            {
                map[current.x, y] = 1;
            }
            current -= new Vector3Int(0, wallHeight, 0);
            if (current.y < 0)
            {
                current = new Vector3Int(current.x, 0, 0);
            }
        }
        else
        {
            current = GenAddDot(current);
            current -= new Vector3Int(1, 0, 0);
            for (var y = current.y; y < current.y + wallHeight; y++)
            {
                if (y > current.y + 3)
                {
                    map[current.x, y] = 2; // NOT 1, to denote possible ground below
                }
                map[current.x + distToOpposite, y] = 1;
            }
            current += new Vector3Int(distToOpposite + 1, wallHeight - 1, 0);
            current = GenAddDot(current);
        }

        return current;
    }

    void Update()
    {

    }

    void Scan()
    {
        AstarPath.active.Scan();
    }
}
