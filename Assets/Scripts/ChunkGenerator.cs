using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }
    public static Dictionary<Vector3Int, Chunk> Chunks => Instance.chunks;

    private Transform chunkHolder;
    private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    private WorldSettings ws => WorldGenerator.Settings;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void GenerateChunks()
    {
        RefreshChunksHolder();

        Vector3Int worldSize = new Vector3Int(
            WorldGenerator.Map.GetLength(0),
            WorldGenerator.Map.GetLength(1),
            WorldGenerator.Map.GetLength(2)
        );

        for (int x = 0; x < worldSize.x; x++)
        {
            for (int y = 0; y < worldSize.y; y++)
            {
                for (int z = 0; z < worldSize.z; z++)
                {
                    GenerateChunk(new Vector3Int(x, y, z));
                }
            }
        }

        foreach (var chunk in chunks.Values)
        {
            chunk.SecondInitialize();
        }

        foreach (var chunk in chunks.Values)
        {
            if (chunk is BiomeChunk biomeChunk)
            {
                biomeChunk.GenerateDensity();
            }
        }

        foreach (var chunk in chunks.Values)
        {
            if (chunk is BlendChunk blendChunk)
            {
                blendChunk.GenerateBlendedDensity();
            }
        }

        foreach (var chunk in chunks.Values)
        {
            chunk.GenerateMesh();
        }
    }

    public void GenerateChunk(Vector3Int position)
    {
        GameObject chunkObject = new GameObject($"Chunk ({position.x}, {position.y}, {position.z})");
        chunkObject.transform.SetParent(chunkHolder);
        chunkObject.transform.position = position * ws.chunkSize;

        Chunk chunk;

        if (WorldGenerator.Map[position.x, position.y, position.z] >= 0)
        {
            int biomeIndex = WorldGenerator.Map[position.x, position.y, position.z];
            Biome biome = WorldGenerator.Instance.biomes[biomeIndex];

            chunk = chunkObject.AddComponent<BiomeChunk>();
            chunk.Initialize(position, biome);
        }
        else if (WorldGenerator.Map[position.x, position.y, position.z] == -1)
        {
            chunk = chunkObject.AddComponent<BlendChunk>();
            chunk.Initialize(position, null); // No biome for blend case chunks
        }
        else
        {
            Debug.LogError($"Invalid map value at {position}: {WorldGenerator.Map[position.x, position.y, position.z]}");
            return;
        }

        chunks.Add(position, chunk);
    }

    private void RefreshChunksHolder()
    {
        chunks.Clear();

        if (chunkHolder != null)
        {
            Destroy(chunkHolder.gameObject);
        }

        chunkHolder = new GameObject("Chunks").transform;
        chunkHolder.SetParent(transform);
    }
}
