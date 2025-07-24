using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator Instance { get; private set; }
    public static bool IsReady => Instance.isReady;
    public static WorldSettings Settings => Instance.worldSettings;
    public static int[,,] Map => Instance.map;

    public static int RenderDistanceHorizontal => Instance.renderDistanceHorizontal;
    public static int RenderDistanceVertical => Instance.renderDistanceVertical;

    protected int[,,] map;

    [Header("World Settings")]
    public List<Biome> biomes = new List<Biome>();
    public int chunkSize = 8;
    public int numPointsPerAxis = 4;

    public int threadGroupSize = 8;

    private bool isReady = false;

    public enum WorldGenMode
    {
        BlendCaseTest,
        IslandTest,
        CaveTest,
    }

    [Header("Generation Settings")]
    public Chunk.NeighborSearch neighborSearchMode = Chunk.NeighborSearch.All;
    public WorldGenMode worldGenMode = WorldGenMode.BlendCaseTest;
    [ShowIf("@worldGenMode != WorldGenMode.BlendCaseTest")] public int setSeed = 0;
    [ShowIf("@worldGenMode != WorldGenMode.BlendCaseTest")] public int renderDistanceHorizontal = 2;
    [ShowIf("@worldGenMode != WorldGenMode.BlendCaseTest")] public int renderDistanceVertical = 1;

    [ShowIf("@worldGenMode != WorldGenMode.BlendCaseTest")] public Vector3Int worldSize = new Vector3Int(5, 5, 5);
    [ShowIf("@worldGenMode == WorldGenMode.BlendCaseTest"), InfoBox("Expand in order to see the full 3x3x3 blend case map."), InfoBox("-1 = blend chunk | 0+ chunks are index in biomes list")] public BlendCaseMap blendCaseMap = new BlendCaseMap();

    private WorldSettings worldSettings;

    #region Unity Methods
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public int GetChunkAt(Vector3Int position)
    {
        return map[position.x, position.y, position.z];
    }

    public Biome GetBiomeFromIndex(int index)
    {
        if (index < 0 || index >= biomes.Count)
        {
            Debug.LogError($"Biome index {index} is out of bounds. Returning default biome.");
            return biomes[0]; // Return the first biome as a fallback
        }
        return biomes[index];
    }

    public void GenerateWorld()
    {
        isReady = false;

        worldSettings = GenerateWorldSettings();
        map = MapGenerator.Instance.GenerateMap();
        ChunkGenerator.Instance.GenerateChunks();

        isReady = true;
    }

    private WorldSettings GenerateWorldSettings()
    {
        int seed = setSeed != 0 ? setSeed : UnityEngine.Random.Range(0, int.MaxValue);

        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numVoxels = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis; // I have no clue why this is the same as numPoints
        int numVoxelsPerAxis = numPointsPerAxis - 1;

        Vector3Int trueWorldSize;

        if (worldGenMode != WorldGenMode.BlendCaseTest)
        {
            trueWorldSize = worldSize * chunkSize;
        }
        else
        {
            trueWorldSize = Vector3Int.one * 3; // 3x3x3 blend case grid
        }

        WorldSettings newWorldSettings = new WorldSettings
        {
            seed = seed,

            chunkSize = chunkSize,

            numPointsPerAxis = numPointsPerAxis,
            numPoints = numPoints,
            numVoxelsPerAxis = numVoxelsPerAxis,
            numVoxels = numVoxels,

            maxTriangleCount = numVoxels * 5,
            numThreadsPerAxis = Mathf.CeilToInt((float)numVoxelsPerAxis / threadGroupSize),
            boundsSize = chunkSize,
            pointSpacing = chunkSize / ((float)numPointsPerAxis - 1),

            worldSize = trueWorldSize,
        };

        return newWorldSettings;
    }
}