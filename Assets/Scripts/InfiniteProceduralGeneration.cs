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
    
    [SerializeField] private int enemySpawnChance = 80;
    
    [SerializeField] private int pickupSpawnChance = 10;
    
    [SerializeField] private GameObject[] pickups;

    [SerializeField] private GameObject backgroundPrefab;

    private int[,] _map;

    private int _lastPlayerX = 0;

    private int _currentTilemap = 0;

    private float _backgroundWidth = 39.09f;
    
    private GameObject[] _currentBackgrounds = new GameObject[5];
    // Start is called before the first frame update
    private void Start()
    {
        var currentX = player.position.x;
        var rotation = Quaternion.Euler(0, -90, 0);
        _currentBackgrounds[0] = Instantiate(backgroundPrefab, new Vector3(currentX, 0, 0), rotation);
        _currentBackgrounds[1] = Instantiate(backgroundPrefab, new Vector3(currentX + _backgroundWidth, 0, 0), rotation);
        _currentBackgrounds[2] = Instantiate(backgroundPrefab, new Vector3(currentX + (_backgroundWidth * 2), 0, 0), rotation);
        _currentBackgrounds[3] = Instantiate(backgroundPrefab, new Vector3(currentX - _backgroundWidth, 0, 0), rotation);
        _currentBackgrounds[4] = Instantiate(backgroundPrefab, new Vector3(currentX - (_backgroundWidth * 2), 0, 0), rotation);
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
                var spawn = Random.Range(0, 100);
                if (spawn < enemySpawnChance)
                {
                    // spawn enemies
                    var enemy = enemies[Random.Range(0, enemies.Length)];
                    var enemyPos = new Vector3((player.position.x + x) - width / 2, perlinHeight + Random.Range(1, 10), 0);
                    var enemyObject = Instantiate(enemy, enemyPos, Quaternion.identity);
                    enemyObject.GetComponent<AIChase>().player = player;
                }
                
            }
            else
            {
                var spawn = Random.Range(0, 100);
                if (!(spawn > pickupSpawnChance))
                {
                    // spawn pickups
                    var pickup = pickups[Random.Range(0, pickups.Length)];
                    var pickupPos = new Vector3((player.position.x + x) - width / 2, perlinHeight + 2, 0);
                    Instantiate(pickup, pickupPos, Quaternion.identity);
                }
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
        var currentPos = player.position;
        var currentX = Mathf.FloorToInt(currentPos.x);
        
        // foreach (var currentBackground in _currentBackgrounds)
        // {
        //     Destroy(currentBackground);
        // }
        //
        // var rotation = Quaternion.Euler(0, -90, 0);
        // _currentBackgrounds[0] = Instantiate(backgroundPrefab, new Vector3(currentX, 0, 0), rotation);
        // _currentBackgrounds[1] = Instantiate(backgroundPrefab, new Vector3(currentX + _backgroundWidth, 0, 0), rotation);
        // _currentBackgrounds[2] = Instantiate(backgroundPrefab, new Vector3(currentX + (_backgroundWidth * 2), 0, 0), rotation);
        // _currentBackgrounds[3] = Instantiate(backgroundPrefab, new Vector3(currentX - _backgroundWidth, 0, 0), rotation);
        // _currentBackgrounds[4] = Instantiate(backgroundPrefab, new Vector3(currentX - (_backgroundWidth * 2), 0, 0), rotation);
        
        foreach (var currentBackground in _currentBackgrounds)
        {
            var current = currentBackground.transform.position;
            current.x += _lastPlayerX - currentX;
            currentBackground.transform.position = current;
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (map[x, y] != 1) continue;
                tilemap.SetTile(new Vector3Int((currentX - width / 2) + x, y, 0), groundTileBase);
            }
        }
    }
}