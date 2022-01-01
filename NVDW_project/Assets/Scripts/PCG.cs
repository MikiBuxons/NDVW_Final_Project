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

    public GameObject finish;

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

    public int minTilesEnem = 2;
    public int spawnEnemFactor = 10;
    public List<SpawnProb> enemies;

    public int minTilesTrap = 5;
    public int spawnTrapFactor = 10;
    public List<SpawnProb> traps;

    [System.Serializable]
    public class SpawnProb
    {
        public int prob;
        public GameObject prefab;
    }

    public void Start()
    {
        List<List<SpawnProb>> spawnProbs = new List<List<SpawnProb>>();
        spawnProbs.Add(enemies);
        spawnProbs.Add(traps);
        foreach (var spawnlist in spawnProbs)
        {
            var prob = 0;
            foreach (var entity in spawnlist)
            {
                entity.prob += prob;
                prob = entity.prob;
            }
        }

        Init();

        // Limits of the map
        var botLeft = terrain.CellToWorld(new Vector3Int(0, 0, 0));
        var botRight = terrain.CellToWorld(new Vector3Int(mapSizeX - 1, 0, 0));
        var topRight = terrain.CellToWorld(new Vector3Int(mapSizeX - 1, mapSizeY, 0));
        var topLeft = terrain.CellToWorld(new Vector3Int(0, mapSizeY, 0));

        Vector2[] confLims = { botLeft, botRight, topRight, topLeft };

        confinement.pathCount = 1;
        confinement.SetPath(0, confLims);

        background.size = new Vector2(mapSizeX, mapSizeY) / 2;
        background.transform.position += new Vector3(mapSizeX, mapSizeY, 0) / 2;

        deathZone.size = new Vector2(mapSizeX, 10);
        deathZone.transform.position += new Vector3(mapSizeX / 2, -5, 0);

        //Invoke("BuildLevel", 20.0f);
        //Invoke("Init", 40.0f);
    }

    public void Init()
    {
        var spawn = new Vector3Int(2, Random.Range(3, mapSizeY - 5), 0);
        spawnPoint.position = terrain.CellToWorld(spawn);
        spawnPoint.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y + 3, 0);

        map = new float[mapSizeX, mapSizeY];

        // Build map
        GenTerrain(spawn);
        AddTraps(spawn);
        AddEnemies(spawn);
        FillTerrain();

        BuildLevel();

        // Astar grid configuration
        foreach (GridGraph graph in AstarPath.active.data.GetUpdateableGraphs())
        {
            var w = mapSizeX / 0.2;
            var h = mapSizeY / 0.2;
            graph.SetDimensions((int)w, (int)h, 0.2f);
            graph.center = new Vector3(mapSizeX / 2, (mapSizeY / 2) + 0, 0);
        }

        player.position = spawnPoint.position;

        Invoke("Scan", 2.0f);

        //Invoke("BuildLevel", 20.0f);
    }

    public void BuildLevel()
    {
        terrain.ClearAllTiles();
        var clones = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var clone in clones)
        {
            Destroy(clone);
        }

        var finishClones = GameObject.FindGameObjectsWithTag("Finish");
        foreach (var clone in finishClones)
        {
            Destroy(clone);
        }
        // Build level from map content
        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 1 || map[x, y] == 2)
                {
                    terrain.SetTile(new Vector3Int(x, y, 0), tile);
                }
                if (map[x, y] > 10)
                {
                    SpawnEntity((int)map[x, y], x, y);
                }
                if (map[x, y] == 6)
                {
                    Vector3 pos = terrain.CellToWorld(new Vector3Int(x, y, 0));
                    pos = new Vector3(pos.x, pos.y + 0.8f, 0);
                    Instantiate(finish, pos, Quaternion.identity);
                }
                if (x == 0 || x == map.GetLength(0) - 1)
                {
                    terrain.SetTile(new Vector3Int(x + ((x == 0) ? -1 : 0), y, 0), limitTile);
                }
            }
        }
    }

    int SelectEntityFrom(List<SpawnProb> entities, int x, int y, int offset)
    {
        var entitySel = Random.Range(0, 100);
        for (int i = 0; i < entities.Count; i++)
        {
            SpawnProb entity = entities[i];
            if (entitySel < entity.prob)
            {
                return i + offset;
            }
        }
        return 0;
    }

    void SpawnEntity(int val, int x, int y)
    {
        Debug.Log("SPAWN"+val);
        if (val - 40 >= 0)
        {
            SpawnEntityFrom(traps, x, y, (val - 40));
        }
        else if (val - 30 >= 0)
        {
            SpawnEntityFrom(enemies, x, y + 1, (val - 30));
        }
    }

    void SpawnEntityFrom(List<SpawnProb> entities, int x, int y, int i)
    {
        SpawnProb entity = entities[i];
        var pos = terrain.CellToWorld(new Vector3Int(x, y, 0));
        Instantiate(entity.prefab, pos, Quaternion.identity);
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

    void AddToPlatform(Vector3Int spawn, int minTiles, int spawnFact, int val, int fill = -1)
    {
        var start = spawn.y + 3;
        for (var y = 0; y < map.GetLength(1); y++)
        {
            var count = 0;
            for (var x = start; x < map.GetLength(0); x++)
            {
                if (map[x, y] == 1)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count > minTiles)
                {
                    var enem = Random.Range(0, 100);
                    if (enem < count * spawnFact)
                    {
                        if (map[x, y + 1] == 0)
                        {
                            map[x, y + 1] = val;
                            if (fill != -1)
                            {
                                for (var i = 1; i < count; i++)
                                {
                                    map[x - i, y + 1] = fill;
                                }
                            }
                            count = 0;
                        }
                    }
                }
            }
        }
    }

    void AddTraps(Vector3Int spawn)
    {
        AddToPlatform(spawn, minTilesTrap, spawnTrapFactor, 4);
        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 4)
                {
                    map[x, y] = SelectEntityFrom(traps, x, y, 40);
                }
            }
        }
    }

    void AddEnemies(Vector3Int spawn)
    {
        AddToPlatform(spawn, minTilesEnem, spawnEnemFactor, 3);
        for (var x = 0; x < map.GetLength(0); x++)
        {
            for (var y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 3)
                {
                    map[x, y] = SelectEntityFrom(enemies, x, y, 30);
                }
            }
        }
    }

    void GenTerrain(Vector3Int spawn)
    {
        var current = new Vector3Int(Random.Range(minRoomSize, maxRoomSize), spawn.y - 1, 0);

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
        // TODO add finish
        map[current.x - 1, current.y + 1] = 6;
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
