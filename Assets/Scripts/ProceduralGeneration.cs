using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour {
	[SerializeField] int width, height;

	[SerializeField] float smoothness;

	[SerializeField] float seed;

	[SerializeField] TileBase groundTile;

	[SerializeField] Tilemap groundTilemap;

	private int[,] _map;

	// Start is called before the first frame update
	private void Start() {
		seed = Random.Range(-1000, 1000);
		Generation();
	}

	public void OnRegenerateWorld() {
		seed = Random.Range(-1000, 1000);
		Generation();
	}

	private void Generation() {
		groundTilemap.ClearAllTiles();
		_map = GenerateArray(width, height, true);
		_map = TerrainGeneration(_map);
		RenderMap(_map, groundTilemap, groundTile);
	}

	private static int[,] GenerateArray(int lWidth, int lHeight, bool empty) {
		var map = new int[lWidth, lHeight];

		for (var x = 0; x < lWidth; x++) {
			for (var y = 0; y < lHeight; y++) {
				map[x, y] = empty ? 0 : 1;
			}
		}

		return map;
	}

	private int[,] TerrainGeneration(int[,] map) {
		for (var x = 0; x < width; x++) {
			var perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(seed, x / smoothness) * height);
			for (var y = 0; y < perlinHeight; y++) {
				map[x, y] = 1;
			}
		}

		return map;
	}

	private void RenderMap(int[,] map, Tilemap tilemap, TileBase groundTileBase) {
		for (var x = 0; x < width; x++) {
			for (var y = 0; y < height; y++) {
				if (map[x, y] == 1) {
					tilemap.SetTile(new Vector3Int(x, y, 0), groundTileBase);
				}
			}
		}
	}
}