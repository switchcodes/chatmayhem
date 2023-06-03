using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteProceduralGeneration : MonoBehaviour
{
    [SerializeField] int width, height;

    [SerializeField] float smoothness;

    [SerializeField] float seed;

    [SerializeField] TileBase groundTile;

    [SerializeField] Tilemap[] groundTilemaps;

    [SerializeField] private Transform player;

    [SerializeField] private GameObject[] enemies;

    [SerializeField] private int enemySpawnHeight;

    private int[,] _map;

    private int _lastPlayerX = 0;

    private int _currentTilemap = 0;

    // Start is called before the first frame update
    private void Start()
    {
        seed = Random.Range(-1000, 1000);
        Generation();
    }

    void Update()
    {
        var halfWidth = Mathf.FloorToInt(width / 2);
        if (!(player.position.x + 10 > _lastPlayerX + halfWidth) &&
            !(player.position.x - 10 < _lastPlayerX - halfWidth)) return;
        _lastPlayerX = Mathf.RoundToInt(player.position.x);
        Generation();
    }

    public void OnRegenerateWorld()
    {
        seed = Random.Range(-1000, 1000);
        Generation();
    }

    private void Generation()
    {
        _currentTilemap = (_currentTilemap + 1) % groundTilemaps.Length;
        var groundTilemap = groundTilemaps[_currentTilemap];
        _map = GenerateArray(width, height, true);
        _map = TerrainGeneration(_map);
        RenderMap(_map, groundTilemap, groundTile);

        for (var i = groundTilemaps.Length - 1; i >= 0; i--)
        {
            if (i == _currentTilemap) continue;
            groundTilemaps[i].ClearAllTiles();
        }
    }

    private static int[,] GenerateArray(int lWidth, int lHeight, bool empty)
    {
        var map = new int[lWidth, lHeight];

        for (var x = 0; x < lWidth; x++)
        {
            for (var y = 0; y < lHeight; y++)
            {
                map[x, y] = empty ? 0 : 1;
            }
        }

        return map;
    }

    private int[,] TerrainGeneration(int[,] map)
    {
        for (var y = 0; y < height; y++)
        {
            map[0, y] = 1;
        }

        for (var x = 1; x < width; x++)
        {
            var perlinHeight =
                Mathf.RoundToInt(Mathf.PerlinNoise(seed, (player.position.x + x) / smoothness) * (height - 5));
            if (perlinHeight > enemySpawnHeight)
            {
                // spawn enemies
                var enemy = enemies[Random.Range(0, enemies.Length)];
                var enemyPos = new Vector3((player.position.x + x) - width / 2, perlinHeight + Random.Range(1, 10), 0);
                var enemyObject = Instantiate(enemy, enemyPos, Quaternion.identity);
                enemyObject.GetComponent<AIChase>().player = player;
            }

            for (var y = 0; y < perlinHeight; y++)
            {
                map[x, y] = 1;
            }

            map[x, height - 1] = 1;
        }

        return map;
    }

    private void RenderMap(int[,] map, Tilemap tilemap, TileBase groundTileBase)
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (map[x, y] != 1) continue;
                var currentPos = player.position;
                var currentX = Mathf.FloorToInt(currentPos.x);
                tilemap.SetTile(new Vector3Int((currentX - width / 2) + x, y, 0), groundTileBase);
            }
        }
    }
}